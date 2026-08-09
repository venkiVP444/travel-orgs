$agentsDir = "c:\personal\TravelOrgOS\.github\copilot\agents"

if (!(Test-Path $agentsDir)) {
    New-Item -ItemType Directory -Force -Path $agentsDir
}

function Write-Agent($filename, $content) {
    $path = Join-Path $agentsDir $filename
    $content | Out-File -FilePath $path -Encoding utf8 -Force
    Write-Host "Updated $filename"
}

# ==========================================
# 1. travel-platform-architect.md
# ==========================================
$architect = @"
# Travel Platform Architect Agent

## 1. Purpose
Governs the overall TravelOrgOS architecture, ensuring compliance with Clean Architecture, SOLID design principles, secure multi-tenant isolation, data governance, and strict cross-agent boundaries.

## 2. Domain Responsibility
- Bounded Context definitions across the entire ecosystem.
- Dependency flow direction validations.
- Cross-module contract interfaces.
- Standard row-level tenant partitioning.
- System audit logging frameworks and database safety guardrails.

## 3. Current Repository Reality
- Solution follows a Clean Architecture design (.NET Web API, Domain, Infrastructure, Web projects).
- Row-level isolation using `OrganizationId` is partially integrated.
- Database safety check intercepts connection strings to prevent office database access.
- Entitlement checks, granular roles validation, and guide scheduling are missing.

## 4. Files to Inspect Before Modifying
- [Program.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Program.cs)
- [BaseApiController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/BaseApiController.cs)
- [DatabaseSafetyChecker.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Data/DatabaseSafetyChecker.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs)

## 5. Database Rules
- Every table holding tenant data must contain a foreign key `OrganizationId` targeting the `Organizations` table.
- All SQL schemas must use relational constraints, foreign keys, and indexes for `OrganizationId`.

## 6. API Rules
- All controllers (excluding anonymous portals) must inherit from `BaseApiController`.
- No endpoints should use fallback hardcoded tenant GUIDs (`11111111-1111-1111-1111-111111111111`) for authenticated scopes.

## 7. Frontend Rules
- Angular routes must resolve URL structures (`/portal/:slug`) for portals and restrict admin pages using guards.
- State management must isolate session tokens per organization tab context.

## 8. Security Rules
- Prevent cross-tenant IDOR (Insecure Direct Object Reference) leaks.
- All administrative endpoints must validate capability permission mappings.

## 9. Integration Dependencies
- Governing authority over all other specialized agents.

## 10. India-Specific Considerations
- Support Indian financial year accounting layout (April 1 to March 31).

## 11. Testing Requirements
- Database connection safety checks must be covered by integration tests.
- Verify cross-tenant isolation boundaries under concurrent requests.

## 12. Production-Readiness Requirements
- Centralized logging context injected into every database request.
- No development credentials or secrets in production configurations.

## 13. Anti-Patterns
- Scattered database connection setups or business logic inside API controllers.
- Hardcoded fallback organization identifiers in controllers or queries.

## 14. Definition of Done
- Strict dependency flow verified. No references from Domain to Infrastructure or Web layers.
- Cross-tenant data isolation verified under regression tests.
"@
Write-Agent "travel-platform-architect.md" $architect

# ==========================================
# 2. trip-management-agent.md
# ==========================================
$trip = @"
# Trip Management Agent

## 1. Purpose
Governs the complete trip lifecycle from draft configuration through pricing, publication, execution, completion, and archiving.

## 2. Domain Responsibility
- Tracks trip profiles, daily itineraries, hotel blocks, transit vehicle mappings, vendor agreements, and passenger capacity.

## 3. Current Repository Reality
- Trip CRUD, basic itinerary days, and stepper configurations exist.
- Hardcoded guide inputs are tracked as a plain text string `HostGuide` in the `Trip` table.
- Lacks a formal scheduling/double-booking protection for transit vehicles or guides.

## 4. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Trip-related tables)
- [TripService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/TripService.cs)
- [TripsController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/TripsController.cs)
- [trip-builder.component.ts](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web/src/app/components/trips/trip-builder.component.ts)

## 5. Database Rules
- Trips must partition by `OrganizationId`.
- Trip code values must be unique within organization contexts.
- Soft-delete trips when bookings have been confirmed.

## 6. API Rules
- Verify `StartDate <= EndDate` on creation/updates.
- Restrict status transitions strictly along the lifecycle sequence (Draft -> Published -> Completed).

