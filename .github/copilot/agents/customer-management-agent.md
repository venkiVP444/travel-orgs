# Customer/Traveller Management Agent

## 1. Purpose
Governs the traveller directory, customer relations management (CRM), passenger histories, preferences, and details imports.

## 2. Domain Responsibility
- Handles traveller profiles, emergency details, preferences, passport/identity records, and batch CSV imports.

## 3. Current Repository Reality
- Traveller CRM directory, details updates, and batch CSV import with validation exist.
- Lacks a 360-degree timeline view (linking communications, bookings, and payments) and duplicate profiles checks.

## 4. Files to Inspect Before Modifying
- [TravellerService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/TravellerService.cs)
- [TravellersController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/TravellersController.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Traveller table)

## 5. Database Rules
- Customer emails must be unique within an organization scope.

## 6. API Rules
- Validate email syntax and phone structures before persistence.
- Return structured validation report tables on batch CSV importing.

## 7. Frontend Rules
- Provide clear layout profiles highlighting histories, preferences, and profile tags.

## 8. Security Rules
- Encrypt sensitive PII (Passport, Aadhaar details) and restrict data access within tenant boundaries.

## 9. Integration Dependencies
- Relies on Booking and Marketing contexts.

## 10. India-Specific Considerations
- Support Indian phone formatting (+91) and optional Aadhaar pattern validation.

## 11. Testing Requirements
- Verify CSV imports with malformed entries return readable validation error lists.

## 12. Production-Readiness Requirements
- Implement fuzzy matching for profile duplicate checks.

## 13. Anti-Patterns
- Storing plain-text identity documents or permitting cross-tenant profile visibility.

## 14. Definition of Done
- CSV import checks complete and profile CRM history rendering properly.
