# Public Booking Agent

## 1. Purpose
Manages public trip catalog routes, checkout operations, and direct bookings creation.

## 2. Domain Responsibility
- Handles public registration flows, seat counts, and payment gateway redirects.

## 3. Files to Inspect Before Modifying
- [BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs) (Portal checkout functions)
- [BookingsController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/BookingsController.cs)

## 4. Database Rules
- Assign appropriate organization mappings to public bookings.

## 5. API Rules
- Do not expose administrative context in public endpoints.

## 6. UI Rules
- Enable mobile-first checkout screens.

## 7. Validation Rules
- Enforce seat count validation and duplicate submission limits.

## 8. Security & Isolation
- Do not leak private participant lists via public trip tokens.

## 9. Definition of Done
- Guests can book trips successfully, and seat counts update correctly.
