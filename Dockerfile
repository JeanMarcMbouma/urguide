# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY UrGuide.slnx ./
COPY UrGuide.WebApp/*.csproj ./UrGuide.WebApp/
COPY UrGuide.Services/*.csproj ./UrGuide.Services/
COPY UrGuide.Data/*.csproj ./UrGuide.Data/
COPY UrGuide.Model/*.csproj ./UrGuide.Model/
COPY UrGuide.Core/*.csproj ./UrGuide.Core/
COPY UrGuide.Shared/*.csproj ./UrGuide.Shared/
COPY UrGuide.ServiceDefaults/*.csproj ./UrGuide.ServiceDefaults/

# Restore dependencies
RUN dotnet restore UrGuide.WebApp/UrGuide.WebApp.csproj

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR /src/UrGuide.WebApp
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install curl for healthchecks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create directories and set permissions for non-root user
RUN mkdir -p /app/wwwroot/uploads /app/logs && \
  chown -R app:app /app && \
  chmod -R 755 /app/wwwroot/uploads /app/logs

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

# Set user to non-root for security (using pre-defined app user in aspnet image)
USER app

ENTRYPOINT ["dotnet", "UrGuide.WebApp.dll"]
