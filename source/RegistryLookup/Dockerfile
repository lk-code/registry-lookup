FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .
RUN dotnet restore

RUN dotnet publish -c Release -o /app/publish

FROM nginx:alpine AS final

RUN rm -rf /usr/share/nginx/html/*

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html