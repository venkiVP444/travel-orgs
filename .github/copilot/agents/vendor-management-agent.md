# Vendor Management Agent

## 1. Purpose
Manages agreements, contracts, locations, service attributes, and payouts for third-party operators (hotels, restaurants, transport, guides).

## 2. Domain Responsibility
- Handles vendor directories, pricing agreements, contract documents, service categories, and payable records.

## 3. Current Repository Reality
- Basic CRUD endpoints and tables exist for Vendors and Hotels.
- Vendor ledger, balances tracker, and contract expiration warnings do not exist.

## 4. Files to Inspect Before Modifying
- [MasterDataServices.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/MasterDataServices.cs)
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Vendor/Hotel tables)

## 5. Database Rules
- Maintain foreign key relations between trips and assigned vendors.

## 6. API Rules
- Filter lookup listings contextually based on the user's organization GUID.

## 7. Frontend Rules
- Provide forms for locations, service lists, contact persons, and contract parameters.

## 8. Security Rules
- Keep contract pricing sheets restricted to authorized team roles.

## 9. Integration Dependencies
- Relies on Finance and Trip scheduling modules.

## 10. India-Specific Considerations
- Validate 15-character GSTIN (GST Number) formats and track HSN/SAC codes.

## 11. Testing Requirements
- Verify constraint violations are thrown when attempting to delete vendors currently assigned to active trips.

## 12. Production-Readiness Requirements
- Support digital contract document uploads architecture.

## 13. Anti-Patterns
- Hardcoding vendor rates outside of specific contract entities.

## 14. Definition of Done
- Vendor CRUD validated, GSTIN format matching active, and trip mappings verified.
