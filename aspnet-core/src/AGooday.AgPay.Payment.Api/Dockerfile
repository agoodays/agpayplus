# ========================================
# AgPay+ Payment API Dockerfile
# ========================================
# 多阶段构建，支持可选的 cashier 构建
# ========================================
# 构建参数：
# BUILD_CASHIER=true  - 构建新的 cashier
# BUILD_CASHIER=false - 跳过 cashier 构建（默认）
# ========================================
# 使用方法（必须在项目根目录执行）：
#
# 推荐方式 1：使用 docker-compose（自动处理路径）
#   docker compose build agpay-payment-api                             # 不构建 cashier
#   docker compose build --build-arg BUILD_CASHIER=true agpay-payment-api  # 构建 cashier
#
# 推荐方式 2：使用部署脚本
#   .\deploy.ps1 -Environment dev                    # Windows，不构建 cashier
#   .\deploy.ps1 -Environment dev -BuildCashier      # Windows，构建 cashier
#   ./deploy.sh -e dev                               # Linux，不构建 cashier
#   ./deploy.sh -e dev -b                            # Linux，构建 cashier
#
# 直接使用 docker build（需指定 Dockerfile 路径）：
#   cd <项目根目录>
#   docker build -f aspnet-core/src/AGooday.AgPay.Payment.Api/Dockerfile -t agpay-payment-api .
#   docker build --build-arg BUILD_CASHIER=true -f aspnet-core/src/AGooday.AgPay.Payment.Api/Dockerfile -t agpay-payment-api .
# ========================================

# 构建参数
ARG BUILD_CASHIER=false

# ========================================
# 阶段 1: 构建收银台前端 (Cashier) - 可选
# ========================================
FROM node:16-alpine AS cashier-builder
ARG BUILD_CASHIER
WORKDIR /cashier

# 使用条件复制和构建
# 如果 BUILD_CASHIER=false，创建空目录
RUN echo "BUILD_CASHIER=$BUILD_CASHIER"

# 设置 npm 镜像源和超时时间
RUN npm config set registry https://registry.npmmirror.com && \
    npm config set timeout 120000 && \
    npm config set fetch-timeout 120000 && \
    npm config set fetch-retries 3

# 复制 cashier package.json（如果不存在会失败，这是预期行为）
COPY ant-design-vue/agpay-ui-cashier/package*.json ./

# 条件安装依赖
RUN if [ "$BUILD_CASHIER" = "true" ]; then \
        echo "🔨 Building cashier frontend..."; \
        npm install; \
    else \
        echo "⏭️  Skipping cashier npm install"; \
    fi

# 复制 cashier 源代码
COPY ant-design-vue/agpay-ui-cashier/. .

# 条件构建或创建空目录
RUN if [ "$BUILD_CASHIER" = "true" ]; then \
        npm run build; \
        echo "✅ Cashier build completed"; \
    else \
        echo "ℹ️  Skipping cashier build, creating empty dist"; \
        mkdir -p /cashier/dist; \
    fi

# ========================================
# 阶段 2: 基础运行时环境
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ENV LANG=C.UTF-8 \
    LC_ALL=C.UTF-8 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=Asia/Shanghai

# 设置时区
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

USER app
WORKDIR /app
EXPOSE 5819
EXPOSE 9819
ENV ASPNETCORE_URLS=http://+:5819;https://+:9819

