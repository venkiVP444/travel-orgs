# TravelOrgOS - Real-World E2E Test Report

This document reports on the functional validation, integration coverage, and E2E verification journeys.

---

## 1. Test Verification Overview

The system compiles clean and successfully passes **20 backend tests** verifying the business rules, security thresholds, and Indian tax allocations.

```bash
Passed!  - Failed:     0, Passed:    20, Skipped:     0, Total:    20, Duration: 1 s
```

---

## 2. Core E2E Scenarios Validated

### Journey A: Authentication & Dashboard
- **Action**: Authenticate as Organization Owner (`owner@demo-travel.com` / `Demo@123`).
- **Result**: Server returns a valid JWT token. Renders the main dashboard containing actual operational KPIs.
- **Vulnerability Check**: Requests lacking the JWT header throw a `401 Unauthorized` exception immediately.

### Journey B: Trip Creation & Stepper Builder
- **Action**: Configure a new trip (dates, capacity, pricing base) and save as draft.
- **Result**: Trip status transitions to `Draft`. The builder lets the operator add hotels, vehicles,certified tour guides, and vendors.
- **Vulnerability Check**: Bypassing stepper fields fails model validations on save.

### Journey C: Public Traveller Portal & Booking Checkout
- **Action**: Renders the public portal `/portal/demo-travel` for anonymous customers. Selects tickets and submits a booking.
- **Result**: Checks capacity limits, allocates seats, and confirms the reservation.
- **Vulnerability Check**: Overbooking checks block checkouts when seats are exceeded. Multiple simultaneous checkout attempts are serialized via EF Core transaction concurrency row locks (`UPDLOCK, ROWLOCK`).

### Journey D: Webhook Signature Checks & Tax Splits
- **Action**: Trigger Stripe and Razorpay webhook events.
- **Result**: Webhooks verify payload signatures against secrets. Calculated CGST (9%), SGST (9%), or IGST (18%) tax splits are successfully persisted.
- **Vulnerability Check**: Forged or missing signatures return a `400 BadRequest` and block ledger updates.

### Journey E: Guide & Vehicle Date Overlaps
- **Action**: Assign Guide A / Vehicle A to Trip 1 (Aug 10 - Aug 15). Attempt to assign same resources to Trip 2 (Aug 12 - Aug 17).
- **Result**: Validation checks block the action with: `GUIDE CONFLICT` / `VEHICLE CONFLICT` error.
- **Vulnerability Check**: Assigning inactive guides or vehicles throws validation errors.

### Journey F: SaaS subscription Quotas
- **Action**: Exceed the active trips limit of the Starter plan (max 3 trips).
- **Result**: Central entitlement middleware checks organization quotas on the server and rejects trip creation.
- **Vulnerability Check**: Upgrading the plan tier immediately expands the limits.

### Journey G: Multi-Tenant Boundary Checks
- **Action**: Log in as Organization A. Attempt to query or modify Organization B's trips or bookings using GUIDs.
- **Result**: Server rejects the cross-tenant operation with a `404 Not Found` or access denied exceptions.
