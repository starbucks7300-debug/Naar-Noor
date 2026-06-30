# Naar & Noor — Restaurant Management App

A full-stack restaurant platform with an Angular frontend and ASP.NET Core backend.

## Stack

- **Frontend**: Angular 18, Tailwind CSS — served on port 5000
- **Backend**: ASP.NET Core 8 (Clean Architecture) — served on port 8080
- **Database**: Replit PostgreSQL (via Npgsql)

## How to run

Two workflows are configured:

| Workflow | Command | Port |
|---|---|---|
| Start application | `cd naar-noor && node node_modules/@angular/cli/bin/ng.js serve --port 5000 --host 0.0.0.0` | 5000 |
| Backend API | `cd api-server && dotnet run --project src/NaarNoor.API` | 8080 |

The Angular dev server proxies `/api/*` requests to `localhost:8080` via `naar-noor/proxy.conf.json`.

## Project layout

```
naar-noor/        Angular frontend
api-server/       ASP.NET Core backend (Clean Architecture)
  src/
    NaarNoor.API/           Controllers, startup
    NaarNoor.Application/   Use cases, DTOs
    NaarNoor.Domain/        Entities, value objects
    NaarNoor.Infrastructure/ EF Core, repositories
docs/             Architecture and development docs
```

## Environment notes

- The backend reads its DB connection string from `ConnectionStrings__DefaultConnection` (env var) or `appsettings.Development.json`.
- Stripe and Supabase credentials are read from environment variables (`Stripe__SecretKey`, `Supabase__Url`, `Supabase__ServiceKey`).
- See `docs/GETTING_STARTED.md` for full setup details.

## User preferences

<!-- Add any project-specific preferences here -->
