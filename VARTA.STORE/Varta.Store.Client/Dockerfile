# Stage 1: Build the C# Blazor code
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Varta.Store.Client/Varta.Store.Client.csproj", "Varta.Store.Client/"]
COPY ["Varta.Store.Shared/Varta.Store.Shared.csproj", "Varta.Store.Shared/"]
RUN dotnet restore "Varta.Store.Client/Varta.Store.Client.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/Varta.Store.Client"
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Serve with Nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html
# Remove default nginx static assets
RUN rm -rf ./*
# Copy the built Blazor files from Stage 1
COPY --from=build /app/publish/wwwroot .
# Copy our custom nginx config
COPY Varta.Store.Client/nginx.conf /etc/nginx/nginx.conf