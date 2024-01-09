#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 安装 GDI+ 库和依赖项
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        libc6-dev \
        libgdiplus \
        fontconfig \
        libfontconfig1-dev \
        libfreetype6-dev \
        libx11-dev \
        libxext-dev \
        libxrender-dev \
        libjpeg62-turbo-dev \
        libpng-dev \
        libgif-dev \
        libwebp-dev \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/AGooday.AgPay.Manager.Api/AGooday.AgPay.Manager.Api.csproj", "src/AGooday.AgPay.Manager.Api/"]
RUN dotnet restore "src/AGooday.AgPay.Manager.Api/AGooday.AgPay.Manager.Api.csproj"
COPY . .
WORKDIR "/src/src/AGooday.AgPay.Manager.Api"
RUN dotnet build "AGooday.AgPay.Manager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AGooday.AgPay.Manager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AGooday.AgPay.Manager.Api.dll"]