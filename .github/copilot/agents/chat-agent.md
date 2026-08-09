# Chat Agent

## 1. Purpose
Manages internal B2B team communication channels, trip coordinates coordination, and passenger details notes.

## 2. Domain Responsibility
- Handles message logs, room states, attachments data mapping, and timeline indexes.

## 3. Current Repository Reality
- Completely missing. No chat structures or UI views exist.

## 4. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Define ChatMessage tables)

## 5. Database Rules
- Chat message records must contain foreign key links to organizations and specific trips/bookings.

## 6. API Rules
- Restrict message retrieval to members of the specific tenant organization.

## 7. Frontend Rules
- Sliding chat drawer components on trip detail panels.

## 8. Security Rules
- Enforce row-level partition filters to block unauthorized cross-tenant messages fetch.

## 9. Integration Dependencies
- Relies on Trip and Team contexts.

## 10. India-Specific Considerations
- Support Indian standard timezone (IST) formatting on timestamps.

## 11. Testing Requirements
- Integration tests checking that users from Organization A cannot fetch chat threads of Organization B.

## 12. Production-Readiness Requirements
- Keep payload shapes small to minimize performance impact.

## 13. Anti-Patterns
- Creating unpartitioned global rooms that expose communications across tenants.

## 14. Definition of Done
- Chat schemas generated, endpoints authorized, and drawers UI functional.
