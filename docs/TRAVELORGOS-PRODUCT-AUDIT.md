# TravelOrgOS — Full Product Audit

This document presents a comprehensive, read-only system audit of the current state of **TravelOrgOS**. It details what is fully implemented, what is partially implemented, what exists only in UI mocks, security and architectural vulnerabilities, technical debt, and a prioritized action matrix for transforming the application into a production-grade enterprise B2B SaaS platform.

---

## 1. Solution & Project Structures

The repository consists of:
- **`TravelOrgOS.Domain`**: A .NET class library containing entity definitions, enums, and request/response DTOs.
- **`TravelOrgOS.Infrastructure`**: EF Core db context mapping, seed initializers, application services (auth, booking, dashboard, reports, travellers, trips), database safety constraints, and mock/gateway abstractions.
- **`TravelOrgOS.Api`**: Controllers, routing mappings, JWT configuration, and CORS setup in ASP.NET Core.
- **`TravelOrgOS.Web`**: An Angular 17 client utilizing standalone components and routing.

---

## 2. Capabilities & Architecture Matrix

Below is the verification matrix identifying functional support levels across all travel SaaS domains.

| Major SaaS Domain / Feature | Status | Backend / EF Model Support | API Integration | Frontend UI Component | Gap Classification / Comments |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Multi-Tenant Isolation** | **Partial** | Yes (`OrganizationId` column) | Partial (hardcoded tenant fallback in some controllers) | Yes | **P0 Critical**: Hardcoded tenant queries bypass safety. |
| **Authentication & Roles** | **Full** | Yes (`OrganizationUser`, `UserRole`) | Yes (JWT credentials validation) | Yes | **P1 Required**: Basic token auth exists; lacks granular permission checks on APIs. |
| **Trip Lifecycle Management** | **Partial** | Yes (`Trips`, status transitions) | Yes (CRUD + builder steps) | Yes | **P1 Required**: Missing trip activation, archive rules, and custom price terms. |
| **Public Booking Flow** | **Full** | Yes (`Bookings`, seat counts) | Yes (`/api/bookings/portal`) | Yes | **P2 Important**: Basic booking works; needs captcha/anti-spam and booking lock. |
| **Booking & Ledger** | **Partial** | Yes (`Bookings`, status enums) | Yes (CRUD + record payment) | Yes | **P1 Required**: Refund transitions and booking communication contexts are missing. |
| **Traveller CRM** | **Partial** | Yes (`Travellers` profiles) | Yes (CRUD + CSV import validation) | Yes | **P2 Important**: Traveller profiles are basic; lacks a consolidated CRM 360-degree timeline. |
| **Vendor Management** | **Partial** | Yes (`Vendors`, `TripVendors`) | Yes (Basic master list CRUD) | Yes | **P2 Important**: Vendors are treated as a static list; lacks vendor balance tracking. |
| **Vehicle / Fleet** | **Partial** | Yes (`Vehicles`, `TripVehicles`) | Yes (Basic master list CRUD) | Yes | **P1 Required**: Lacks vehicle availability calendaring and double-booking blockers. |
| **Guide Management** | **Missing** | No model representation for guides | No API | No | **P1 Required**: Guide profiles and calendar conflicts are completely missing. |
| **Team Management** | **Partial** | Yes (`OrganizationUsers`) | Yes (Auth login) | No UI | **P1 Required**: Lacks tenant team member list CRUD and invite invitation flows. |
| **Payment Gateways** | **Partial** | Yes (`Payments`) | Yes (Mock, Stripe, Razorpay) | Partial | **P1 Required**: Razorpay signature verification logic is stubbed. |
| **Finance & Balances** | **Partial** | Yes (Math on bookings) | Yes (Reports + dashboard stats) | Yes | **P2 Important**: Lacks trip-level vendor payables calculation and actual margins logs. |
| **Marketing Campaigns** | **Missing** | No models, tables, or records | No API | No UI | **P2 Important**: Needs basic segment filter campaigns and newsletter scheduler. |
| **SaaS Subscription / Billing** | **Missing** | No tables or limit enforcement | No API | No UI | **P2 Important**: System lacks central entitlement checker or SaaS limits enforcement. |
| **Notification Center** | **Partial** | Yes (`Notifications`) | Yes (Fetch list + mark read) | Yes | **P2 Important**: Basic center works; needs actual background template dispatch triggers. |
| **Internal Chat & Notes** | **Missing** | No tables, connections | No API | No UI | **P3 Enhancement**: Conversation threads and attachment architecture are missing. |
| **Business Analytics** | **Partial** | Yes (EF aggregate queries) | Yes (`/api/dashboard`) | Yes (Static ranges) | **P2 Important**: Lacks custom date range inputs and guide/vehicle utilization rates. |
| **Operations Dashboard** | **Partial** | Yes (Fetch lists) | Yes | Yes | **P1 Required**: Dashboard has basic lists; lacks prioritizations for expiring documents. |
| **India Travel Domain** | **Partial** | Yes (Currency text) | Yes | Yes | **P2 Important**: Lacks GST tax splits and Indian state code validations. |
| **E2E / Automated Tests** | **Partial** | Yes (3 db safety, 9 math/tenant) | Yes | No | **P1 Required**: Core unit tests exist; lacks end-to-end user-journey scenarios. |

