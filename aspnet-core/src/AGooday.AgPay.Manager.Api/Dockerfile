#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 5017
EXPOSE 9017

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
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
RUN dotnet restore "./src/AGooday.AgPay.Manager.Api/AGooday.AgPay.Manager.Api.csproj"
COPY . .
WORKDIR "/src/src/AGooday.AgPay.Manager.Api"
RUN dotnet build "./AGooday.AgPay.Manager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AGooday.AgPay.Manager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AGooday.AgPay.Manager.Api.dll"]