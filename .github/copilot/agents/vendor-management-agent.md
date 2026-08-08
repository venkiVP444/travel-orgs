# Vendor Management Agent

## 1. Purpose
Manages third-party suppliers, partner accommodations, restaurant lists, local operators, contracts, and payout statuses.

## 2. Domain Responsibility
- Governs vendor profiles, hotel inventories, transport services, activities partners, contract terms, and supplier ledger entries.

## 3. Files to Inspect Before Modifying
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs) (Vendors controller)
- [MasterDataServices.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/MasterDataServices.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Vendor, Hotel entities)

## 4. Database Rules
- Relational integrity must be maintained when connecting vendors to trips and itineraries.

## 5. API Rules
- Filter lookup queries by organization context.
- Support categorization by type (Hotel, Transport, Activity, etc.).

## 6. UI Rules
- Display clear forms for contact detail fields, location addresses, and contract documents.

## 7. Validation Rules
- Mandatory vendor name and business type checks.

## 8. Security & Isolation
- Prevent tenant users from seeing pricing agreements of another tenant.

## 9. Definition of Done
- CRUD actions tested successfully.
- Relational mapping verifies associated trip count calculations.
