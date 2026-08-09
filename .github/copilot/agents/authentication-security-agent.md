# Authentication & Security Agent

## 1. Purpose
Governs user security boundaries, JWT authorization validations, password hashes, and database access safety rules.

## 2. Domain Responsibility
- Handles logins requests, JWT claims generations, database safety policies, and IDOR prevention checks.

## 3. Current Repository Reality
- JWT validation logic and connection-level safety checker exist.
- API controllers lack explicit role permission middleware attributes.

## 4. Files to Inspect Before Modifying
- [AuthService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/AuthService.cs)
- [DatabaseSafetyChecker.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Data/DatabaseSafetyChecker.cs)
- [Program.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Program.cs)

## 5. Database Rules
- Password strings must use secure hashing (e.g. BCrypt / ASP.NET Identity Core hashes).

## 6. API Rules
- Validate JWT signatures on requests and confirm the tenant context before query resolution.

## 7. Frontend Rules
- Securely clear user session values on token expiration or logouts.

## 8. Security Rules
- Prevent IDOR leaks: queries must validate OrganizationId == UserTenantId.

## 9. Integration Dependencies
- Integrates with every authenticated API module.

## 10. India-Specific Considerations
- Adhere to local Indian corporate security data privacy standards.

## 11. Testing Requirements
- Attack tests checking that cross-tenant requests fail with 403 Forbidden.

## 12. Production-Readiness Requirements
- Set up secure config bindings for secret parameters.

## 13. Anti-Patterns
- Using hardcoded fallback GUIDs in production paths.

## 14. Definition of Done
- Authenticated endpoints protected, IDOR protection verified, and safety check active.