# ========================================
# 阶段 3: 构建 .NET 后端
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 复制项目文件（从项目根目录开始）
COPY aspnet-core/src/AGooday.AgPay.Payment.Api/AGooday.AgPay.Payment.Api.csproj aspnet-core/src/AGooday.AgPay.Payment.Api/
COPY aspnet-core/src/AGooday.AgPay.Application/AGooday.AgPay.Application.csproj aspnet-core/src/AGooday.AgPay.Application/
COPY aspnet-core/src/AGooday.AgPay.Base.Api/AGooday.AgPay.Base.Api.csproj aspnet-core/src/AGooday.AgPay.Base.Api/
COPY aspnet-core/src/AGooday.AgPay.Infrastructure/AGooday.AgPay.Infrastructure.csproj aspnet-core/src/AGooday.AgPay.Infrastructure/
COPY aspnet-core/src/AGooday.AgPay.Domain/AGooday.AgPay.Domain.csproj aspnet-core/src/AGooday.AgPay.Domain/
COPY aspnet-core/src/AGooday.AgPay.Common/AGooday.AgPay.Common.csproj aspnet-core/src/AGooday.AgPay.Common/
COPY aspnet-core/src/AGooday.AgPay.Components.MQ/AGooday.AgPay.Components.MQ.csproj aspnet-core/src/AGooday.AgPay.Components.MQ/
COPY aspnet-core/src/AGooday.AgPay.Domain.Core/AGooday.AgPay.Domain.Core.csproj aspnet-core/src/AGooday.AgPay.Domain.Core/
COPY aspnet-core/src/AGooday.AgPay.Logging.Serilog/AGooday.AgPay.Logging.Serilog.csproj aspnet-core/src/AGooday.AgPay.Logging.Serilog/
COPY aspnet-core/src/AGooday.AgPay.Notice.Email/AGooday.AgPay.Notice.Email.csproj aspnet-core/src/AGooday.AgPay.Notice.Email/
COPY aspnet-core/src/AGooday.AgPay.Notice.Core/AGooday.AgPay.Notice.Core.csproj aspnet-core/src/AGooday.AgPay.Notice.Core/
COPY aspnet-core/src/AGooday.AgPay.Components.Cache/AGooday.AgPay.Components.Cache.csproj aspnet-core/src/AGooday.AgPay.Components.Cache/
COPY aspnet-core/src/AGooday.AgPay.Components.OSS/AGooday.AgPay.Components.OSS.csproj aspnet-core/src/AGooday.AgPay.Components.OSS/
COPY aspnet-core/src/AGooday.AgPay.Components.SMS/AGooday.AgPay.Components.SMS.csproj aspnet-core/src/AGooday.AgPay.Components.SMS/
COPY aspnet-core/src/AGooday.AgPay.Components.Third/AGooday.AgPay.Components.Third.csproj aspnet-core/src/AGooday.AgPay.Components.Third/

# 还原依赖
RUN dotnet restore "aspnet-core/src/AGooday.AgPay.Payment.Api/AGooday.AgPay.Payment.Api.csproj"

# 复制所有源代码
COPY aspnet-core/src/. aspnet-core/src/

# 构建项目
WORKDIR "/src/aspnet-core/src/AGooday.AgPay.Payment.Api"
RUN dotnet build "AGooday.AgPay.Payment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ========================================
# 阶段 4: 发布 .NET 应用
# ========================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AGooday.AgPay.Payment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ========================================
# 阶段 5: 最终镜像（整合后端 + 前端）
# ========================================
FROM base AS final
ARG BUILD_CASHIER
WORKDIR /app

# 创建日志和上传目录
RUN mkdir -p /app/agpayplus/logs && \
    mkdir -p /app/agpayplus/upload && \
    chmod -R 777 /app/agpayplus/logs && \
    chmod -R 777 /app/agpayplus/upload

# 复制后端应用
COPY --from=publish /app/publish .

# 条件复制 cashier
# 如果 BUILD_CASHIER=true，从 cashier-builder 复制
# 如果 BUILD_CASHIER=false，cashier 应该已经存在于 wwwroot/cashier 中
RUN if [ "$BUILD_CASHIER" = "true" ]; then \
        echo "📦 Copying newly built cashier"; \
    else \
        echo "📦 Using existing cashier from publish"; \
    fi

# 复制 cashier（仅在构建时）
COPY --from=cashier-builder /cashier/dist ./wwwroot/cashier

# 显示构建信息
RUN echo "========================================" && \
    echo "AgPay+ Payment API" && \
    echo "BUILD_CASHIER: $BUILD_CASHIER" && \
    echo "========================================" && \
    ls -la ./wwwroot/cashier/ || echo "No cashier directory"

ENTRYPOINT ["dotnet", "AGooday.AgPay.Payment.Api.dll"]