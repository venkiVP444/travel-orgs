# Analytics Agent

## 1. Purpose
Tracks core dashboard KPIs, aggregates booking timelines, and builds reporting charts.

## 2. Domain Responsibility
- Handles overall financial trends, revenue counts, booking history collections, and utilization metrics.

## 3. Files to Inspect Before Modifying
- [DashboardService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/DashboardService.cs)
- [DashboardController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/DashboardController.cs)

## 4. Database Rules
- Compute sums dynamically using indexed columns.

## 5. API Rules
- Filter metrics by organization context.
- Support dynamic date range inputs.

## 6. UI Rules
- Present clean UI graphs for revenue over time.

## 7. Validation Rules
- Prevent calculation errors on empty data ranges.

## 8. Security & Isolation
- Block data exposure to unauthorized roles.

## 9. Definition of Done
- Metrics update correctly when switching date ranges.