## 7. Frontend Rules
- STEPS 1-10 stepper forms must enforce validity before allowing next steps or publishing.

## 8. Security Rules
- Block copying or duplicating trip templates across organization boundaries.

## 9. Integration Dependencies
- Relies on Vendor, Vehicle, Guide, and Pricing modules.

## 10. India-Specific Considerations
- Handle localized Indian destination routing descriptions (State/City tags).

## 11. Testing Requirements
- Verify validation limits on dates, seat availability calculations, and price formats.

## 12. Production-Readiness Requirements
- Graceful error returns on booking validation failures.

## 13. Anti-Patterns
- Modifying capacity below currently reserved seat counts.
- Storing guides as unstructured free-form text strings in the trip model.

## 14. Definition of Done
- Wizard workflows complete, db schemas generated, and date boundaries verified.
"@
Write-Agent "trip-management-agent.md" $trip

# ==========================================
# 3. booking-management-agent.md
# ==========================================
$booking = @"
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
- Maintain unique index constraints on `BookingReference` (BK-XXX-XXXX format).
- Roster associations must hold foreign keys to `Travellers`.

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
"@
Write-Agent "booking-management-agent.md" $booking

# ==========================================
# 4. customer-management-agent.md
# ==========================================
$customer = @"
# Customer/Traveller Management Agent

## 1. Purpose
Governs the traveller directory, customer relations management (CRM), passenger histories, preferences, and details imports.

## 2. Domain Responsibility
- Handles traveller profiles, emergency details, preferences, passport/identity records, and batch CSV imports.

## 3. Current Repository Reality
- Traveller CRM directory, details updates, and batch CSV import with validation exist.
- Lacks a 360-degree timeline view (linking communications, bookings, and payments) and duplicate profiles checks.

## 4. Files to Inspect Before Modifying
- [TravellerService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/TravellerService.cs)
- [TravellersController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/TravellersController.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Traveller table)

## 5. Database Rules
- Customer emails must be unique within an organization scope.

## 6. API Rules
- Validate email syntax and phone structures before persistence.
- Return structured validation report tables on batch CSV importing.

## 7. Frontend Rules
- Provide clear layout profiles highlighting histories, preferences, and profile tags.

## 8. Security Rules
- Encrypt sensitive PII (Passport, Aadhaar details) and restrict data access within tenant boundaries.

## 9. Integration Dependencies
- Relies on Booking and Marketing contexts.

## 10. India-Specific Considerations
- Support Indian phone formatting (+91) and optional Aadhaar pattern validation.

## 11. Testing Requirements
- Verify CSV imports with malformed entries return readable validation error lists.

## 12. Production-Readiness Requirements
- Implement fuzzy matching for profile duplicate checks.

## 13. Anti-Patterns
- Storing plain-text identity documents or permitting cross-tenant profile visibility.

## 14. Definition of Done
- CSV import checks complete and profile CRM history rendering properly.
"@
Write-Agent "customer-management-agent.md" $customer

# ==========================================
# 5. vendor-management-agent.md
# ==========================================
$vendor = @"
# Vendor Management Agent

## 1. Purpose
Manages agreements, contracts, locations, service attributes, and payouts for third-party operators (hotels, restaurants, transport, guides).

## 2. Domain Responsibility
- Handles vendor directories, pricing agreements, contract documents, service categories, and payable records.

## 3. Current Repository Reality
- Basic CRUD endpoints and tables exist for Vendors and Hotels.
- Vendor ledger, balances tracker, and contract expiration warnings do not exist.

## 4. Files to Inspect Before Modifying
- [MasterDataServices.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/MasterDataServices.cs)
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Vendor/Hotel tables)

## 5. Database Rules
- Maintain foreign key relations between trips and assigned vendors.

## 6. API Rules
- Filter lookup listings contextually based on the user's organization GUID.

## 7. Frontend Rules
- Provide forms for locations, service lists, contact persons, and contract parameters.

## 8. Security Rules
- Keep contract pricing sheets restricted to authorized team roles.

## 9. Integration Dependencies
- Relies on Finance and Trip scheduling modules.

## 10. India-Specific Considerations
- Validate 15-character GSTIN (GST Number) formats and track HSN/SAC codes.

## 11. Testing Requirements
- Verify constraint violations are thrown when attempting to delete vendors currently assigned to active trips.

## 12. Production-Readiness Requirements
- Support digital contract document uploads architecture.

