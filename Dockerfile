# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/Matrimonial.AdminApi/Matrimonial.AdminApi.csproj backend/Matrimonial.AdminApi/
RUN dotnet restore backend/Matrimonial.AdminApi/Matrimonial.AdminApi.csproj

COPY backend/Matrimonial.AdminApi/ backend/Matrimonial.AdminApi/
WORKDIR /src/backend/Matrimonial.AdminApi
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render sets PORT; ASP.NET Core listens on this URL
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Matrimonial.AdminApi.dll"]