---

## 3. Core Technical & Architectural Gaps

### 3.1 Hardcoded Tenant References
Several master data controller operations in [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs#L59) define the helper method `GetOrgId()` with a hardcoded fallback value:
```csharp
private Guid GetOrgId() => Guid.Parse("11111111-1111-1111-1111-111111111111");
```
This is a critical security vulnerability that compromises multi-tenant isolation. It must retrieve the tenant GUID exclusively from the authenticated JWT `OrganizationId` claim.

### 3.2 Dummy / Missing Guide Management
While the system contains database tables for vehicles, hotels, and vendors, **Guides** are absent from the EF Core schema, the API controllers, and the UI layout. Guides are currently tracked as simple text inputs on Trips (`HostGuide`), which does not support conflict checking or profile attributes.

### 3.3 Missing Team CRUD & Invites
There is no route or page for managing organization users. Adding a member requires manual database insertion. An enterprise SaaS requires a **Team Management UI** to invite, activate/deactivate, and assign roles to users.

### 3.4 Incomplete Razorpay Verification
[BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs) contains signatures for payment session validation, but the webhook verification lacks signature verification against the gateway secret, exposing the system to payment spoofing.

### 3.5 Marketing Campaign stubs
Marketing segments, campaigns, and conversions are completely missing from the schema. A basic tenant campaign log model is required to support Phase 13.

### 3.6 SaaS Limits Enforcement
There is no central entitlement engine in the backend to restrict resource creation (e.g., number of users, active bookings, or total trips) according to the organization's subscription plan.

---

## 4. Security & Compliance Findings

1. **IDOR Risks**: Booking and customer lookups rely on Guids but lack thorough tenant bounds check. Any endpoint retrieving database entities must strictly validate `OrganizationId == userTenantId` on the server.
2. **Secrets Storage**: Verify that no development environment secrets are committed in JSON config formats. Production configurations must resolve environment variables instead.
3. **No GST Calculations**: Bookings calculate `basePrice * passengers` directly without computing standard Indian GST or local state tax levies.

---

## 5. UI/UX Optimization Gaps

- **Loader Feedback**: High-latency actions like importing traveller CSV logs or launching trip configurator stepper lack immediate skeleton screen loaders.
- **Modals vs. Drawers**: Master lists use modal popups for forms instead of sliding side drawers, which restricts context visibility.
- **Desktop/Mobile Focus**: Large operational tables require horizontal scrolling on smaller viewport screens.

---

## 6. Priorities & Backlog Categorization

### P0 Critical (Must fix immediately)
- Resolve hardcoded tenant GUIDs in [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs) to enforce strict tenant isolation.
- Restructure JWT token validations to enforce authorization checks.

### P1 Required (Core Business Integrity)
- Add the `Guide` entity to the domain and EF schema, creating the corresponding management service and UI layout.
- Build the **Team Management UI** for creating, editing, and disabling organization users.
- Add vehicle availability checking to prevent double-booking transport assets.
- Complete the Stripe and Razorpay webhook validation routines.

### P2 Important (Functional Completeness)
- Create a customer history view under the Traveller CRM.
- Implement the basic **Marketing Campaigns** tracker table.
- Build the central SaaS entitlement checker to enforce plan limits on trips/bookings.
- Incorporate Indian GST rules and calculations into the booking workflow.

### P3 Enhancement (Polishing & Context)
- Integrate basic notification email triggers.
- Implement booking notes and audit tracking logs.
