# TravelOrgOS - Travel Organization Operating System

TravelOrgOS is a multi-tenant B2B SaaS platform for travel agencies, tour operators, group tour companies, and travel management organizations. It brings trips, itineraries, travellers, bookings, payments, and fleet operations into one platform while giving travellers a mobile-friendly booking experience branded under each organization's identity.

---

## CRITICAL DATABASE SAFETY GUARANTEE

> [!CAUTION]
> **LOCALDB SAFETY ENFORCEMENT**:
> - TravelOrgOS **NEVER** connects to office server `10.50.6.6` or database `dbEMMA_Restore`.
> - TravelOrgOS exclusively targets the developer's local SQL Server LocalDB instance `(localdb)\MSSQLLocalDB` and target database `TravelOrgOS_Dev`.
> - Programmatic assertion `DatabaseSafetyChecker.AssertConnectionIsLocalDbOnly()` executes prior to any database operation.

---

## SSMS Connection Information

To inspect TravelOrgOS tables in SQL Server Management Studio (SSMS):

- **Server Type**: Database Engine
- **Server Name**: `(localdb)\MSSQLLocalDB`
- **Authentication**: Windows Authentication
- **Database**: `TravelOrgOS_Dev`

**Full SSMS Connection String**:
```text
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TravelOrgOS_Dev;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;Application Name="SQL Server Management Studio";Command Timeout=0;
```

---

## Demo Credentials

All demo accounts use the development password: **`Demo@123`**

| Role | Email | Access Scope |
| :--- | :--- | :--- |
| **Platform Admin** | `admin@travelorgos.com` | Full platform management |
| **Organization Owner** | `owner@demo-travel.com` | Full organization & financial access |
| **Organization Admin** | `manager@demo-travel.com` | Trips, Travellers & Bookings management |
| **Finance User** | `finance@demo-travel.com` | Revenue, Balances & Payment ledger |
| **Traveller** | `traveller@demo-travel.com` | Mobile Portal & My Bookings |

---

## Quick Start & Running Locally

### 1. Execute Environment Setup Script
Open PowerShell and run:
```powershell
.\scripts\setup.ps1
```

### 2. Start Backend API
```powershell
.\scripts\run-api.ps1
```
The API server will run on: `http://localhost:5100`

### 3. Start Angular Web Frontend
```powershell
.\scripts\run-web.ps1
```
The Angular SPA will run on: `http://localhost:4400`

---

## Primary Sales Demo Flow

1. **Login**: Go to `http://localhost:4400` and click **Org Owner** (`owner@demo-travel.com`).
2. **Dashboard**: Inspect live SaaS KPIs (Active Trips, Travellers, Confirmed Bookings, Revenue, Outstanding Balances).
3. **Traveller Directory & CSV Import**: View traveller profiles and test batch CSV import with live validation.
4. **Trip Builder**: Launch the 10-step Trip Builder to configure basic info, dates, itinerary days, hotels, vehicles, meals, vendors, and pricing.
5. **Publish**: Click **Publish to Traveller Portal Now**.
6. **Mobile Traveller Portal**: Open `/portal/demo-travel` to view branded portal home and trip catalog.
7. **Book & Mock Payment**: View trip details, select 2 passengers, select **Pay 30% Deposit Now**, and submit.
8. **Confirmation & Real-Time Metrics**: View booking confirmation screen `BK-KER-XXXX`. Return to Admin to see booking recorded, seats deducted, and notification generated!

---

## Automated Test Verification

Run backend xUnit tests:
```bash
dotnet test tests/TravelOrgOS.Api.Tests/TravelOrgOS.Api.Tests.csproj
```
Tests verify:
- LocalDB safety connection checks (rejecting `10.50.6.6` / `dbEMMA_Restore`)
- Seat reservation & overbooking prevention
- Payment balance calculation
