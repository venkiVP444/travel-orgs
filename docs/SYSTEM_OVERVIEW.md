# TravelOrgOS - System Overview

Welcome to the **TravelOrgOS** (Travel Organization Operating System) developer documentation. This document provides a high-level walkthrough of the system architecture, directory structures, database design, API design, frontend layout, and deployment/run guides.

---

## 1. Directory Structure

The project is structured as a multi-project .NET solution paired with an Angular 17 Single Page Application (SPA).

- **`database/`**: Contains SQL migration scripts and seed schema definitions under [database/scripts/](file:///c:/personal/TravelOrgOS/database/scripts).
- **`docs/`**: Technical specs, guidebooks, and roadmap notes.
  - [ARCHITECTURE.md](file:///c:/personal/TravelOrgOS/docs/ARCHITECTURE.md) - Architecture pattern overview.
  - [API.md](file:///c:/personal/TravelOrgOS/docs/API.md) - Full REST API specifications.
  - [DATABASE.md](file:///c:/personal/TravelOrgOS/docs/DATABASE.md) - Database configuration details.
  - [DEMO-GUIDE.md](file:///c:/personal/TravelOrgOS/docs/DEMO-GUIDE.md) - Scripted sales flow instructions.
  - [ROADMAP.md](file:///c:/personal/TravelOrgOS/docs/ROADMAP.md) - Current phases & upcoming milestones.
- **`scripts/`**: PowerShell scripts for local development setup and execution.
  - [setup.ps1](file:///c:/personal/TravelOrgOS/scripts/setup.ps1) - Configures the LocalDB database and installs frontend npm packages.
  - [run-api.ps1](file:///c:/personal/TravelOrgOS/scripts/run-api.ps1) - Launches the ASP.NET Core API server on port `5100`.
  - [run-web.ps1](file:///c:/personal/TravelOrgOS/scripts/run-web.ps1) - Starts the Angular development server on port `4400`.
  - [reset-demo-data.ps1](file:///c:/personal/TravelOrgOS/scripts/reset-demo-data.ps1) - Restores demo records back to baseline.
- **`src/`**: Project source code.
  - [TravelOrgOS.Domain](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain) - Entities, enums, value objects, and domain transfer objects (DTOs).
  - [TravelOrgOS.Infrastructure](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure) - Data contexts, repositories, services, and external integrations (Stripe, Razorpay, Mock).
  - [TravelOrgOS.Api](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api) - Web API controller layer, authentication handlers, and middleware routing.
  - [TravelOrgOS.Web](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web) - Frontend Angular client codebase.
- **`tests/`**: Automated verification test suites.
  - [TravelOrgOS.Api.Tests](file:///c:/personal/TravelOrgOS/tests/TravelOrgOS.Api.Tests) - xUnit unit and integration tests.

---

## 2. Architecture Layers (.NET Clean Architecture)

```
                       +---------------------------------------+
                       |           TravelOrgOS.Web             |
                       |       (Angular 17 Standalone)         |
                       +-------------------+-------------------+
                                           | HTTP REST
                                           v
                       +---------------------------------------+
                       |           TravelOrgOS.Api             |
                       |        (ASP.NET Core Web API)         |
                       +-------------------+-------------------+
                                           | DI / Ref
                                           v
                       +---------------------------------------+
                       |        TravelOrgOS.Infrastructure     |
                       |      (EF Core / DB Context / Serv)    |
                       +-------------------+-------------------+
                                           | DI / Ref
                                           v
                       +---------------------------------------+
                       |          TravelOrgOS.Domain           |
                       |        (Core Entities & Enums)        |
                       +-------------------+-------------------+
                                           | SQL Protocol
                                           v
                       +---------------------------------------+
                       |          SQL Server LocalDB           |
                       |             (Local Only)              |
                       +---------------------------------------+
```

### 2.1 Domain Layer ([TravelOrgOS.Domain](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain))
- Independent of database implementations or frameworks.
- Contains the core model representations:
  - [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs): Defines structures for `Organization`, `OrganizationUser`, `Traveller`, `Trip`, `Hotel`, `Vehicle`, `Vendor`, `Booking`, `Payment`, etc.
  - [DomainEnums.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Enums/DomainEnums.cs): Defines enums like `UserRole`, `TripStatus`, `BookingStatus`, `PaymentStatus`, etc.
  - [Dtos.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/DTOs/Dtos.cs): Request and response data structures for API interactions.

### 2.2 Infrastructure Layer ([TravelOrgOS.Infrastructure](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure))
- Implements data storage, persistence, and external service contracts.
- **EF Core Database Context**: [TravelOrgOSDbContext.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Data/TravelOrgOSDbContext.cs) models table indexes, unique constraints, and foreign key relations.
- **Seed Data Management**: [DbInitializer.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Data/DbInitializer.cs) defines realistic initial seeding, creating mock organizations (e.g., Demo Travel), users, trips, vehicles, and booking histories.
- **Application Services**: Core business logic orchestration occurs here:
  - [AuthService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/AuthService.cs) handles credentials check and JWT generation.
  - [BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs) handles seat deductions, overbooking prevention, and payments auditing.
  - [TripService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/TripService.cs) handles the 10-step Trip Builder logic and publication parameters.
  - [Payment Gateway Integration](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/Payment/): Extensible architecture supports Mock payments, Stripe, and Razorpay.

### 2.3 API Layer ([TravelOrgOS.Api](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api))
- REST Controller endpoints that communicate with the frontend using JSON.
- Main entry point is [Program.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Program.cs).
- Enforces security via JWT Authentication policies.
- Includes a dedicated [WebhooksController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/WebhooksController.cs) for async external notifications from payment providers.

### 2.4 Web Frontend ([TravelOrgOS.Web](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web))
- Angular 17 standalone SPA architecture.
- Enforces user session management via interceptors, guarding administrative dashboards and letting clients access customized traveller portals.
- Key modules in [components/](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web/src/app/components):
  - **`dashboard/`**: Admin portal tracking real-time KPIs (Revenue, Balances, Bookings).
  - **`trips/`**: Multi-step trip configuration workspace.
  - **`portal/`**: Custom mobile-first booking front-ends branded to specific tenant domains.
  - **`travellers/`**: Profiles lists and batch CSV importer.

---

## 3. Database Safety Guarantee

> [!IMPORTANT]
> To prevent accidental connections to office databases or restoring raw test scripts onto company databases, the system runs programmatic safety checks.

The [DatabaseSafetyChecker.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Data/DatabaseSafetyChecker.cs) intercepts connection strings in [Program.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Program.cs) and throws an exception if:
1. The server is not exactly `(localdb)\MSSQLLocalDB`.
2. The database is not exactly `TravelOrgOS_Dev`.
3. The string contains the forbidden office IP (`10.50.6.6`) or restore DB name (`dbEMMA_Restore`).

---

## 4. Database Schema Overview (Multi-Tenant B2B SaaS)

TravelOrgOS uses an **OrganizationId-based** database isolation model. Most tables contain a foreign key reference to the [Organization](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs#L7) entity to segment tenant spaces:

- **Identity**: `Organizations`, `OrganizationUsers`
- **Profiles**: `Travellers`, `BookingTravellers`
- **Experiences**: `Trips`, `TripItineraryDays`, `TripMeals`
- **Accommodations & Logistics**: `Hotels`, `TripHotels`, `Vehicles`, `TripVehicles`
- **Partners**: `Vendors`, `TripVendors`
- **Ledger**: `Bookings`, `Payments`
- **Audits & Comms**: `Notifications`, `AuditLogs`

---

## 5. Sales Demo Walkthrough

A standard presentation script is pre-configured in [DEMO-GUIDE.md](file:///c:/personal/TravelOrgOS/docs/DEMO-GUIDE.md):

1. **Dashboard Overview**: Check active SaaS statistics after signing in as `owner@demo-travel.com`.
2. **CSV Import**: Test bulk traveller CSV uploading with syntax/data validation reports.
3. **Trip Builder**: Formulate a new travel experience utilizing the 10-step wizard.
4. **Traveller Portal Booking**: Open the public `/portal/demo-travel` route, pick a package, and mock a 30% deposit booking.
5. **Real-time Synchronization**: Observe available seat updates, payment ledger records, and new audit logs.

---

## 6. How to Run Locally

1. **Prerequisites**: Ensure you have SQL Server LocalDB, .NET 8 SDK, and Node.js installed.
2. **Step 1: Setup Workspace**
   ```powershell
   .\scripts\setup.ps1
   ```
3. **Step 2: Launch Backend API**
   ```powershell
   .\scripts\run-api.ps1
   ```
   The backend resides at `http://localhost:5100`.
4. **Step 3: Launch Frontend**
   ```powershell
   .\scripts\run-web.ps1
   ```
   The application will serve on `http://localhost:4400`.
5. **Step 4: Verify Tests**
   ```powershell
   dotnet test tests/TravelOrgOS.Api.Tests/TravelOrgOS.Api.Tests.csproj
   ```