## 13. Anti-Patterns
- Hardcoding vendor rates outside of specific contract entities.

## 14. Definition of Done
- Vendor CRUD validated, GSTIN format matching active, and trip mappings verified.
"@
Write-Agent "vendor-management-agent.md" $vendor

# ==========================================
# 6. vehicle-fleet-agent.md
# ==========================================
$vehicle = @"
# Vehicle Fleet Agent

## 1. Purpose
Manages organization transport assets, vehicle specifications, scheduling, driver allocations, and maintenance.

## 2. Domain Responsibility
- Handles vehicle records, capacities, registrations, driver names, insurance details, and scheduling logs.

## 3. Current Repository Reality
- Basic CRUD operations exist for Vehicles.
- Scheduling calendar, date overlap blockades, and document expiration alerts do not exist.

## 4. Files to Inspect Before Modifying
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs) (Vehicles)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Vehicle)

## 5. Database Rules
- Registration plate strings must be unique within an organization boundary.

## 6. API Rules
- Validate date overlaps when assigning vehicles to trips.

## 7. Frontend Rules
- Highlight alert states for vehicles requiring maintenance or having expired documents.

## 8. Security Rules
- Fleet metrics and tracking details must remain strictly isolated per tenant.

## 9. Integration Dependencies
- Relies on Trip Management.

## 10. India-Specific Considerations
- Verify state road permits, National Permit (NP) tags, and local pollution checks (PUC).

## 11. Testing Requirements
- Unit tests validating double-booking prevention of the same vehicle on concurrent dates.

## 12. Production-Readiness Requirements
- Expiring document check schedules triggering in-app alerts.

## 13. Anti-Patterns
- Bypassing scheduling validation checks, resulting in fleet double-bookings.

## 14. Definition of Done
- Date scheduling conflict check logic verified and plates validations integrated.
"@
Write-Agent "vehicle-fleet-agent.md" $vehicle

# ==========================================
# 7. guide-management-agent.md
# ==========================================
$guide = @"
# Guide Management Agent

## 1. Purpose
Orchestrates tour guide profiles, certification details, language proficiencies, scheduling, and calendar conflicts.

## 2. Domain Responsibility
- Handles guide directory profiles, language competencies, daily fee rates, calendars, and trip assignments.

## 3. Current Repository Reality
- Entirely missing from domain entities and UI. Guide details are currently represented by a single text field `HostGuide` on the `Trip` table.

## 4. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Create Guide entity)
- [TripService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/TripService.cs)
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs)

## 5. Database Rules
- Add a new `Guide` table with relationships to `OrganizationId` and `TripGuides`.

## 6. API Rules
- Block guide allocation to trips that contain date overlaps (Prevent double-bookings).

## 7. Frontend Rules
- Build a Guide Management page and integrate guide selection into the Trip Builder wizard.

## 8. Security Rules
- Guide contact details and fee agreements must not leak to other organizations.

## 9. Integration Dependencies
- Relies on Trip Management and Finance.

## 10. India-Specific Considerations
- Track localized language certifications (e.g., Kannada, Tamil, Telugu, Hindi, Malayalam) and local state tourism badges.

## 11. Testing Requirements
- Integration tests checking guide booking overlaps across multiple trips.

## 12. Production-Readiness Requirements
- Handle guide profile statuses (Active, Inactive, OnLeave).

## 13. Anti-Patterns
- Saving guide details as simple text strings in trip logs.

## 14. Definition of Done
- Guide database schema defined, conflict prevention active, and profile CRUD functional.
"@
Write-Agent "guide-management-agent.md" $guide

# ==========================================
# 8. team-management-agent.md
# ==========================================
$team = @"
# Team Management Agent

## 1. Purpose
Governs organization users administration, inviter layouts, roles configuration, security tokens, and permission limits.

## 2. Domain Responsibility
- Handles user lists, role scopes (Owner, Admin, Finance, Coordinator, etc.), invitations, activation states, and security logs.

## 3. Current Repository Reality
- Basic user logins and seed credentials exist.
- Lacks a UI for team list management, invite invitation flows, and roles editor. Endpoints do not enforce permission attribute checks.

