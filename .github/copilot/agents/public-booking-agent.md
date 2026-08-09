# Public Booking Agent

## 1. Purpose
Manages public passenger routes, checkout screens, passenger profiles inputs, and direct bookings creation.

## 2. Domain Responsibility
- Handles public trip catalog layouts, passenger booking forms, and payment redirects.

## 3. Current Repository Reality
- Public booking portal and detail routes exist.
- Lacks anti-spam captcha, booking locks, and confirmation validation screens.

## 4. Files to Inspect Before Modifying
- [BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs)
- [BookingsController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/BookingsController.cs)
- [portal-booking.component.ts](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web/src/app/components/portal/portal-booking.component.ts)

## 5. Database Rules
- Assign appropriate tenant GUID mappings to bookings made through public forms.

## 6. API Rules
- Validate email, phone, and seat parameters. Return clean errors for missing fields.

## 7. Frontend Rules
- Force mobile-responsive layouts tailored to customized brand colors.

## 8. Security Rules
- Public endpoints must not expose private customer lists or tenant administration routes.

## 9. Integration Dependencies
- Relies on Booking and Payments.

## 10. India-Specific Considerations
- Support UPI, Razorpay payment flows, and localized mobile phone formats.

## 11. Testing Requirements
- Integration tests validating guest booking submissions correct seat reductions.

## 12. Production-Readiness Requirements
- Implement rate-limiting validations on checkout endpoints.

## 13. Anti-Patterns
- Permitting checkout booking processing without active seat capacity confirmation.

## 14. Definition of Done
- Checkout complete, seat deductions validated, and redirects secure.
