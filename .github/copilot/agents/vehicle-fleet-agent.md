# Vehicle Fleet Agent

## 1. Purpose
Manages transport assets, bus configurations, driver log pairings, maintenance windows, and availability scheduling.

## 2. Domain Responsibility
- Handles vehicle profiles, capacities, registrations, driver mappings, and trip allocation logs.

## 3. Files to Inspect Before Modifying
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs) (Vehicles controller)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Vehicle entity)

## 4. Database Rules
- Registration plate strings must be unique within tenant scopes.

## 5. API Rules
- Block vehicle assignment to overlapping trip dates (Double-booking prevention).
- Check insurance and permit dates for validity.

## 6. UI Rules
- Provide clear visual indicators for vehicles requiring maintenance or having expired permits.

## 7. Validation Rules
- Capacities must be positive integers.

## 8. Security & Isolation
- Do not expose vehicle tracking across tenant organizations.

## 9. Definition of Done
- Database model logic restricts simultaneous usage.
- Warning alerts show up on expiring documents.
