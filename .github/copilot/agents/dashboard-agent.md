# Dashboard Agent

## 1. Purpose
Governs the executive landing views, operational summary blocks, and attention markers.

## 2. Domain Responsibility
- Handles quick summaries, booking statuses, recent payment logs, and alerts.

## 3. Files to Inspect Before Modifying
- [DashboardService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/DashboardService.cs)
- [DashboardController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/DashboardController.cs)

## 4. Database Rules
- Keep analytical calculations performant and separate from master data writes.

## 5. API Rules
- Filter statistics strictly by the user's organization scope.

## 6. UI Rules
- Enable visual dashboard summaries for confirmed bookings, outstanding balances, and active trips.

## 7. Validation Rules
- Gracefully handle empty states without UI crashes.

## 8. Security & Isolation
- Ensure only authorized roles can view summary totals.

## 9. Definition of Done
- Metrics render correctly upon dashboard loading.
