# Trip Management Agent

## 1. Purpose
Governs the complete trip lifecycle from draft configuration through pricing, publication, execution, completion, and archiving.

## 2. Domain Responsibility
- Tracks trip profiles, daily itineraries, hotel blocks, transit vehicle mappings, vendor agreements, and passenger capacity.

## 3. Current Repository Reality
- Trip CRUD, basic itinerary days, and stepper configurations exist.
- Hardcoded guide inputs are tracked as a plain text string HostGuide in the Trip table.
- Lacks a formal scheduling/double-booking protection for transit vehicles or guides.

## 4. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Trip-related tables)
- [TripService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/TripService.cs)
- [TripsController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/TripsController.cs)
- [trip-builder.component.ts](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web/src/app/components/trips/trip-builder.component.ts)

## 5. Database Rules
- Trips must partition by OrganizationId.
- Trip code values must be unique within organization contexts.
- Soft-delete trips when bookings have been confirmed.

## 6. API Rules
- Verify StartDate <= EndDate on creation/updates.
- Restrict status transitions strictly along the lifecycle sequence (Draft -> Published -> Completed).

## 7. Frontend Rules
- STEPS 1-10 stepper forms must enforce validity before allowing next steps or publishing.

## 8. Security Rules
- Block copying or duplicating trip templates across organization boundaries.

## 9. Integration Dependencies
- Relies on Vendor, Vehicle, Guide, and Pricing modules.

## 10. India-Specific Considerations
- Handle localized Indian destination routing descriptions (State/City tags).

## 11. Testing Requirements
- Verify validation limits on dates, seat availability calculations, and price formats.

## 12. Production-Readiness Requirements
- Graceful error returns on booking validation failures.

## 13. Anti-Patterns
- Modifying capacity below currently reserved seat counts.
- Storing guides as unstructured free-form text strings in the trip model.

## 14. Definition of Done
- Wizard workflows complete, db schemas generated, and date boundaries verified.
