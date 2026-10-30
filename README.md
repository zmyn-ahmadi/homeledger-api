# HomeLedger API

The ASP.NET Core backend for HomeLedger, a personal and family finance app. The web client is in [homeledger-client](https://github.com/zmyn-ahmadi/homeledger-client).

![Personal dashboard](https://raw.githubusercontent.com/zmyn-ahmadi/homeledger-client/main/docs/screenshots/personal.png)

## Run the whole app

You only need Docker:

```bash
git clone https://github.com/zmyn-ahmadi/homeledger-api.git
cd homeledger-api
docker compose up --build
```

Open http://localhost:3000 and log in with `demo@homeledger.local` / `Demo1234!`. The first build takes a few minutes.

This starts PostgreSQL, the API and the web client (built from GitHub), then creates the database and fills it with an invented household. To use another port, set `HOMELEDGER_PORT=3100`. To reset the data, run `docker compose down -v`.

## What the API does

- **Accounts:** registration, login, refresh tokens in HttpOnly cookies, and email support.
- **Personal finance:** income, expenses, categories, budgets, bills and work-day tracking.
- **Family finance:** households with roles, invite links, and shared bills and payments.
- **Open banking:** connects bank accounts through [Basiq](https://basiq.io) to read accounts and transactions.

## Built with

C#, ASP.NET Core 10, Entity Framework Core, PostgreSQL, JWT, BCrypt, Mapperly, and Docker.

## Code layout

| Folder | Purpose |
| --- | --- |
| `Controllers/` | REST endpoints |
| `Services/` | Business logic |
| `Persistence/` | `DbContext`, repositories and unit of work |
| `Models/`, `DTOs/`, `Mappers/` | Database entities, API contracts and mapping |
| `Middleware/` | Correlation IDs and error handling |

The database also enforces key rules itself. For example, a check constraint requires each bill to belong to either a person or a family, never both.

## Develop without Docker

You need the .NET 10 SDK and PostgreSQL.

```bash
cd HomeLedger.API
cp appsettings.example.json appsettings.json   # fill in your own values
dotnet run
```

Run the tests from the repo root with `dotnet test`.

## Known limits

- Test coverage is small, and the integration smoke test needs a `DefaultConnection` value, so it fails on a clean checkout.
- Basiq and email need your own credentials, so they're off in the Docker demo.
- EF Core migrations aren't in this repo. The Docker demo builds the schema directly from the model, so a real deployment needs migrations generated first.