## 4. Files to Inspect Before Modifying
- [AuthService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/AuthService.cs)
- [PermissionAttribute.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Authorization/PermissionAttribute.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (OrganizationUser)

## 5. Database Rules
- User email strings must be unique across the entire database.

## 6. API Rules
- Secure all administrative endpoints using the custom `[RequiresPermission("Capability")]` attribute.

## 7. Frontend Rules
- Create a Team settings dashboard displaying member tables, roles, and status controls.

## 8. Security Rules
- Only Owners and Admins can modify other member roles or trigger invitations.

## 9. Integration Dependencies
- Relies on Authentication and Security.

## 10. India-Specific Considerations
- Support SMS invite alerts matching DND and TRAI communication rules.

## 11. Testing Requirements
- Verify that a user assigned a restricted role (e.g., FinanceUser) receives a 403 Forbidden when trying to access admin configurations.

## 12. Production-Readiness Requirements
- Trace profile audits for role modifications.

## 13. Anti-Patterns
- Storing password hashes in plain text or relying solely on client-side UI buttons hidden states to enforce permissions.

## 14. Definition of Done
- All API endpoints guarded, team CRUD active, and role validations verified.
"@
Write-Agent "team-management-agent.md" $team

# ==========================================
# 9. payment-management-agent.md
# ==========================================
$payment = @"
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
- Transaction attempts must be recorded in the `Payments` table with unique provider references.

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
"@
Write-Agent "payment-management-agent.md" $payment

# ==========================================
# 10. finance-balance-agent.md
# ==========================================
$finance = @"
# Finance & Balance Agent

## 1. Purpose
Maintains the organization-level accounting books, ledger entries, gross margins, and tax splits.

## 2. Domain Responsibility
- Tracks total revenue, outstanding customer balances, vendor payables, margins, and taxes.

## 3. Current Repository Reality
- Basic math aggregates in EF Core exist.
- Split GST, ledger audit records, and vendor payables calculations are missing.

## 4. Files to Inspect Before Modifying
- [DashboardService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/DashboardService.cs)
- [BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs)

## 5. Database Rules
- Finance values must calculate dynamically from transaction records (no manual overrides).

## 6. API Rules
- Restrict queries strictly within the tenant's organization boundary.

## 7. Frontend Rules
- Provide clear widgets displaying net margins, overall collections, and outstanding balances.

## 8. Security Rules
- Financial tables must be blocked from users without specific Finance or Owner permissions.

## 9. Integration Dependencies
- Relies on Payments and Bookings.

## 10. India-Specific Considerations
- CGST, SGST, and IGST computations, HSN/SAC mappings, and e-invoice details.

## 11. Testing Requirements
- Unit tests verifying tax calculations and remaining balance equations.

## 12. Production-Readiness Requirements
- Support CSV export of accounting records.

## 13. Anti-Patterns
- Hardcoded tax values without checking destination rules or business zones.

## 14. Definition of Done
- Financial aggregates verified, GST splits integrated, and role restrictions active.
"@
Write-Agent "finance-balance-agent.md" $finance

# ==========================================
# 11. marketing-agent.md
# ==========================================
$marketing = @"
# Marketing Agent

## 1. Purpose
Manages target passenger marketing segments, email campaigns, newsletter configurations, and conversion attribution.

## 2. Domain Responsibility
- Handles campaign templates, segment filters, scheduled dispatches, and campaign analytics logs.

## 3. Current Repository Reality
- Completely missing. No database tables, APIs, or UI interfaces exist.

## 4. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Define Campaign and CampaignLog tables)

## 5. Database Rules
- Campaigns and subscriber logs must map to an `OrganizationId`.

## 6. API Rules
- Restrict subscriber segmentation criteria lookup to the active tenant space.

## 7. Frontend Rules
- Provide form template wizards, schedule inputs, and campaign metrics charts.

## 8. Security Rules
- Tenant subscriber lists must be completely segmented to prevent cross-tenant message leakages.

## 9. Integration Dependencies
- Relies on Customer CRM and Notifications.

## 10. India-Specific Considerations
- TRAI DND (Do Not Disturb) checks and WhatsApp Business API integration templates.

## 11. Testing Requirements
- Unit tests validating segment queries correctly extract target users.

## 12. Production-Readiness Requirements
- Manage opt-out (unsubscribe) links in dispatch scripts.

## 13. Anti-Patterns
- Sending bulk promotional emails to customers who have not opted in.

## 14. Definition of Done
- Marketing models created, segment builder functional, and opt-out flows configured.
"@
Write-Agent "marketing-agent.md" $marketing

# ==========================================
# 12. subscription-agent.md
# ==========================================
$subscription = @"
# Subscription Agent

## 1. Purpose
Governs tenant subscription tiers, billing limits, active feature authorizations, and system usage meters.

## 2. Domain Responsibility
- Handles subscription plan parameters, total trips counts, member limits, and entitlement checks.

## 3. Current Repository Reality
- Completely missing. No limits checking filters, subscriptions mapping, or plans entities exist.

## 4. Files to Inspect Before Modifying
- [Program.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Program.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Organization status details)

## 5. Database Rules
- Entitlements and quotas rules must be configured inside dedicated billing schemas.

## 6. API Rules
- Intercept creation calls (Trips, Bookings, Members) via middleware to verify quotas.

## 7. Frontend Rules
- Display clear warnings when billing limits are near capacity or have been exceeded.

## 8. Security Rules
- Quotas and limit constraints must be validated strictly on the server.

## 9. Integration Dependencies
- Relies on Authentication/Security and governing middleware.

## 10. India-Specific Considerations
- Support B2B SaaS e-invoicing and tax rules for Indian corporate billing.

## 11. Testing Requirements
- Unit tests verifying that creation requests are blocked once quotas are exceeded.

## 12. Production-Readiness Requirements
- Support graceful trial expiration warnings.

## 13. Anti-Patterns
- Scattering limit checks inside separate controller files instead of utilizing central middleware filters.

## 14. Definition of Done
- Central quota validator integrated, middleware checks active, and warnings UI complete.
"@
Write-Agent "subscription-agent.md" $subscription

# ==========================================
# 13. notification-agent.md
# ==========================================
$notification = @"
# Notification Agent

## 1. Purpose
Manages system alerts dispatching, email templates, booking confirmations, and SMS delivery reports.

## 2. Domain Responsibility
- Handles in-app notifications, templates, reads, delivery logs, and SMS configurations.

## 3. Current Repository Reality
- In-app Notification entity and master API exist.
- Email/SMS senders are not integrated, and there is no background queue execution.

## 4. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Notification table)
- [MasterDataControllers.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/MasterDataControllers.cs)

## 5. Database Rules
- Keep alerts partitioned strictly by `OrganizationId`.

## 6. API Rules
- Support operations to fetch notifications, mark read, and archive items.

## 7. Frontend Rules
- Dynamic dashboard header bell showing active counts.

## 8. Security Rules
- Enforce strict authentication checks for reading or marking notifications.

## 9. Integration Dependencies
- Relies on Bookings, Payments, and Team settings.

## 10. India-Specific Considerations
- SMS templates approved under DLT registrations.

## 11. Testing Requirements
- Verify notifications are triggered upon checkout completions.

## 12. Production-Readiness Requirements
- Background task dispatch queue (e.g. Hangfire/Queue Background Services) to prevent blocking main threads.

## 13. Anti-Patterns
- Blocking transactional requests while waiting for SMTP or SMS gateway responses.

## 14. Definition of Done
- Web triggers active, templates validated, and background dispatch queue active.
"@
Write-Agent "notification-agent.md" $notification

# ==========================================
# 14. chat-agent.md
# ==========================================
$chat = @"
# Chat Agent

## 1. Purpose
Manages internal B2B team communication channels, trip coordinates coordination, and passenger details notes.

## 2. Domain Responsibility
- Handles message logs, room states, attachments data mapping, and timeline indexes.

## 3. Current Repository Reality
- Completely missing. No chat structures or UI views exist.

## 4. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Define ChatMessage tables)

## 5. Database Rules
- Chat message records must contain foreign key links to organizations and specific trips/bookings.

## 6. API Rules
- Restrict message retrieval to members of the specific tenant organization.

## 7. Frontend Rules
- Sliding chat drawer components on trip detail panels.

## 8. Security Rules
- Enforce row-level partition filters to block unauthorized cross-tenant messages fetch.

## 9. Integration Dependencies
- Relies on Trip and Team contexts.

## 10. India-Specific Considerations
- Support Indian standard timezone (IST) formatting on timestamps.

## 11. Testing Requirements
- Integration tests checking that users from Organization A cannot fetch chat threads of Organization B.

## 12. Production-Readiness Requirements
- Keep payload shapes small to minimize performance impact.

## 13. Anti-Patterns
- Creating unpartitioned global rooms that expose communications across tenants.

## 14. Definition of Done
- Chat schemas generated, endpoints authorized, and drawers UI functional.
"@
Write-Agent "chat-agent.md" $chat

# ==========================================
# 15. analytics-agent.md
# ==========================================
$analytics = @"
# Analytics Agent

## 1. Purpose
Aggregates sales metrics, occupancy details, operational fleet data, and guide utilization reports.

## 2. Domain Responsibility
- Handles business KPIs, graphs data aggregates, date limits validations, and CSV downloads formatting.

## 3. Current Repository Reality
- Dashboard summary controller serves metrics aggregates.
- Lacks dynamic date ranges and utilization metrics for vehicles or guides.

## 4. Files to Inspect Before Modifying
- [DashboardService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/DashboardService.cs)
- [DashboardController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/DashboardController.cs)

## 5. Database Rules
- Optimization of aggregate queries using index parameters.

## 6. API Rules
- Support parameters for date filtering (`startDate` and `endDate`) and scope to organization.

## 7. Frontend Rules
- Present clean graph charts for metrics over time.

## 8. Security Rules
- Block access to metrics dashboards for Coordinator or Traveller roles.

## 9. Integration Dependencies
- Relies on Dashboard and Finance modules.

## 10. India-Specific Considerations
- Partition records according to Indian Fiscal Year quarters (Q1-Q4).

## 11. Testing Requirements
- Verify query speeds and check results output on empty datasets.

## 12. Production-Readiness Requirements
- Cache results of heavy aggregate queries.

## 13. Anti-Patterns
- Reading entire database logs to calculate sums in application memory.

## 14. Definition of Done
- Dynamic date ranges working, metrics rendering, and role permissions checked.
"@
Write-Agent "analytics-agent.md" $analytics

# ==========================================
# 16. dashboard-agent.md
# ==========================================
$dashboard = @"
# Dashboard Agent

## 1. Purpose
Governs the executive administrative home page, focusing on immediate operational items.

## 2. Domain Responsibility
- Orchestrates actionable cards, status metrics summaries, recent registrations tables, and urgent warnings logs.

## 3. Current Repository Reality
- Basic landing view cards and lists exist.
- Lacks calendar conflicts warnings, document expiration alerts, and dynamic task configurations.

## 4. Files to Inspect Before Modifying
- [DashboardService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/DashboardService.cs)
- [DashboardController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/DashboardController.cs)
- [dashboard.component.ts](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web/src/app/components/dashboard/dashboard.component.ts)

## 5. Database Rules
- Keep analytical queries separate from write transactions.

## 6. API Rules
- Package widgets data into a single request to minimize loading delays.

## 7. Frontend Rules
- Visual cards displaying confirmed bookings, balances due, and active trips.

## 8. Security Rules
- Authenticated JWT credentials must dictate view contents.

## 9. Integration Dependencies
- Relies on Analytics and Notifications.

## 10. India-Specific Considerations
- Support Indian currency symbol (₹) and formats.

## 11. Testing Requirements
- Graceful empty state renders verified.

## 12. Production-Readiness Requirements
- Enforce responsive layout designs across desktop and tablet screens.

## 13. Anti-Patterns
- Hardcoding static demo metrics in dashboard cards.

## 14. Definition of Done
- Summary widgets operational, alert warnings active, and layouts verified.
"@
Write-Agent "dashboard-agent.md" $dashboard

# ==========================================
# 17. public-booking-agent.md
# ==========================================
$publicBooking = @"
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
"@
Write-Agent "public-booking-agent.md" $publicBooking

# ==========================================
# 18. authentication-security-agent.md
# ==========================================
$security = @"
# Authentication & Security Agent

## 1. Purpose
Governs user security boundaries, JWT authorization validations, password hashes, and database access safety rules.

## 2. Domain Responsibility
- Handles logins requests, JWT claims generations, database safety policies, and IDOR prevention checks.

## 3. Current Repository Reality
- JWT validation logic and connection-level safety checker exist.
- API controllers lack explicit role permission middleware attributes.

## 4. Files to Inspect Before Modifying
- [AuthService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/AuthService.cs)
- [DatabaseSafetyChecker.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Data/DatabaseSafetyChecker.cs)
- [Program.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Program.cs)

## 5. Database Rules
- Password strings must use secure hashing (e.g. BCrypt / ASP.NET Identity Core hashes).

## 6. API Rules
- Validate JWT signatures on requests and confirm the tenant context before query resolution.

## 7. Frontend Rules
- Securely clear user session values on token expiration or logouts.

## 8. Security Rules
- Prevent IDOR leaks: queries must validate `OrganizationId == UserTenantId`.

## 9. Integration Dependencies
- Integrates with every authenticated API module.

## 10. India-Specific Considerations
- Adhere to local Indian corporate security data privacy standards.

## 11. Testing Requirements
- Attack tests checking that cross-tenant requests fail with 403 Forbidden.

## 12. Production-Readiness Requirements
- Set up secure config bindings for secret parameters.

## 13. Anti-Patterns
- Using hardcoded fallback GUIDs in production paths.

## 14. Definition of Done
- Authenticated endpoints protected, IDOR protection verified, and safety check active.
"@
Write-Agent "authentication-security-agent.md" $security

# ==========================================
# 19. ux-design-agent.md
# ==========================================
$ux = @"
# UX Design Agent

## 1. Purpose
Governs interface component styles, responsive layout rules, form validator indicators, and loading skeletons.

## 2. Domain Responsibility
- Handles stylesheets, design systems alignment, empty states components, buttons indicators, and responsive flows.

## 3. Current Repository Reality
- Tailwind CSS styles exist.
- Lacks skeleton loaders for batch CSV imports and multi-step configurations. Forms use generic modals instead of slide drawers.

## 4. Files to Inspect Before Modifying
- [styles.scss](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web/src/styles.scss)
- [app.component.ts](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web/src/app/app.component.ts)

## 5. Database Rules
- None (UI presentation focus).

## 6. API Rules
- Display readable error messages matching specific HTTP codes.

## 7. Frontend Rules
- Interfaces must be mobile-responsive and include loading states for high-latency actions.

## 8. Security Rules
- Render active view items matching user permissions.

## 9. Integration Dependencies
- Applies to all Angular views.

## 10. India-Specific Considerations
- Support Indian Rupee formatting (₹) and localized date layouts (DD-MM-YYYY).

## 11. Testing Requirements
- Verify styles and grids align cleanly across standard screen sizes.

## 12. Production-Readiness Requirements
- Minimize CSS size and optimize image assets load.

## 13. Anti-Patterns
- Using static cards, unlinked buttons, or displaying raw error codes on forms.

## 14. Definition of Done
- Interactivity verified, loaders functional, and layouts responsive.
"@
Write-Agent "ux-design-agent.md" $ux

# ==========================================
# 20. qa-e2e-agent.md
# ==========================================
$qa = @"
# QA & E2E Agent

## 1. Purpose
Governs integration test setups, unit tests assertions, and automated end-to-end user-journey testing.

## 2. Domain Responsibility
- Handles backend unit/integration testing suite, E2E journey scripts, and validation coverage checks.

## 3. Current Repository Reality
- Backend xUnit tests for database safety checker and booking calculations exist.
- Lacks automated E2E testing framework setups and coverage for customer journeys.

## 4. Files to Inspect Before Modifying
- [BackendTests.cs](file:///c:/personal/TravelOrgOS/tests/TravelOrgOS.Api.Tests/BackendTests.cs)
- [TravelOrgOS.Api.Tests.csproj](file:///c:/personal/TravelOrgOS/tests/TravelOrgOS.Api.Tests/TravelOrgOS.Api.Tests.csproj)

## 5. Database Rules
- Keep test executions isolated (using memory DB or setup transaction rollbacks).

## 6. API Rules
- Assert correct status codes, data payloads, and error messaging formats.

## 7. Frontend Rules
- Verify E2E flows map accurately to user steps.

## 8. Security Rules
- Include validation cases verifying data leakage is blocked between tenants.

## 9. Integration Dependencies
- Validates all other system modules.

## 10. India-Specific Considerations
- Test GST calculation variations and Razorpay transaction logic.

## 11. Testing Requirements
- Code coverage validations.

## 12. Production-Readiness Requirements
- Integrate testing steps into local deployment validation scripts.

## 13. Anti-Patterns
- Asserting success using HTTP 200 without checking actual database values updates.

## 14. Definition of Done
- Test suites execution passes, coverage limits met, and E2E paths defined.
"@
Write-Agent "qa-e2e-agent.md" $qa

# ==========================================
# 21. india-travel-domain-agent.md
# ==========================================
$india = @"
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
- Localized currency display (₹) and Indian standard date formats.

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
"@
Write-Agent "india-travel-domain-agent.md" $india

Write-Host "All agent files have been successfully updated!"
