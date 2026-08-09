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
- Secure all administrative endpoints using the custom [RequiresPermission("Capability")] attribute.

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
