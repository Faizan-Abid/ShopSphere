# ---------- Stage 1: Build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only project files first, so restore is cached
COPY Catalog.Domain/Catalog.Domain.csproj Catalog.Domain/
COPY Catalog.Application/Catalog.Application.csproj Catalog.Application/
COPY Catalog.Infrastructure/Catalog.Infrastructure.csproj Catalog.Infrastructure/
COPY Catalog.API/Catalog.API.csproj Catalog.API/
RUN dotnet restore Catalog.API/Catalog.API.csproj

# Now copy the source code and publish
COPY Catalog.Domain/ Catalog.Domain/
COPY Catalog.Application/ Catalog.Application/
COPY Catalog.Infrastructure/ Catalog.Infrastructure/
COPY Catalog.API/ Catalog.API/
RUN dotnet publish Catalog.API/Catalog.API.csproj -c Release -o /app/publish --no-restore

# ---------- Stage 2: Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/logs && chown -R $APP_UID /app/logs
USER $APP_UID

EXPOSE 8080
ENTRYPOINT ["dotnet", "Catalog.API.dll"]