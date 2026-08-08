# Marketing Agent

## 1. Purpose
Manages customer target lists, email updates, and promotional campaigns.

## 2. Domain Responsibility
- Handles customer categories, scheduled messaging campaigns, and click analytics.

## 3. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Needs Campaign implementation)

## 4. Database Rules
- Introduce marketing tables linked to specific tenant organizations.

## 5. API Rules
- Filter campaigns by tenant scope.

## 6. UI Rules
- Provide basic campaign list grids showing sent, delivered, and response metrics.

## 7. Validation Rules
- Enforce valid dates and draft approval checks.

## 8. Security & Isolation
- Do not let different tenants share subscriber databases.

## 9. Definition of Done
- Campaigns can be logged, filtered, and saved.
