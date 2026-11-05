FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Project files first, so the restore layer is cached until dependencies change.
COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/BookOwners.Domain/BookOwners.Domain.csproj src/BookOwners.Domain/
COPY src/BookOwners.Application/BookOwners.Application.csproj src/BookOwners.Application/
COPY src/BookOwners.Infrastructure/BookOwners.Infrastructure.csproj src/BookOwners.Infrastructure/
COPY src/BookOwners.Api/BookOwners.Api.csproj src/BookOwners.Api/
RUN dotnet restore src/BookOwners.Api/BookOwners.Api.csproj

COPY src/ src/
RUN dotnet publish src/BookOwners.Api/BookOwners.Api.csproj \
    --configuration Release --no-restore --output /app/publish -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

USER app
EXPOSE 8080

# The image has no curl, so the /health request is made with bash.
HEALTHCHECK --interval=10s --timeout=5s --start-period=20s --retries=6 \
    CMD ["bash", "-c", "exec 3<>/dev/tcp/127.0.0.1/8080 && printf 'GET /health HTTP/1.0\\r\\nHost: localhost\\r\\n\\r\\n' >&3 && grep -q '200 OK' <&3"]

ENTRYPOINT ["dotnet", "BookOwners.Api.dll"]
