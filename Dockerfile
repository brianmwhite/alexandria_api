FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY alexandria_api.csproj ./
RUN dotnet restore alexandria_api.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish alexandria_api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build-env /app/out ./

EXPOSE 5384

ENTRYPOINT ["dotnet", "alexandria_api.dll"]