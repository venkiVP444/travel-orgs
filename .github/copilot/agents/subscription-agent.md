# Subscription Agent

## 1. Purpose
Governs tenant subscription tiers, billing limits, active feature authorizations, and system usage meters.

## 2. Domain Responsibility
- Handles subscription plan parameters, total trips counts, member limits, and entitlement checks.

## 3. Current Repository Reality
- Completely missing. No limits checking filters, subscriptions mapping, or plans entities exist.

## 4. Files to Inspect Before Modifying
- [Program.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Program.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Organization status details)

## 5. Database Rules
- Entitlements and quotas rules must be configured inside dedicated billing schemas.

## 6. API Rules
- Intercept creation calls (Trips, Bookings, Members) via middleware to verify quotas.

## 7. Frontend Rules
- Display clear warnings when billing limits are near capacity or have been exceeded.

## 8. Security Rules
- Quotas and limit constraints must be validated strictly on the server.

## 9. Integration Dependencies
- Relies on Authentication/Security and governing middleware.

## 10. India-Specific Considerations
- Support B2B SaaS e-invoicing and tax rules for Indian corporate billing.

## 11. Testing Requirements
- Unit tests verifying that creation requests are blocked once quotas are exceeded.

## 12. Production-Readiness Requirements
- Support graceful trial expiration warnings.

## 13. Anti-Patterns
- Scattering limit checks inside separate controller files instead of utilizing central middleware filters.

## 14. Definition of Done
- Central quota validator integrated, middleware checks active, and warnings UI complete.
