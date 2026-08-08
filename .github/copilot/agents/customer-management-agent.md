# Customer Management Agent

## 1. Purpose
Manages traveller profiles, contact logs, history summaries, passport credentials, and client segmentations.

## 2. Domain Responsibility
- Handles CRM details for travellers, passport information, contact lists, preferences, and organization directory logs.

## 3. Files to Inspect Before Modifying
- [TravellerService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/TravellerService.cs)
- [TravellersController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/TravellersController.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Traveller entity)

## 4. Database Rules
- Emails must be unique within an organization scope.
- Enforce strict index on `OrganizationId` and `Email` combination.

## 5. API Rules
- Validate email formats and phone formats before database entry.
- Implement CSV batch uploads with structured error outputs.

## 6. UI Rules
- Provide clear layout grids detailing previous booking history, emergency numbers, and profile tags.

## 7. Validation Rules
- Enforce passport format matching according to nationality where appropriate.

## 8. Security & Isolation
- Do not leak customer profiles across different tenant spaces.

## 9. Definition of Done
- Validation logic catches formatting failures on CRM imports.
- Profiles show historical trip associations correctly.
