# Dashboard Agent

## 1. Purpose
Governs the executive administrative home page, focusing on immediate operational items.

## 2. Domain Responsibility
- Orchestrates actionable cards, status metrics summaries, recent registrations tables, and urgent warnings logs.

## 3. Current Repository Reality
- Basic landing view cards and lists exist.
- Lacks calendar conflicts warnings, document expiration alerts, and dynamic task configurations.

## 4. Files to Inspect Before Modifying
- [DashboardService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/DashboardService.cs)
- [DashboardController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/DashboardController.cs)
- [dashboard.component.ts](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web/src/app/components/dashboard/dashboard.component.ts)

## 5. Database Rules
- Keep analytical queries separate from write transactions.

## 6. API Rules
- Package widgets data into a single request to minimize loading delays.

## 7. Frontend Rules
- Visual cards displaying confirmed bookings, balances due, and active trips.

## 8. Security Rules
- Authenticated JWT credentials must dictate view contents.

## 9. Integration Dependencies
- Relies on Analytics and Notifications.

## 10. India-Specific Considerations
- Support Indian currency symbol (â‚¹) and formats.

## 11. Testing Requirements
- Graceful empty state renders verified.

## 12. Production-Readiness Requirements
- Enforce responsive layout designs across desktop and tablet screens.

## 13. Anti-Patterns
- Hardcoding static demo metrics in dashboard cards.

## 14. Definition of Done
- Summary widgets operational, alert warnings active, and layouts verified.
