# Payment Management Agent

## 1. Purpose
Governs checkout session initialization, transaction records, online gateways, and webhooks processing.

## 2. Domain Responsibility
- Handles Stripe and Razorpay integrations, session states, webhook checks, refunds, and payment retries.

## 3. Current Repository Reality
- Pluggable Stripe/Razorpay/Mock gateway structures exist.
- Signature checking and webhook payload validation are missing or stubbed in Razorpay implementations.

## 4. Files to Inspect Before Modifying
- [RazorpayPaymentGatewayService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/Payment/RazorpayPaymentGatewayService.cs)
- [StripePaymentGatewayService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/Payment/StripePaymentGatewayService.cs)
- [WebhooksController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/WebhooksController.cs)

## 5. Database Rules
- Transaction attempts must be recorded in the Payments table with unique provider references.

## 6. API Rules
- Webhook signature checks must validate before updating checkout record statuses.
- Webhook endpoints must process events idempotently using event identifiers.

## 7. Frontend Rules
- Provide payment gateway modal integration or checkout redirection layouts.

## 8. Security Rules
- Never store customer card details locally.
- Validate that the webhook payload's tenant details match the booking database records.

## 9. Integration Dependencies
- Relies on Bookings, Finance, and Notifications.

## 10. India-Specific Considerations
- Razorpay UPI, Netbanking, and Wallets integrations (INR transactions and currency codes validation).

## 11. Testing Requirements
- Integration tests simulating successful/failed gateway webhook payloads with valid signatures.

## 12. Production-Readiness Requirements
- Gateway reconciliation job script to catch missing webhook sessions.

## 13. Anti-Patterns
- Marking booking states to Paid without verifying signatures.

## 14. Definition of Done
- Signature verification working, payment states mapped, and webhooks tested.
