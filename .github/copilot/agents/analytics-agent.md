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
- Support parameters for date filtering (startDate and endDate) and scope to organization.

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
