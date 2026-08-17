# Matrimonial Admin API

ASP.NET Core 8 Web API for the Matrimonial SaaS admin panel with PostgreSQL database.

## Tech Stack

- **Backend:** ASP.NET Core 8 Web API
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core 8
- **Auth:** JWT Bearer tokens
- **Frontend:** Next.js (separate folder - coming soon)

## Project Structure

```
Matrimonial/
├── backend/
│   ├── Matrimonial.Admin.sln
│   └── Matrimonial.AdminApi/
│       ├── Controllers/       # API endpoints
│       ├── Data/              # DbContext & seeding
│       ├── Entities/          # Database models
│       ├── DTOs/              # Request/Response models
│       ├── Services/          # Business logic
│       ├── Configurations/    # App settings classes
│       └── Common/            # Shared utilities
└── frontend/                  # Next.js admin UI (future)
```

## Database Tables

| Table | Description |
|-------|-------------|
| AdminUsers | Platform admin accounts |
| Tenants | Matrimonial business tenants |
| SubscriptionPlans | Available subscription plans |
| TenantSubscriptions | Tenant plan subscriptions |
| Payments | Payment records |
| EmailTemplates | Email notification templates |
| SystemSettings | Global system configuration |

## Prerequisites

- .NET 8 SDK
- PostgreSQL 14+

## Setup

### 1. Configure Database

Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=MatrimonialAdmin;Username=postgres;Password=YOUR_PASSWORD"
}
```

### 2. Create Database & Run Migrations

```bash
cd backend/Matrimonial.AdminApi
dotnet ef migrations add InitialCreate
dotnet run
```

The app auto-applies migrations and seeds a default admin user on startup.

### 3. Default Admin Credentials

| Field | Value |
|-------|-------|
| Username | `admin` |
| Password | `Admin@123` |

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Admin login (returns JWT) |

### Admin Users
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/adminusers` | List admin users (paginated) |
| GET | `/api/adminusers/{id}` | Get admin user by ID |
| POST | `/api/adminusers` | Create admin user |
| PUT | `/api/adminusers/{id}` | Update admin user |
| DELETE | `/api/adminusers/{id}` | Delete admin user |

### Tenants
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tenants` | List tenants |
| GET | `/api/tenants/{id}` | Get tenant by ID |
| POST | `/api/tenants` | Create tenant |
| PUT | `/api/tenants/{id}` | Update tenant |
| DELETE | `/api/tenants/{id}` | Delete tenant |

### Subscription Plans
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/subscriptionplans` | List plans |
| GET | `/api/subscriptionplans/{id}` | Get plan by ID |
| POST | `/api/subscriptionplans` | Create plan |
| PUT | `/api/subscriptionplans/{id}` | Update plan |
| DELETE | `/api/subscriptionplans/{id}` | Delete plan |

### Tenant Subscriptions
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tenantsubscriptions` | List subscriptions |
| GET | `/api/tenantsubscriptions/{id}` | Get subscription |
| POST | `/api/tenantsubscriptions` | Create subscription |
| PUT | `/api/tenantsubscriptions/{id}` | Update subscription |
| DELETE | `/api/tenantsubscriptions/{id}` | Delete subscription |

### Payments
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/payments` | List payments |
| GET | `/api/payments/{id}` | Get payment |
| POST | `/api/payments` | Create payment |
| PUT | `/api/payments/{id}` | Update payment |
| DELETE | `/api/payments/{id}` | Delete payment |

### Email Templates
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/emailtemplates` | List templates |
| GET | `/api/emailtemplates/{id}` | Get template |
| GET | `/api/emailtemplates/by-name/{name}` | Get by name |
| POST | `/api/emailtemplates` | Create template |
| PUT | `/api/emailtemplates/{id}` | Update template |
| DELETE | `/api/emailtemplates/{id}` | Delete template |

### System Settings
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/systemsettings` | List settings |
| GET | `/api/systemsettings/{id}` | Get setting |
| GET | `/api/systemsettings/by-key/{key}` | Get by key |
| POST | `/api/systemsettings` | Create setting |
| PUT | `/api/systemsettings/{id}` | Update setting |
| DELETE | `/api/systemsettings/{id}` | Delete setting |

> All endpoints except `/api/auth/login` require JWT Bearer token in the `Authorization` header.

## Swagger UI

When running in Development mode, access Swagger at:
```
https://localhost:7xxx/swagger
```

## Response Format

All API responses follow this structure:

```json
{
  "success": true,
  "message": "Success",
  "data": { }
}
```

Paginated lists return:

```json
{
  "success": true,
  "message": "Success",
  "data": {
    "items": [],
    "totalCount": 0,
    "page": 1,
    "pageSize": 10,
    "totalPages": 0
  }
}
```
