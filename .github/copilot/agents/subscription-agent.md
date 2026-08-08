# Subscription Agent

## 1. Purpose
Manages tenant plan selections, features enablement, and limits check triggers.

## 2. Domain Responsibility
- Handles limits validation for total trips, members count, and booking thresholds.

## 3. Files to Inspect Before Modifying
- [Program.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Program.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Organization status details)

## 4. Database Rules
- Organization plan statuses must reside in tenant mapping fields.

## 5. API Rules
- Block creation commands if organizational limits have been reached.

## 6. UI Rules
- Provide clear displays when plan quotas are near limits.

## 7. Validation Rules
- Enforce strict checks against current database counts.

## 8. Security & Isolation
- Limit definitions must not be overrideable from the frontend.

## 9. Definition of Done
- Plan blocks operate correctly during creation requests.
