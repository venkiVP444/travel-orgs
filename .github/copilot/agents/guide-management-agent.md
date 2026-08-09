# Guide Management Agent

## 1. Purpose
Orchestrates tour guide profiles, certification details, language proficiencies, scheduling, and calendar conflicts.

## 2. Domain Responsibility
- Handles guide directory profiles, language competencies, daily fee rates, calendars, and trip assignments.

## 3. Current Repository Reality
- Entirely missing from domain entities and UI. Guide details are currently represented by a single text field HostGuide on the Trip table.

## 4. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Create Guide entity)
- [TripService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/TripService.cs)
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs)

## 5. Database Rules
- Add a new Guide table with relationships to OrganizationId and TripGuides.

## 6. API Rules
- Block guide allocation to trips that contain date overlaps (Prevent double-bookings).

## 7. Frontend Rules
- Build a Guide Management page and integrate guide selection into the Trip Builder wizard.

## 8. Security Rules
- Guide contact details and fee agreements must not leak to other organizations.

## 9. Integration Dependencies
- Relies on Trip Management and Finance.

## 10. India-Specific Considerations
- Track localized language certifications (e.g., Kannada, Tamil, Telugu, Hindi, Malayalam) and local state tourism badges.

## 11. Testing Requirements
- Integration tests checking guide booking overlaps across multiple trips.

## 12. Production-Readiness Requirements
- Handle guide profile statuses (Active, Inactive, OnLeave).

## 13. Anti-Patterns
- Saving guide details as simple text strings in trip logs.

## 14. Definition of Done
- Guide database schema defined, conflict prevention active, and profile CRUD functional.
