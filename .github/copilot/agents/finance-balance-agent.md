# Finance & Balance Agent

## 1. Purpose
Tracks organization-level financials, margins, costs, and customer payables.

## 2. Domain Responsibility
- Handles overall revenue stats, outstanding balances, vendor cost estimates, and profit logs.

## 3. Files to Inspect Before Modifying
- [DashboardService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/DashboardService.cs)
- [BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs)

## 4. Database Rules
- Financial metrics must calculate sums from database transactions (no hardcoded totals).

## 5. API Rules
- Filter aggregate sums contextually by organization scope.

## 6. UI Rules
- Provide clear summaries for net margins, total payables, and outstanding balances.

## 7. Validation Rules
- Prevent negative calculations on gross profit displays.

## 8. Security & Isolation
- Ensure financial details are only viewable by authorized roles (Owner, Finance, Admin).

## 9. Definition of Done
- Financial widgets pull data dynamically from transaction tables.
