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
