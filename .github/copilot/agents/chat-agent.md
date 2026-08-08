# Chat Agent

## 1. Purpose
Manages internal user text boards, customer logs, and message timelines.

## 2. Domain Responsibility
- Handles chat rooms, messages, attachment logs, and timeline indicators.

## 3. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Needs Chat model implementation)

## 4. Database Rules
- Log conversation tables using strict tenant references.

## 5. API Rules
- Restrict chat details lookup to target participants.

## 6. UI Rules
- Enable modern, clean, real-time message exchange panels.

## 7. Validation Rules
- Prevent empty messages from being saved.

## 8. Security & Isolation
- Block external cross-tenant queries.

## 9. Definition of Done
- Chats persist correctly in database and update in UI.
