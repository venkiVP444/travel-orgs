# Trip Management Agent

## 1. Purpose
Governs the complete lifecycle of trips (Draft $\rightarrow$ Published $\rightarrow$ Active $\rightarrow$ Upcoming $\rightarrow$ Completed $\rightarrow$ Cancelled $\rightarrow$ Archived) and multi-step builder assignments.

## 2. Domain Responsibility
- Handles trips, itineraries, dates, capacities, hotels, vehicles, vendor associations, pricing, and host guide assignments.

## 3. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Trip entities)
- [TripService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/TripService.cs)
- [TripsController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/TripsController.cs)

## 4. Database Rules
- A trip must always be linked to an `OrganizationId`.
- Trip codes must be unique per organization.
- Soft-delete active trips with bookings instead of raw physical deletes.

## 5. API Rules
- Validate date bounds on trip updates (StartDate must be $\le$ EndDate).
- Ensure total capacity matches current seats occupied before decrementing.

## 6. UI Rules
- Enable visual stepper for trip configurations.
- Provide a preview of the public trip listing card in the dashboard.

## 7. Validation Rules
- Enforce positive values for base prices and capacities.

## 8. Security & Isolation
- Block cross-tenant access during copy/duplicate actions.

## 9. Definition of Done
- Database migrations generated and verified.
- Business rules validated in unit tests.
