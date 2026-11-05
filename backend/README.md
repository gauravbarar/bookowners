# Book owners API

A .NET API that serves book owners in the same JSON shape as the Bupa book owners API, backed by
MSSQL. A feature flag can switch the data source to the real Bupa API.

```
.NET API -> IBookOwnerSource -> MSSQL            (default)
                             -> Bupa HTTP API    (UseBupaApi = true)
```

.NET 10, ASP.NET Core minimal APIs, EF Core and SQL Server, in Clean Architecture layers:

```
src/BookOwners.Domain/          entities, no dependencies
src/BookOwners.Application/     use cases, DTOs, IBookOwnerSource
src/BookOwners.Infrastructure/  EF Core, migrations, data sources
src/BookOwners.Api/             endpoints, error handling, startup
tests/                          unit and integration tests
clients/typescript/             the @books/api-client package for API consumers
```

See [ASSUMPTIONS.md](ASSUMPTIONS.md) for the decisions made along the way.

## Run with Docker

Only Docker is needed. This starts the API and SQL Server.

```bash
cp .env.example .env     # then set MSSQL_SA_PASSWORD
docker compose up --build -d
```

The API is on http://localhost:5080. Stop it with `docker compose down`, and add `-v` to delete the
database too.

To build only the API image: `docker build -t books-backend .`

## Run for local development

Needs the .NET SDK 10 as well as Docker for SQL Server. In Development the API applies migrations
on startup.

```bash
cp .env.example .env     # then set MSSQL_SA_PASSWORD
docker compose up -d sqlserver
dotnet tool restore
dotnet user-secrets set "ConnectionStrings:BookOwners" \
  "Server=127.0.0.1,1433;Database=BookOwners;User Id=sa;Password=<your password>;TrustServerCertificate=True" \
  --project src/BookOwners.Api
dotnet run --project src/BookOwners.Api
```

Use `127.0.0.1` rather than `localhost`. SQL Server is published on IPv4 only, and `localhost`
tries IPv6 first and times out.

## API

| Route                    | Purpose                                             |
| ------------------------ | --------------------------------------------------- |
| `GET /api/v1/bookowners` | Book owners, in the same JSON shape as the Bupa API |
| `GET /health`            | Health check, including the database                |
| `GET /openapi/v1.json`   | OpenAPI document (Development only)                 |
| `GET /swagger`           | Swagger UI (Development only)                       |

Swagger UI is on http://localhost:5080/swagger when running with `dotnet run`. The Docker image runs
in Production, so these two Development routes return 404 there.

Response shape:

```json
[{ "name": "Jane", "age": 23, "books": [{ "name": "Hamlet", "type": "Hardcover" }] }]
```

## Configuration

| Setting                             | Purpose                                    | Default |
| ----------------------------------- | ------------------------------------------ | ------- |
| `ConnectionStrings:BookOwners`      | SQL Server connection                      | none    |
| `Cors:AllowedOrigins`               | Browser origins allowed to call the API    | none    |
| `FeatureManagement:UseBupaApi`      | Read from the Bupa API instead of MSSQL    | `false` |
| `Database:ApplyMigrationsOnStartup` | Apply EF Core migrations on startup        | `false` |
| `BupaApi:MaxAttempts`               | Attempts when a response body is unusable  | `3`     |
| `BupaApi:RetryDelay`                | Base wait between those attempts           | 200 ms  |

With Docker, `FRONTEND_ORIGIN` and `USE_BUPA_API` in `.env` set the CORS origin and the flag.

## See the error handling working

The Bupa API regularly returns 429s, empty bodies and corrupted data, so turning the flag on is the
way to exercise the error handling. Set `USE_BUPA_API=true` in `.env`, then recreate the API:

```bash
docker compose up -d backend
```

For local development, use `FeatureManagement__UseBupaApi=true dotnet run --project src/BookOwners.Api`.

| Upstream response                   | What happens                                      |
| ----------------------------------- | ------------------------------------------------- |
| 429 or 5xx                          | Retried by the HTTP resilience handler            |
| 200 with an empty or corrupted body | Rejected by strict parsing, retried up to 3 times |
| Still failing after retries         | API returns a 502 ProblemDetails                  |

Partial data is never returned as a success.

## Client package

`clients/typescript` builds `@books/api-client`, a types-only npm package generated from the API's
OpenAPI document. Consumers use it to check their code against the contract.

When the API's contract changes:

1. Bump `version` in `clients/typescript/package.json`. A breaking change gets a new major version.
2. Build the package. This needs Node.js 24 as well as the .NET SDK.

   ```bash
   cd clients/typescript
   npm install
   npm run package
   ```

3. Publish the `.tgz` it creates. There is no registry yet, so copy it into the frontend's `vendor/`
   folder and follow the upgrade steps in the frontend README.

Commit the regenerated `openapi.json` and `generated.d.ts` with the API change.

## Test

```bash
dotnet test
```

## Database migrations

```bash
dotnet ef migrations add <Name> -p src/BookOwners.Infrastructure -s src/BookOwners.Infrastructure -o Persistence/Migrations
```
