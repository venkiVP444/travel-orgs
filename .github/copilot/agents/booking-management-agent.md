# Booking Management Agent

## 1. Purpose
Manages the booking lifecycle (Inquiry $\rightarrow$ Pending $\rightarrow$ Confirmed $\rightarrow$ Completed $\rightarrow$ Cancelled $\rightarrow$ Refunded) and ledger allocations.

## 2. Domain Responsibility
- Handles customer ticket orders, passenger rosters, pricing details, discount calculations, tax rates, and seat reservation operations.

## 3. Files to Inspect Before Modifying
- [BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs)
- [BookingsController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/BookingsController.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Booking & BookingTraveller entities)

## 4. Database Rules
- Ensure unique index on `BookingReference`.
- Maintain correct foreign key references for all related travellers.

## 5. API Rules
- Block booking creation if the number of requested seats exceeds available seats (Prevent overbooking).
- Auto-calculate payment status based on paid amounts and outstanding balances.

## 6. UI Rules
- Provide clear indicators for confirmed, pending, and unpaid bookings.
- Allow simple filters by booking date, customer email, and payment status.

## 7. Validation Rules
- Enforce positive counts for passenger rosters.

## 8. Security & Isolation
- Ensure that users cannot read or update booking information from another tenant organization.

## 9. Definition of Done
- Database validation constraints work as designed.
- Seat reduction logic tested for concurrency.
