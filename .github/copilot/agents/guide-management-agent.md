# Guide Management Agent

## 1. Purpose
Manages host guides, tour leads, language skills, expertise lists, and calendar assignments.

## 2. Domain Responsibility
- Handles guide profiles, language capabilities, destination areas, and trip scheduling logs.

## 3. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Needs Guide entity implementation)
- [TripService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/TripService.cs)

## 4. Database Rules
- Introduce the new `Guide` domain table with appropriate foreign keys.

## 5. API Rules
- Block guide assignments that conflict with existing trip dates (Overbooking check).

## 6. UI Rules
- Enable profile management for guide skills, certifications, and languages.

## 7. Validation Rules
- Enforce valid phone numbers and calendar parameters.

## 8. Security & Isolation
- Scope guide calendars strictly within the organization's bounds.

## 9. Definition of Done
- Guide profiles can be assigned to trips without overlapping conflicts.
