#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/AGooday.AgPay.Payment.Api/AGooday.AgPay.Payment.Api.csproj", "src/AGooday.AgPay.Payment.Api/"]
RUN dotnet restore "src/AGooday.AgPay.Payment.Api/AGooday.AgPay.Payment.Api.csproj"
COPY . .
WORKDIR "/src/src/AGooday.AgPay.Payment.Api"
RUN dotnet build "AGooday.AgPay.Payment.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AGooday.AgPay.Payment.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AGooday.AgPay.Payment.Api.dll"]