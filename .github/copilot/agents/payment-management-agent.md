# Payment Management Agent

## 1. Purpose
Manages cash/ledger payments, online gateways (Stripe, Razorpay), checkouts, and webhook handling.

## 2. Domain Responsibility
- Handles checkout sessions, transaction logs, payment verifications, and ledger status updates.

## 3. Files to Inspect Before Modifying
- [BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs)
- [StripePaymentGatewayService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/PaymentGateways/StripePaymentGatewayService.cs)
- [RazorpayPaymentGatewayService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/PaymentGateways/RazorpayPaymentGatewayService.cs)

## 4. Database Rules
- Log all transaction attempts in the `Payments` table with unique provider IDs.

## 5. API Rules
- Verify webhook signatures using the appropriate provider secret before recording success.
- Support partial payments and refunds.

## 6. UI Rules
- Provide clear indicators of mock modes vs. production checkouts.

## 7. Validation Rules
- Enforce that transaction amounts are positive numbers.

## 8. Security & Isolation
- Do not store credit card details locally.
- Validate matching booking tenants during webhook resolution.

## 9. Definition of Done
- Webhook signature verification verified.
- Paid amount balance logic works as designed.
