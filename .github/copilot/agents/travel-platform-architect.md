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
- Row-level isolation using OrganizationId is partially integrated.
- Database safety check intercepts connection strings to prevent office database access.
- Entitlement checks, granular roles validation, and guide scheduling are missing.

## 4. Files to Inspect Before Modifying
- [Program.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Program.cs)
- [BaseApiController.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Api/Controllers/BaseApiController.cs)
- [DatabaseSafetyChecker.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Data/DatabaseSafetyChecker.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs)

## 5. Database Rules
- Every table holding tenant data must contain a foreign key OrganizationId targeting the Organizations table.
- All SQL schemas must use relational constraints, foreign keys, and indexes for OrganizationId.

## 6. API Rules
- All controllers (excluding anonymous portals) must inherit from BaseApiController.
- No endpoints should use fallback hardcoded tenant GUIDs (11111111-1111-1111-1111-111111111111) for authenticated scopes.

## 7. Frontend Rules
- Angular routes must resolve URL structures (/portal/:slug) for portals and restrict admin pages using guards.
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
