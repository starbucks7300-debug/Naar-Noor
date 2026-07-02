# 📁 Project Structure

This document outlines the organization of the Naar & Noor project.

## Root Directory

```
Naar-Noor/
├── api-server/          # .NET Backend Application
├── naar-noor/           # Angular Frontend Application
├── docs/                # Documentation
└── .git/                # Git repository
```

## Backend Structure (`api-server/`)

```
api-server/
├── src/
│   ├── NaarNoor.API/                    # API Layer
│   │   ├── Controllers/                 # API Endpoints
│   │   │   ├── ChefsController.cs
│   │   │   ├── MenuController.cs
│   │   │   ├── ReservationsController.cs
│   │   │   ├── ReviewsController.cs
│   │   │   ├── ContactController.cs
│   │   │   └── HealthController.cs
│   │   ├── Program.cs                   # Application Configuration
│   │   ├── appsettings.json             # Configuration
│   │   └── NaarNoor.API.csproj
│   │
│   ├── NaarNoor.Application/            # Business Logic Layer
│   │   ├── Chefs/
│   │   │   └── Queries/
│   │   │       └── GetChefs/
│   │   ├── MenuItems/
│   │   │   └── Queries/
│   │   │       └── GetMenuItems/
│   │   ├── Reservations/
│   │   │   ├── Commands/
│   │   │   │   └── CreateReservation/
│   │   │   └── Queries/
│   │   │       └── GetReservations/
│   │   ├── Reviews/
│   │   │   └── Queries/
│   │   │       └── GetApprovedReviews/
│   │   ├── Contact/
│   │   │   └── Commands/
│   │   │       └── SubmitInquiry/
│   │   ├── Common/
│   │   │   ├── Behaviours/
│   │   │   └── Interfaces/
│   │   └── NaarNoor.Application.csproj
│   │
│   ├── NaarNoor.Domain/                 # Domain Layer
│   │   ├── Entities/
│   │   │   ├── Chef.cs
│   │   │   ├── MenuItem.cs
│   │   │   ├── Reservation.cs
│   │   │   ├── Review.cs
│   │   │   └── ContactInquiry.cs
│   │   ├── Enums/
│   │   │   ├── MenuCategory.cs
│   │   │   └── ReservationStatus.cs
│   │   ├── Common/
│   │   │   └── BaseEntity.cs
│   │   └── NaarNoor.Domain.csproj
│   │
│   └── NaarNoor.Infrastructure/         # Data Access Layer
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── Configurations/          # Entity Configurations
│       │   │   ├── ChefConfiguration.cs
│       │   │   ├── MenuItemConfiguration.cs
│       │   │   ├── ReservationConfiguration.cs
│       │   │   ├── ReviewConfiguration.cs
│       │   │   └── ContactInquiryConfiguration.cs
│       │   ├── Seeds/
│       │   │   └── DatabaseSeeder.cs
│       │   └── Migrations/
│       ├── DependencyInjection.cs
│       └── NaarNoor.Infrastructure.csproj
│
├── NaarNoor.sln                         # Solution File
├── run-dev.sh                           # Development Script
└── .gitignore
```

## Frontend Structure (`naar-noor/`)

```
naar-noor/
├── src/
│   ├── app/
│   │   ├── components/                  # Reusable Components
│   │   │   ├── header/
│   │   │   ├── footer/
│   │   │   ├── animated-background/
│   │   │   ├── custom-calendar/
│   │   │   └── custom-dropdown/
│   │   │
│   │   ├── pages/                       # Page Components
│   │   │   └── home/
│   │   │
│   │   ├── sections/                    # Page Sections
│   │   │   ├── hero/
│   │   │   ├── about/
│   │   │   ├── menu/
│   │   │   ├── chefs/
│   │   │   ├── reservations/
│   │   │   ├── reviews/
│   │   │   ├── locations/
│   │   │   ├── blog/
│   │   │   ├── category/
│   │   │   └── cinematic-banner/
│   │   │
│   │   ├── services/                    # Services
│   │   │   ├── api.service.ts
│   │   │   └── dropdown-manager.service.ts
│   │   │
│   │   ├── app.component.ts
│   │   ├── app.config.ts
│   │   └── app.routes.ts
│   │
│   ├── assets/                          # Static Assets
│   │   ├── blog/
│   │   ├── categories/
│   │   ├── chefs/
│   │   ├── cinematic/
│   │   ├── hero/
│   │   ├── locations/
│   │   └── icons/
│   │
│   ├── data/                            # Data Files
│   │   ├── blog.data.ts
│   │   ├── category.data.ts
│   │   ├── chefs.data.ts
│   │   ├── menu.data.ts
│   │   └── reviews.data.ts
│   │
│   ├── index.html
│   ├── main.ts
│   ├── styles.css
│   └── sitemap.xml
│
├── angular.json                         # Angular Configuration
├── package.json                         # Dependencies
├── tailwind.config.js                   # Tailwind Configuration
├── postcss.config.js                    # PostCSS Configuration
├── tsconfig.json                        # TypeScript Configuration
└── .gitignore
```

## Key Directories Explained

### Backend

| Directory | Purpose |
|-----------|---------|
| `NaarNoor.API` | REST API endpoints and HTTP request handling |
| `NaarNoor.Application` | Business logic, CQRS commands/queries |
| `NaarNoor.Domain` | Core business entities and rules |
| `NaarNoor.Infrastructure` | Database context, migrations, configurations |

### Frontend

| Directory | Purpose |
|-----------|---------|
| `components/` | Reusable UI components |
| `pages/` | Full page components |
| `sections/` | Page sections/features |
| `services/` | API communication and business logic |
| `assets/` | Images, icons, and static files |
| `data/` | Mock data and constants |

## Architecture Pattern

The backend follows **Clean Architecture** with **CQRS** pattern:

```
┌─────────────────────────────────────┐
│         API Layer (Controllers)      │
├─────────────────────────────────────┤
│    Application Layer (Commands/      │
│    Queries, Business Logic)          │
├─────────────────────────────────────┤
│      Domain Layer (Entities,         │
│      Business Rules)                 │
├─────────────────────────────────────┤
│   Infrastructure Layer (Database,    │
│   External Services)                 │
└─────────────────────────────────────┘
```

## File Naming Conventions

### Backend
- **Controllers**: `{Entity}Controller.cs`
- **Commands**: `{Action}{Entity}Command.cs`
- **Queries**: `Get{Entities}Query.cs`
- **Handlers**: `{Command/Query}Handler.cs`
- **Validators**: `{Command/Query}Validator.cs`

### Frontend
- **Components**: `{name}.component.ts`, `{name}.component.html`, `{name}.component.css`
- **Services**: `{name}.service.ts`
- **Data**: `{name}.data.ts`

---

For more details, see the [Architecture](./ARCHITECTURE.md) guide.
