# TravelOrgOS System Architecture

## Architecture Pattern
TravelOrgOS follows clean architecture principles with a multi-tenant B2B SaaS design.

```
+-------------------------------------------------------------+
|                     TravelOrgOS.Web                         |
|   (Angular 17 Standalone SPA + Mobile-First Portal)         |
+------------------------------+------------------------------+
                               | REST APIs (HTTP / JSON)
+------------------------------v------------------------------+
|                     TravelOrgOS.Api                         |
|   (ASP.NET Core Web API, JWT Auth, Controllers, DTOs)        |
+------------------------------+------------------------------+
                               | EF Core Services
+------------------------------v------------------------------+
|                TravelOrgOS.Infrastructure                   |
|   (DbContext, Safety Checker, Repositories, Seed Data)       |
+------------------------------+------------------------------+
                               | EF Core Domain Entities
+------------------------------v------------------------------+
|                    TravelOrgOS.Domain                       |
|   (Entities, Enums, Value Objects, Domain DTOs)             |
+-------------------------------------------------------------+
                               | SQL Server Protocol
+------------------------------v------------------------------+
|              SQL Server LocalDB Instance                    |
|       (localdb)\MSSQLLocalDB -> TravelOrgOS_Dev             |
+-------------------------------------------------------------+
```

## Multi-Tenant Isolation
- Tenant isolation is enforced via `OrganizationId` column attached to all organization-owned entities (`Trips`, `Travellers`, `Bookings`, `Hotels`, `Vehicles`, `Vendors`, `Payments`, `Notifications`, `AuditLogs`).
- Authenticated JWT tokens carry the `OrganizationId` claim, ensuring users cannot access or mutate cross-organization data.

## Mobile-First Branded Traveller Portal
- Dynamic route `/portal/{organizationSlug}` renders custom organization branding (Logo, Primary Color, Secondary Color, Welcome Message).
- Frictionless passenger booking flow with instant Mock Payment processing and seat reservation.
