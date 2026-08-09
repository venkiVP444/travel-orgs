# TravelOrgOS - Repository Discovery & Agent Self-Audit Report

This document reports the findings of a complete repository-aware analysis of the **TravelOrgOS** codebase and its 21 specialized agents. The objective is to align all agents with the repository reality and establish the foundation to evolve TravelOrgOS into an enterprise-grade, multi-tenant B2B Travel SaaS platform tailored for the Indian travel market.

---

## 1. Current Repository Capability Matrix

Below is a detailed matrix representing the exact status of each SaaS domain in the codebase:

| Domain / Feature Area | Backend Model / Schema Status | Backend API / Service Status | Angular Frontend UI Status | Gap Classification & Findings |
| :--- | :--- | :--- | :--- | :--- |
| **Multi-Tenant Isolation** | **Full**: `OrganizationId` column exists in major tables. | **Partial**: Auth token extracts tenant context. PlatformAdmin has fallback. | **Full**: URL routes resolve dynamic organization slug. | Lacks checks on master data deletions and cascade rules. |
| **Auth & Security** | **Full**: `OrganizationUser` and `UserRole` enums are mapped. | **Full**: JWT signing, verification, and authentication filters. | **Full**: Session persistence and route guards. | Capability permission validation attribute (`RequiresPermission`) exists but is not applied to any controller endpoints. |
| **Trip Management** | **Full**: Trips, itineraries, dates, hotel/vehicle/vendor mappings. | **Full**: Stepper CRUD operations, copy/duplicate, and publish. | **Full**: 10-step wizard configuration layout. | Needs vehicle/guide availability checking; `HostGuide` is a plain text field. |
| **Booking & Seats** | **Full**: `Bookings` and `BookingTravellers` models exist. | **Full**: Create, cancel, and seat calculations service logic. | **Full**: Seats matrices, passenger listing, and checkout. | Lacks concurrency locks on seat reservation; no refund ledger mappings. |
| **Customer CRM** | **Full**: `Travellers` table with emergency details and passport info. | **Full**: Profiles CRUD and batch CSV import with validation. | **Full**: Profile grids and file uploading buttons. | Lacks a consolidated customer timeline (bookings, transactions, communications). |
| **Vendor Management** | **Full**: `Vendors` and `TripVendors` mapped. | **Full**: Basic CRUD services. | **Full**: Master list pages. | treated as static registry; lacks accounts payable tracking. |
| **Vehicle Fleet** | **Full**: `Vehicles` and `TripVehicles` mapped. | **Full**: Basic CRUD services. | **Full**: Fleet master lists. | Lacks calendar checking and prevention of double-booking transport assets. |
| **Guide Management** | **Missing**: No Guide entities or tables. | **Missing**: No Guide services or APIs. | **Missing**: Handled via simple text inputs on Trips. | Guides must be fully integrated as a scheduling entity. |
| **Team Management** | **Full**: `OrganizationUsers` table exists. | **Full**: Basic registration. | **Missing**: No UI layout to invite or edit team users. | Needs invitation flow and role assignment control views. |
| **Payments** | **Full**: `Payments` table holds gateway attributes. | **Partial**: Stripe/Razorpay session initialize. Razorpay signature check stubbed. | **Partial**: Payment option selectors and return page. | Signature checking has a mock fallback and needs live webhook validations. |
| **Finance & Balances** | **Full**: Fields for amount totals, paid, and balances. | **Full**: Auto calculations of balance upon payments updates. | **Full**: Dashboard analytics cards. | Needs GST splits (CGST, SGST, IGST) and immutable transaction ledgers. |
| **Marketing** | **Missing**: No Campaign entities or tables. | **Missing**: No Marketing services. | **Missing**: No UI. | Requires campaign models and segment queries. |
| **Subscription Billing** | **Missing**: No billing or entitlement maps. | **Missing**: No entitlement checks. | **Missing**: No UI. | Quota enforcement middleware must be introduced. |
| **Notifications** | **Full**: `Notification` table and types exist. | **Full**: Save alerts and list fetch. | **Full**: Bell icon header count. | Alerts are in-app only; lacks SMS and email sender dispatchers. |
| **Internal Chat** | **Missing**: No Chat tables or models. | **Missing**: No APIs. | **Missing**: No UI. | Chat threads must map to trips/bookings. |
| **Analytics** | **Full**: Summarized collections. | **Full**: Dashboard KPIs. | **Full**: Metrics charts. | Calculations use full table queries; lacks date filters and utilization ratios. |
| **Public Booking Portal** | **Full**: Portals fetch and checkout. | **Full**: Guest booking creation. | **Full**: Mobile-first portals. | Needs anti-spam limits and reservation holds. |
| **E2E Testing** | **Partial**: Unit tests cover connection checks and bookings. | **Partial**: xUnit tests in `tests/`. | **Missing**: No Cypress/Playwright E2E coverage. | Verification of user workflows is missing. |

---

## 2. Current Agent Capability Matrix

All 21 agents under `.github/copilot/agents` have been updated. The matrix below defines the capabilities owned by each updated agent:

