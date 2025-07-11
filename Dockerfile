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

# Publish backend
RUN dotnet publish "source/RegistryLookup.Backend/RegistryLookup.Backend.csproj" -c $BUILD_CONFIGURATION -o /backend_dist /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
EXPOSE 80

RUN apt-get update && \
    apt-get install -y nginx && \
    rm -rf /var/lib/apt/lists/*

RUN rm -rf /usr/share/nginx/html/*

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /frontend_dist/wwwroot /usr/share/nginx/html

COPY --from=build /backend_dist /app
WORKDIR /app

CMD ["/bin/sh", "-c", "dotnet /app/RegistryLookup.Backend.dll & nginx -g 'daemon off;'"]