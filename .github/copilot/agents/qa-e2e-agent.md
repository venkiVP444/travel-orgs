# QA & E2E Agent

## 1. Purpose
Governs integration test setups, unit tests assertions, and automated end-to-end user-journey testing.

## 2. Domain Responsibility
- Handles backend unit/integration testing suite, E2E journey scripts, and validation coverage checks.

## 3. Current Repository Reality
- Backend xUnit tests for database safety checker and booking calculations exist.
- Lacks automated E2E testing framework setups and coverage for customer journeys.

## 4. Files to Inspect Before Modifying
- [BackendTests.cs](file:///c:/personal/TravelOrgOS/tests/TravelOrgOS.Api.Tests/BackendTests.cs)
- [TravelOrgOS.Api.Tests.csproj](file:///c:/personal/TravelOrgOS/tests/TravelOrgOS.Api.Tests/TravelOrgOS.Api.Tests.csproj)

## 5. Database Rules
- Keep test executions isolated (using memory DB or setup transaction rollbacks).

## 6. API Rules
- Assert correct status codes, data payloads, and error messaging formats.

## 7. Frontend Rules
- Verify E2E flows map accurately to user steps.

## 8. Security Rules
- Include validation cases verifying data leakage is blocked between tenants.

## 9. Integration Dependencies
- Validates all other system modules.

## 10. India-Specific Considerations
- Test GST calculation variations and Razorpay transaction logic.

## 11. Testing Requirements
- Code coverage validations.

## 12. Production-Readiness Requirements
- Integrate testing steps into local deployment validation scripts.

## 13. Anti-Patterns
- Asserting success using HTTP 200 without checking actual database values updates.

## 14. Definition of Done
- Test suites execution passes, coverage limits met, and E2E paths defined.
