# ========================================
# AgPay+ Manager API Dockerfile
# ========================================
# 多阶段构建：.NET 后端构建 + 运行时部署
# ========================================
# 使用方法：
#
# 推荐方式 1：使用 docker-compose（自动处理 context）
#   docker compose build agpay-manager-api
#
# 推荐方式 2：使用部署脚本
#   .\deploy.ps1 -Environment dev          # Windows
#   ./deploy.sh -e dev                     # Linux
#
# 直接使用 docker build（context 是 aspnet-core/src 目录）：
#   cd <项目根目录>
#   docker build -f aspnet-core/src/AGooday.AgPay.Manager.Api/Dockerfile -t agpay-manager-api ./aspnet-core/src
# ========================================

# 使用 .NET SDK 镜像作为构建阶段
# 基础镜像：mcr.microsoft.com/dotnet/aspnet:9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# 设置语言环境变量以支持 UTF-8 编码，并确保全球化设置不被禁用
ENV LANG=C.UTF-8 \
    LC_ALL=C.UTF-8 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=Asia/Shanghai

# 设置时区
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# 设置运行应用的用户为非 root 用户 'app'
USER app

# 设置工作目录
WORKDIR /app

# 暴露端口 5817 和 9817
EXPOSE 5817
EXPOSE 9817

# 设置 ASP.NET Core 监听的 URL（HTTP 和 HTTPS）
ENV ASPNETCORE_URLS=http://+:5817;https://+:9817

# 使用 .NET SDK 镜像进行构建和发布
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

# 设置工作目录
WORKDIR /src

# 复制项目文件到容器中
COPY ["AGooday.AgPay.Manager.Api/AGooday.AgPay.Manager.Api.csproj", "AGooday.AgPay.Manager.Api/"]
COPY ["AGooday.AgPay.AopSdk/AGooday.AgPay.AopSdk.csproj", "AGooday.AgPay.AopSdk/"]
COPY ["AGooday.AgPay.Application/AGooday.AgPay.Application.csproj", "AGooday.AgPay.Application/"]
COPY ["AGooday.AgPay.Base.Api/AGooday.AgPay.Base.Api.csproj", "AGooday.AgPay.Base.Api/"]
COPY ["AGooday.AgPay.Infrastructure/AGooday.AgPay.Infrastructure.csproj", "AGooday.AgPay.Infrastructure/"]
COPY ["AGooday.AgPay.Domain/AGooday.AgPay.Domain.csproj", "AGooday.AgPay.Domain/"]
COPY ["AGooday.AgPay.Common/AGooday.AgPay.Common.csproj", "AGooday.AgPay.Common/"]
COPY ["AGooday.AgPay.Components.MQ/AGooday.AgPay.Components.MQ.csproj", "AGooday.AgPay.Components.MQ/"]
COPY ["AGooday.AgPay.Domain.Core/AGooday.AgPay.Domain.Core.csproj", "AGooday.AgPay.Domain.Core/"]
COPY ["AGooday.AgPay.Logging.Serilog/AGooday.AgPay.Logging.Serilog.csproj", "AGooday.AgPay.Logging.Serilog/"]
COPY ["AGooday.AgPay.Notice.Email/AGooday.AgPay.Notice.Email.csproj", "AGooday.AgPay.Notice.Email/"]
COPY ["AGooday.AgPay.Notice.Core/AGooday.AgPay.Notice.Core.csproj", "AGooday.AgPay.Notice.Core/"]
COPY ["AGooday.AgPay.Components.Cache/AGooday.AgPay.Components.Cache.csproj", "AGooday.AgPay.Components.Cache/"]
COPY ["AGooday.AgPay.Components.OCR/AGooday.AgPay.Components.OCR.csproj", "AGooday.AgPay.Components.OCR/"]
COPY ["AGooday.AgPay.Components.OSS/AGooday.AgPay.Components.OSS.csproj", "AGooday.AgPay.Components.OSS/"]
COPY ["AGooday.AgPay.Components.SMS/AGooday.AgPay.Components.SMS.csproj", "AGooday.AgPay.Components.SMS/"]
COPY ["AGooday.AgPay.Notice.Sms/AGooday.AgPay.Notice.Sms.csproj", "AGooday.AgPay.Notice.Sms/"]

# 还原 NuGet 包
RUN dotnet restore "./AGooday.AgPay.Manager.Api/AGooday.AgPay.Manager.Api.csproj"

# 复制剩余的源代码
COPY . .

# 设置工作目录并构建项目
WORKDIR "/src/AGooday.AgPay.Manager.Api"
RUN dotnet build "./AGooday.AgPay.Manager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 发布阶段，基于构建阶段创建
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AGooday.AgPay.Manager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 运行时阶段，基于之前定义的基础镜像
FROM base AS final
WORKDIR /app

# 创建日志和上传目录
RUN mkdir -p /app/agpayplus/logs && \
    mkdir -p /app/agpayplus/upload && \
    chmod -R 777 /app/agpayplus/logs && \
    chmod -R 777 /app/agpayplus/upload

# 从发布阶段复制发布的文件到运行时镜像中
COPY --from=publish /app/publish .

# 设置入口点
ENTRYPOINT ["dotnet", "AGooday.AgPay.Manager.Api.dll"]