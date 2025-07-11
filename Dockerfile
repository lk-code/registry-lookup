FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .

# Restore dependencies
RUN dotnet restore "source/RegistryLookup.Backend/RegistryLookup.Backend.csproj"
RUN dotnet restore "source/RegistryLookup.Frontend/RegistryLookup.Frontend.csproj"

# Publish Blazor frontend
RUN dotnet publish "source/RegistryLookup.Frontend/RegistryLookup.Frontend.csproj" -c $BUILD_CONFIGURATION -o /frontend_dist

# Copy Blazor output into backend wwwroot
RUN mkdir -p /src/source/RegistryLookup.Backend/wwwroot \
    && cp -r /frontend_dist/wwwroot/* /src/source/RegistryLookup.Backend/wwwroot/

# Publish backend
RUN dotnet publish "source/RegistryLookup.Backend/RegistryLookup.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
RUN rm -rf /usr/share/nginx/html/*
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY --from=build /app/publish /app
WORKDIR /app

CMD ["/bin/sh", "-c", "dotnet RegistryLookup.Backend.dll & nginx -g 'daemon off;'"]