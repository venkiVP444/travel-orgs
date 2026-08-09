# Vehicle Fleet Agent

## 1. Purpose
Manages organization transport assets, vehicle specifications, scheduling, driver allocations, and maintenance.

## 2. Domain Responsibility
- Handles vehicle records, capacities, registrations, driver names, insurance details, and scheduling logs.

## 3. Current Repository Reality
- Basic CRUD operations exist for Vehicles.
- Scheduling calendar, date overlap blockades, and document expiration alerts do not exist.

## 4. Files to Inspect Before Modifying
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs) (Vehicles)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Vehicle)

## 5. Database Rules
- Registration plate strings must be unique within an organization boundary.

## 6. API Rules
- Validate date overlaps when assigning vehicles to trips.

## 7. Frontend Rules
- Highlight alert states for vehicles requiring maintenance or having expired documents.

## 8. Security Rules
- Fleet metrics and tracking details must remain strictly isolated per tenant.

## 9. Integration Dependencies
- Relies on Trip Management.

## 10. India-Specific Considerations
- Verify state road permits, National Permit (NP) tags, and local pollution checks (PUC).

## 11. Testing Requirements
- Unit tests validating double-booking prevention of the same vehicle on concurrent dates.

## 12. Production-Readiness Requirements
- Expiring document check schedules triggering in-app alerts.

## 13. Anti-Patterns
- Bypassing scheduling validation checks, resulting in fleet double-bookings.

## 14. Definition of Done
- Date scheduling conflict check logic verified and plates validations integrated.
