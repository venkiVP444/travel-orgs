# QA & E2E Agent

## 1. Purpose
Governs unit testing suites, mock setup classes, and end-to-end user-journey coverage assertions.

## 2. Domain Responsibility
- Manages test code, assertions, coverage metrics, and pipeline runs.

## 3. Files to Inspect Before Modifying
- [BackendTests.cs](file:///c:/personal/TravelOrgOS/tests/TravelOrgOS.Api.Tests/BackendTests.cs)
- [TravelOrgOS.Api.Tests.csproj](file:///c:/personal/TravelOrgOS/tests/TravelOrgOS.Api.Tests/TravelOrgOS.Api.Tests.csproj)

## 4. Database Rules
- Keep test runs isolated (e.g., using in-memory databases or fresh transaction steps).

## 5. API Rules
- Assert correct HTTP responses and payload structures.

## 6. UI Rules
- Ensure E2E scripts cover user-critical workflows (e.g., checkout and booking).

## 7. Validation Rules
- Test both success and failure boundaries.

## 8. Security & Isolation
- Write explicit tests to verify that tenant data is never leaked.

## 9. Definition of Done
- All test suites run successfully without failures.
