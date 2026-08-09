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
- Support Indian Rupee formatting (â‚¹) and localized date layouts (DD-MM-YYYY).

## 11. Testing Requirements
- Verify styles and grids align cleanly across standard screen sizes.

## 12. Production-Readiness Requirements
- Minimize CSS size and optimize image assets load.

## 13. Anti-Patterns
- Using static cards, unlinked buttons, or displaying raw error codes on forms.

## 14. Definition of Done
- Interactivity verified, loaders functional, and layouts responsive.
