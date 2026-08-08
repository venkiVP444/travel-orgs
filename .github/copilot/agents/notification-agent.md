# Notification Agent

## 1. Purpose
Manages internal communications, transactional emails, system alerts, and notification lists.

## 2. Domain Responsibility
- Handles notification creation, delivery formats, reads, and archive markers.

## 3. Files to Inspect Before Modifying
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs) (Notifications controller)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Notification entity)

## 4. Database Rules
- System alerts must contain standard organizational identifier links.

## 5. API Rules
- Filter alerts by the user's specific context.

## 6. UI Rules
- Provide dynamic notification bell UI components in the dashboard header.

## 7. Validation Rules
- Enforce valid message types and titles.

## 8. Security & Isolation
- Block cross-tenant notification queries.

## 9. Definition of Done
- Notifications trigger on bookings and payments, showing read statuses correctly.
