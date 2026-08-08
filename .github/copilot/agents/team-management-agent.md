# Team Management Agent

## 1. Purpose
Manages internal users, role scopes, invite states, permissions, and audit tracking.

## 2. Domain Responsibility
- Handles organization users, role assignments (Owner, Admin, Finance, Coordinator, etc.), and activity checks.

## 3. Files to Inspect Before Modifying
- [AuthService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/AuthService.cs)
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (OrganizationUser entity)

## 4. Database Rules
- Emails must be unique across all system users.

## 5. API Rules
- Validate role scopes on request execution.
- Only organization Owners or Admins can modify member roles.

## 6. UI Rules
- Provide clear layout grids listing current users, roles, statuses, and login audit times.

## 7. Validation Rules
- Password hashes and strong password criteria checks must match standards.

## 8. Security & Isolation
- Scope member lists strictly to the authenticated tenant.

## 9. Definition of Done
- Admins can invite or de-activate members.
- Role changes take effect immediately on next authentication token.
