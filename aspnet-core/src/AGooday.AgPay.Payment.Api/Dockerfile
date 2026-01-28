# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# ============================================
# 阶段 1: 构建 Cashier 前端
# ============================================
FROM node:16-alpine AS cashier-build
WORKDIR /cashier

# 设置 npm 镜像
RUN npm config set registry https://registry.npmmirror.com && \
    npm config set timeout 60000

# 复制 cashier 前端项目文件
COPY ../../ant-design-vue/agpay-ui-cashier/package*.json ./
RUN npm install

# 复制源代码并构建
COPY ../../ant-design-vue/agpay-ui-cashier/. .
RUN npm run build

# ============================================
# 阶段 2: 运行时基础镜像
# ============================================
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

# ============================================
# 阶段 3: 构建 .NET 后端
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AGooday.AgPay.Payment.Api/AGooday.AgPay.Payment.Api.csproj", "AGooday.AgPay.Payment.Api/"]
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
COPY ["AGooday.AgPay.Components.OSS/AGooday.AgPay.Components.OSS.csproj", "AGooday.AgPay.Components.OSS/"]
COPY ["AGooday.AgPay.Components.SMS/AGooday.AgPay.Components.SMS.csproj", "AGooday.AgPay.Components.SMS/"]
COPY ["AGooday.AgPay.Components.Third/AGooday.AgPay.Components.Third.csproj", "AGooday.AgPay.Components.Third/"]
RUN dotnet restore "./AGooday.AgPay.Payment.Api/AGooday.AgPay.Payment.Api.csproj"
COPY . .
WORKDIR "/src/AGooday.AgPay.Payment.Api"
RUN dotnet build "./AGooday.AgPay.Payment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ============================================
# 阶段 4: 发布 .NET 应用
# ============================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AGooday.AgPay.Payment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ============================================
# 阶段 5: 最终运行镜像
# ============================================
FROM base AS final
WORKDIR /app

# 创建日志和上传目录
RUN mkdir -p /app/agpayplus/logs && \
    mkdir -p /app/agpayplus/upload && \
    chmod -R 777 /app/agpayplus/logs && \
    chmod -R 777 /app/agpayplus/upload

# 复制 .NET 应用
COPY --from=publish /app/publish .

# 复制 Cashier 前端到 wwwroot/cashier
COPY --from=cashier-build /cashier/dist ./wwwroot/cashier

ENTRYPOINT ["dotnet", "AGooday.AgPay.Payment.Api.dll"]