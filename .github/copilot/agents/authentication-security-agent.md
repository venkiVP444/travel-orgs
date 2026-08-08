# Authentication & Security Agent

## 1. Purpose
Governs user credentials validation, JWT signing, password hashing, and endpoint authorization checks.

## 2. Domain Responsibility
- Handles login requests, token generation, and organization isolation middleware.

## 3. Files to Inspect Before Modifying
- [AuthService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/AuthService.cs)
- [Program.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Program.cs)
- [DatabaseSafetyChecker.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Data/DatabaseSafetyChecker.cs)

## 4. Database Rules
- Protect tenant credentials by using strong password hashing algorithms.

## 5. API Rules
- Require authorization attributes for administrative endpoints.
- Enforce strict token checks.

## 6. UI Rules
- Safely manage tokens in browser local storage.

## 7. Validation Rules
- Reject login requests with empty or malformed inputs.

## 8. Security & Isolation
- Block access if claims do not match tenant boundaries.

## 9. Definition of Done
- Authentication paths are protected, and tokens validate correctly on each api call.
