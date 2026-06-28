# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (better layer caching)
COPY HomeLedger.API.csproj .
RUN dotnet restore 

# Build & publish
COPY . .
RUN dotnet publish \
    -c Release -o /app/publish /p:UseAppHost=false

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5265
ENV ASPNETCORE_URLS=http://+:5265

ENTRYPOINT ["dotnet", "HomeLedger.API.dll"]