| Agent File | Purpose & Primary Scope | Governing Rules & Domain Limits |
| :--- | :--- | :--- |
| **1. travel-platform-architect.md** | Central technical authority | Clean Architecture compliance, dependency direction, multi-tenancy, and audit frameworks. |
| **2. trip-management-agent.md** | Trip lifecycle orchestrator | Stepper validations, copying template rules, status progression, and capacity limits. |
| **3. booking-management-agent.md** | Reservation controller | Reference generations, passenger rosters, concurrency holds, and overbooking blocks. |
| **4. customer-management-agent.md** | Customer CRM manager | Duplicate checks, fuzzy profile matching, CSV parsing, and passport/PII data encryption. |
| **5. vendor-management-agent.md** | Vendor operations manager | Supplier profiles, contract amount ledgers, location details, and GSTIN/SAC tags. |
| **6. vehicle-fleet-agent.md** | Transit logistics manager | Registration plates uniqueness, fleet maintenance schedules, and double-booking blocks. |
| **7. guide-management-agent.md** | Guide scheduler | Guide entity mapping, calendar conflict validations, language badges, and tourism licenses. |
| **8. team-management-agent.md** | Member access manager | Roles mapping, invitations activation, audit trails, and capability permission checks. |
| **9. payment-management-agent.md** | Checkout & webhook authority | Webhook signature checks, idempotency keys, refunds, and transaction logging. |
| **10. finance-balance-agent.md** | Ledgers & accounting checker | Profit accounting, CGST/SGST/IGST splits, gross margins, and immutable records. |
| **11. marketing-agent.md** | Subscriber promotions manager | Segments builder, email templates, consent check (opt-out), and TRAI compliance. |
| **12. subscription-agent.md** | SaaS limits manager | Subscriptions tiers, usage meters, quota checker middleware, and billing alerts. |
| **13. notification-agent.md** | Alerts dispatcher | In-app alerts, background dispatch queues, SMS DLT templates, and email scripts. |
| **14. chat-agent.md** | Thread communication manager | Room isolation, trip thread mapping, message payloads, and standard IST timestamps. |
| **15. analytics-agent.md** | Business analytics generator | SQL metrics optimizations, date range parameters, and vehicle/guide utilization rates. |
| **16. dashboard-agent.md** | Command dashboard designer | "Attention required" cards, task lists, due actions, and responsive layout grids. |
| **17. public-booking-agent.md** | Guest booking flow authority | Portals checkout, rate-limiting check, anti-spam, and private roster concealment. |
| **18. authentication-security-agent.md** | Security boundary protector | JWT claims extraction, IDOR check, database connection safety, and input cleaning. |
| **19. ux-design-agent.md** | UI consistency inspector | Component grids, loader skeletons, slide drawers, forms feedback, and mobile layouts. |
| **20. qa-e2e-agent.md** | Validation suite checker | Integration test logs, transaction rollback rules, and user-journey E2E coverage. |
| **21. india-travel-domain-agent.md** | Localized markets authority | INR currency format (₹), CGST/SGST/IGST zones, local states validation, and Razorpay. |

---

## 3. Agent Dependency Map

The module scheduling hierarchy is structured as follows:

```text
Subscription (Entitlements check on creation workflows)
  └── Security (Applies permissions checks across all modules)
        ├── Trip Management
        │     ├── Traveller (CRM link)
        │     ├── Vendor (Contracts)
        │     ├── Vehicle (Fleet overlaps)
        │     └── Guide (Scheduling overlaps)
        ├── Booking Management
        │     ├── Traveller (Roster records)
        │     ├── Payment Management (Transactions)
        │     ├── Finance & Balance (GST split accounting)
        │     └── Notification (Receipt confirmations)
        └── Team Management (User role permissions)
              └── Chat (Trip/Booking logs)
```

---

## 4. Critical Architecture Conflicts & Resolutions

1. **Trip vs. Vehicle / Guide Assignments**:
   - *Conflict*: If trip dates change, assigned vehicles/guides could overlap with other active trips.
   - *Resolution*: Trip Management agent holds authority over the trip parameters; however, if dates are changed, it must trigger dates validations in the Vehicle and Guide agents. If conflicts are found, the update is blocked.
2. **Booking vs. Payment Webhooks**:
   - *Conflict*: A payment webhook could execute after a booking has already been cancelled.
   - *Resolution*: Webhooks processing must follow a strict transaction block. If booking is cancelled, the amount must be logged as a "Refund Pending" credit rather than modifying the booking state back to Paid.
3. **SaaS Quotas vs. Endpoint Operations**:
   - *Conflict*: Quota checks scattered throughout different controllers create redundant queries and code duplication.
   - *Resolution*: Subscription Agent enforces a centralized Quota Middleware filter that executes prior to model creations, keeping controllers thin.

---

## 5. Security & Isolation Risks

1. **Granular Permissions Gaps**: Controller endpoints do not enforce permission parameters checks. Any authenticated user can execute updates.
2. **IDOR Risks**: Queries on details lookup (e.g. fetch booking by ID) do not verify if the target resource matches the user's organization GUID.
3. **Hardcoded Fallbacks**: Bypassing tenant verification checks by falling back to the default organization GUID in `BaseApiController` must be strictly restricted to `PlatformAdmin` scopes.

---

## 6. Backlog Priorities & Execution Roadmap

### P0: Critical Security & Core Alignment
- Refactor all controllers to ensure `RequiresPermission` filters are applied.
- Apply row-level organization checks on booking detail requests.
- Restrict default organization fallbacks strictly to verified system admin roles.

### P1: Core Business Entities
- Build Guide domain tables, scheduling validation logic, and frontend configuration lists.
- Implement the User management list and invitation flow (Team settings).
- Integrate vehicle/fleet date overlap checking.
- Refactor Razorpay signatures checks to validate webhook payloads against secret keys.

### P2: India Market & SaaS Architecture
- Implement CGST, SGST, and IGST tax splits during booking and finance updates.
- Create the billing quota middleware filter.
- Build campaign and segments models for target promotions.
- Create background queues for notification alerts (SMS/Email).

### P3: Polish & UX Enhancements
- Build slide drawers for master directories (instead of popups).
- Implement the chat log drawer inside trip detail dashboards.
- Add dynamic date pickers to the business analytics graphs.
