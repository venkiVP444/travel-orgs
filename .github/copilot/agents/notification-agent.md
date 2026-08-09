# Notification Agent

## 1. Purpose
Manages system alerts dispatching, email templates, booking confirmations, and SMS delivery reports.

## 2. Domain Responsibility
- Handles in-app notifications, templates, reads, delivery logs, and SMS configurations.

## 3. Current Repository Reality
- In-app Notification entity and master API exist.
- Email/SMS senders are not integrated, and there is no background queue execution.

## 4. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Notification table)
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs)

## 5. Database Rules
- Keep alerts partitioned strictly by OrganizationId.

## 6. API Rules
- Support operations to fetch notifications, mark read, and archive items.

## 7. Frontend Rules
- Dynamic dashboard header bell showing active counts.

## 8. Security Rules
- Enforce strict authentication checks for reading or marking notifications.

## 9. Integration Dependencies
- Relies on Bookings, Payments, and Team settings.

## 10. India-Specific Considerations
- SMS templates approved under DLT registrations.

## 11. Testing Requirements
- Verify notifications are triggered upon checkout completions.

## 12. Production-Readiness Requirements
- Background task dispatch queue (e.g. Hangfire/Queue Background Services) to prevent blocking main threads.

## 13. Anti-Patterns
- Blocking transactional requests while waiting for SMTP or SMS gateway responses.

## 14. Definition of Done
- Web triggers active, templates validated, and background dispatch queue active.
