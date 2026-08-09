# TravelOrgOS - Travel Organization Operating System

TravelOrgOS is an enterprise-grade, multi-tenant B2B SaaS platform for travel agencies, tour operators, group tour companies, and travel management organizations. It manages trips, itineraries, travellers CRM, bookings ledger, payment checkouts, and fleet operations under a single panel while giving customers a mobile-friendly booking experience branded under each organization's identity.

For a comprehensive high-level guide to the system, see the [Project Overview](file:///c:/personal/TravelOrgOS/docs/PROJECT_OVERVIEW.md) or the detailed [System Overview](file:///c:/personal/TravelOrgOS/docs/SYSTEM_OVERVIEW.md).

---

## 1. Product Overview & Core Lifecycle

TravelOrgOS manages the complete travel operational lifecycle:
- **Organization & Branding**: Setup domain slug, colors, logo, and portal greetings.
- **Role-Based Access Control**: Enforce separation between Owner, Admin, Trip Coordinator, and Finance User roles.
- **Trip Stepper Builder**: Construct 11-step trips detailing itinerary days, hotels, vehicles, tour guides, meals, vendors, and deposit pricing.
- **India GST Support**: Calculate CGST, SGST, and IGST splits based on operator and customer billing states automatically.
- **Branded Portal**: Mobilized public trip cards list letting travellers checkout with automatic seat deductions.
- **Payment Gateway Integrity**: Fully integrated webhooks verifying Razorpay & Stripe signatures securely.
- **SaaS Entitlement Engine**: Intercept creations that exceed active subscription quotas (limits on team members, active trips, and monthly bookings).

---

## 2. Platform Architecture

The system is constructed using clean, decoupled architectural patterns:
- **`TravelOrgOS.Domain`**: Core domain entities, enums, and request/response DTO contracts. No external infrastructure dependencies.
- **`TravelOrgOS.Infrastructure`**: EF Core context mapping, service implementations (`TaxService`, `GuideService`, `TeamService`, `SubscriptionService`), and payment gateway factory providers.
- **`TravelOrgOS.Api`**: Controllers, routing rules, capability filters (`RequiresPermission`), and `EntitlementMiddleware` pipeline interceptors.
- **`TravelOrgOS.Web`**: Standalone Angular client featuring side drawer panels, route guards, and responsive layouts.

---

## 3. Prerequisites & Requirements

- **Runtime Target**: .NET 10.0 SDK
- **SQL Server Database Engine**: LocalDB instance `(localdb)\MSSQLLocalDB`
- **Frontend Compiler**: Node.js 18+ and NPM 10+
- **Command Line Helpers**: Entity Framework Core CLI (`dotnet-ef`)

---

## 4. Setup & Running Locally

### Step 1: Initialize Database & Dependencies
Run the initial safety script to verify LocalDB and install frontend packages:
```powershell
.\scripts\setup.ps1
```

### Step 2: Apply Migrations & Start API Server
Launch the backend. On startup, the API server automatically applies EF migrations to recreate `TravelOrgOS_Dev` and populate seed data:
```powershell
.\scripts\run-api.ps1
```
The API listens on: `http://localhost:5100`

### Step 3: Run Web Client
Compile and serve the Angular UI app:
```powershell
.\scripts\run-web.ps1
```
The client SPA serves on: `http://localhost:4400`

---

## 5. SSMS Database Connection Info

To inspect tables inside SQL Server Management Studio (SSMS):
- **Server Type**: Database Engine
- **Server Name**: `(localdb)\MSSQLLocalDB`
- **Authentication**: Windows Authentication
- **Database**: `TravelOrgOS_Dev`

**SSMS Connection String**:
```text
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TravelOrgOS_Dev;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;
```

---

## 6. Secrets & Production Configuration

> [warning]
> Do NOT check secrets, certificates, or database access passwords into source control.

Configuration overrides are resolved from:
- `appsettings.json` / `appsettings.Development.json` for developer fallbacks.
- `appsettings.local.json` (git-ignored) for local environment overrides.
- **Environment Variables** in production mode:
  - `ConnectionStrings__DefaultConnection` (Database endpoint)
  - `Jwt__Secret` (HMAC256 signing secret key)
  - `PaymentGateway__Stripe__WebhookSecret` (Stripe signature verification secret)
  - `PaymentGateway__Razorpay__KeySecret` (Razorpay signature verification secret)

---

## 7. Demo Credentials & Access Roles

All seeded demo credentials use the password: **`Demo@123`**

| Role | Email | Capabilities & Permissions |
| :--- | :--- | :--- |
| **Platform Admin** | `admin@travelorgos.com` | Global metrics, global resets |
| **Organization Owner** | `owner@demo-travel.com` | Full admin, team member management, subscription adjustments |
| **Organization Admin** | `manager@demo-travel.com` | Manage trips, configure pricing, manage travellers CRM |
| **Finance User** | `finance@demo-travel.com` | Manage payments ledger, refunds, view GST tax splits |
| **Trip Coordinator** | `coordinator@demo-travel.com` | Manage vehicles, meals, and guides scheduling assignments |
| **Traveller** | `traveller@demo-travel.com` | Mobile checkout portal access |

---

## 8. Development Simulation Mode

When running in development environments, external payments (Stripe/Razorpay) execute in **simulation mode**:
- Direct checkout URLs return mocks.
- Testing webhook dispatches to `api/webhooks/stripe` and `api/webhooks/razorpay` require passing valid mock signatures or setting debug headers to simulate payment completions.

---

## 9. Automated Testing Verification

Execute backend unit and integration tests verifying concurrency locks, overlapping calendar rejections, GST state calculations, and subscription entitlement filters:
```bash
dotnet test
```

Execute frontend typescript compilation build checks:
```bash
npx ng build
```

---

## 10. Production Deployment

To containerize and deploy to cloud hosting providers:
1. Compile the Angular static output: `npm run build --prod` and serve using an NGINX container.
2. Package the ASP.NET Core API using the Dockerfile (targeting `mcr.microsoft.com/dotnet/aspnet:10.0`).
3. Ensure secrets are injected via secure environment variables rather than hardcoded configs.
