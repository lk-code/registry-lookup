FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .
RUN dotnet restore "RegistryLookup.csproj"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RegistryLookup.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
RUN rm -rf /usr/share/nginx/html/*
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html