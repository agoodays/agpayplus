#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# 使用 .NET SDK 镜像作为构建阶段
# 基础镜像：mcr.microsoft.com/dotnet/aspnet:9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# 设置语言环境变量以支持 UTF-8 编码，并确保全球化设置不被禁用
ENV LANG=C.UTF-8 \
    LC_ALL=C.UTF-8 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

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
COPY ["src/AGooday.AgPay.Manager.Api/AGooday.AgPay.Manager.Api.csproj", "src/AGooday.AgPay.Manager.Api/"]
COPY ["src/AGooday.AgPay.AopSdk/AGooday.AgPay.AopSdk.csproj", "src/AGooday.AgPay.AopSdk/"]
COPY ["src/AGooday.AgPay.Application/AGooday.AgPay.Application.csproj", "src/AGooday.AgPay.Application/"]
COPY ["src/AGooday.AgPay.Infrastructure/AGooday.AgPay.Infrastructure.csproj", "src/AGooday.AgPay.Infrastructure/"]
COPY ["src/AGooday.AgPay.Common/AGooday.AgPay.Common.csproj", "src/AGooday.AgPay.Common/"]
COPY ["src/AGooday.AgPay.Domain/AGooday.AgPay.Domain.csproj", "src/AGooday.AgPay.Domain/"]
COPY ["src/AGooday.AgPay.Components.MQ/AGooday.AgPay.Components.MQ.csproj", "src/AGooday.AgPay.Components.MQ/"]
COPY ["src/AGooday.AgPay.Domain.Core/AGooday.AgPay.Domain.Core.csproj", "src/AGooday.AgPay.Domain.Core/"]
COPY ["src/AGooday.AgPay.Notice.Email/AGooday.AgPay.Notice.Email.csproj", "src/AGooday.AgPay.Notice.Email/"]
COPY ["src/AGooday.AgPay.Notice.Core/AGooday.AgPay.Notice.Core.csproj", "src/AGooday.AgPay.Notice.Core/"]
COPY ["src/AGooday.AgPay.Components.OCR/AGooday.AgPay.Components.OCR.csproj", "src/AGooday.AgPay.Components.OCR/"]
COPY ["src/AGooday.AgPay.Components.OSS/AGooday.AgPay.Components.OSS.csproj", "src/AGooday.AgPay.Components.OSS/"]
COPY ["src/AGooday.AgPay.Components.SMS/AGooday.AgPay.Components.SMS.csproj", "src/AGooday.AgPay.Components.SMS/"]
COPY ["src/AGooday.AgPay.Notice.Sms/AGooday.AgPay.Notice.Sms.csproj", "src/AGooday.AgPay.Notice.Sms/"]

# 还原 NuGet 包
RUN dotnet restore "./src/AGooday.AgPay.Manager.Api/AGooday.AgPay.Manager.Api.csproj"

# 复制剩余的源代码
COPY . .

# 设置工作目录并构建项目
WORKDIR "/src/src/AGooday.AgPay.Manager.Api"
RUN dotnet build "./AGooday.AgPay.Manager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 发布阶段，基于构建阶段创建
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AGooday.AgPay.Manager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 运行时阶段，基于之前定义的基础镜像
FROM base AS final
WORKDIR /app

# 从发布阶段复制发布的文件到运行时镜像中
COPY --from=publish /app/publish .

# 设置入口点
ENTRYPOINT ["dotnet", "AGooday.AgPay.Manager.Api.dll"]