# Booking Management Agent

## 1. Purpose
Manages the booking lifecycle (Pending -> Confirmed -> Cancelled -> Completed), passenger rosters, seat deductions, and reservation holds.

## 2. Domain Responsibility
- Handles booking references, seat allocation checks, pricing aggregates, discount structures, cancellations, and refunds.

## 3. Current Repository Reality
- Basic booking creation, seat availability checks, and status tracking exist.
- Lacks booking locks, concurrent seat race protections, and refund ledgers.

## 4. Files to Inspect Before Modifying
- [BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs)
- [BookingsController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/BookingsController.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Booking details)

## 5. Database Rules
- Maintain unique index constraints on BookingReference (BK-XXX-XXXX format).
- Roster associations must hold foreign keys to Travellers.

## 6. API Rules
- Block bookings if number of passengers exceeds available trip seats (Prevent overbooking).
- Force atomic seat reductions during checkout operations.

## 7. Frontend Rules
- Present booking status badges clearly and support filtering by passenger context and payment state.

## 8. Security Rules
- Block anonymous portals from accessing list rosters or internal tenant records.

## 9. Integration Dependencies
- Relies on Payments, Customer CRM, and Notification interfaces.

## 10. India-Specific Considerations
- Support Aadhaar or Passport ID collections on passenger listings for legal compliance.

## 11. Testing Requirements
- Concurrency test cases verifying multiple simultaneous bookings on a single remaining seat.

## 12. Production-Readiness Requirements
- Implement anti-spam / captcha limits on public checkout pages.
- Add checkout session locks.

## 13. Anti-Patterns
- Deducting seats without thread safety checks or transaction locks.

## 14. Definition of Done
- Booking state transitions validated and overbooking prevention verified.
