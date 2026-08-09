# India Travel Domain Agent

## 1. Purpose
Governs Indian travel market business integrations, taxation (GST), invoice formatting, and Razorpay rules.

## 2. Domain Responsibility
- Handles GST (CGST/SGST/IGST) calculations, GSTIN validation formats, local state code configs, and INR transactions.

## 3. Current Repository Reality
- Pluggable Razorpay gateway class exists.
- Incomplete Indian business rules: GST tax computations, state validations, SAC mappings, and invoice itemizations are missing.

## 4. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs)
- [BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs)
- [RazorpayPaymentGatewayService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/Payment/RazorpayPaymentGatewayService.cs)

## 5. Database Rules
- Support organization GSTIN profiles and itemized tax breakdowns.

## 6. API Rules
- Validate GSTIN syntax (15-character alphanumeric format). Enforce phone prefix (+91).

## 7. Frontend Rules
- Localized currency display (â‚¹) and Indian standard date formats.

## 8. Security Rules
- Restrict tax report ledger viewing to specific authorized roles.

## 9. Integration Dependencies
- Relies on Payments, Bookings, and Finance.

## 10. India-Specific Considerations
- CGST, SGST, and IGST rule determination based on inter-state vs. intra-state operations.

## 11. Testing Requirements
- Unit tests verifying GST split calculations on diverse customer state origins.

## 12. Production-Readiness Requirements
- Generate accounting invoice PDF structures.

## 13. Anti-Patterns
- Hardcoding static GST percentage numbers without verifying state zones.

## 14. Definition of Done
- Tax splits integrated, GSTIN formats checked, and INR formatting implemented.
