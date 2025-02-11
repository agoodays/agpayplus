# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ENV LANG=C.UTF-8 \
    LC_ALL=C.UTF-8 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
USER app
WORKDIR /app
EXPOSE 5816
EXPOSE 9816
ENV ASPNETCORE_URLS=http://+:5816;https://+:9816

# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AGooday.AgPay.Agent.Api/AGooday.AgPay.Agent.Api.csproj", "AGooday.AgPay.Agent.Api/"]
COPY ["AGooday.AgPay.AopSdk/AGooday.AgPay.AopSdk.csproj", "AGooday.AgPay.AopSdk/"]
COPY ["AGooday.AgPay.Application/AGooday.AgPay.Application.csproj", "AGooday.AgPay.Application/"]
COPY ["AGooday.AgPay.Infrastructure/AGooday.AgPay.Infrastructure.csproj", "AGooday.AgPay.Infrastructure/"]
COPY ["AGooday.AgPay.Domain/AGooday.AgPay.Domain.csproj", "AGooday.AgPay.Domain/"]
COPY ["AGooday.AgPay.Common/AGooday.AgPay.Common.csproj", "AGooday.AgPay.Common/"]
COPY ["AGooday.AgPay.Components.MQ/AGooday.AgPay.Components.MQ.csproj", "AGooday.AgPay.Components.MQ/"]
COPY ["AGooday.AgPay.Domain.Core/AGooday.AgPay.Domain.Core.csproj", "AGooday.AgPay.Domain.Core/"]
COPY ["AGooday.AgPay.Notice.Email/AGooday.AgPay.Notice.Email.csproj", "AGooday.AgPay.Notice.Email/"]
COPY ["AGooday.AgPay.Notice.Core/AGooday.AgPay.Notice.Core.csproj", "AGooday.AgPay.Notice.Core/"]
COPY ["AGooday.AgPay.Components.Cache/AGooday.AgPay.Components.Cache.csproj", "AGooday.AgPay.Components.Cache/"]
COPY ["AGooday.AgPay.Components.OSS/AGooday.AgPay.Components.OSS.csproj", "AGooday.AgPay.Components.OSS/"]
COPY ["AGooday.AgPay.Components.SMS/AGooday.AgPay.Components.SMS.csproj", "AGooday.AgPay.Components.SMS/"]
RUN dotnet restore "./AGooday.AgPay.Agent.Api/AGooday.AgPay.Agent.Api.csproj"
COPY . .
WORKDIR "/src/AGooday.AgPay.Agent.Api"
RUN dotnet build "./AGooday.AgPay.Agent.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AGooday.AgPay.Agent.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AGooday.AgPay.Agent.Api.dll"]