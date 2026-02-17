# Build the API
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["Varta.Store.API/Varta.Store.API.csproj", "Varta.Store.API/"]
COPY ["Varta.Store.Shared/Varta.Store.Shared.csproj", "Varta.Store.Shared/"]
RUN dotnet restore "Varta.Store.API/Varta.Store.API.csproj"

# Copy everything else
COPY . .
WORKDIR "/src/Varta.Store.API"
RUN dotnet publish -c Release -o /app/publish

# Run the API
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Varta.Store.API.dll"]