# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY src/Lexicon.Domain/*.csproj ./src/Lexicon.Domain/
COPY src/Lexicon.Application/*.csproj ./src/Lexicon.Application/
COPY src/Lexicon.Infrastructure/*.csproj ./src/Lexicon.Infrastructure/
COPY src/Lexicon.Api/*.csproj ./src/Lexicon.Api/

# Restore dependencies
RUN dotnet restore src/Lexicon.Api/Lexicon.Api.csproj

# Copy source code
COPY src/ ./src/

# Build and publish
WORKDIR /src/src/Lexicon.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser

# Copy published files
COPY --from=build /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "Lexicon.Api.dll"]
