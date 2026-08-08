# UX Design Agent

## 1. Purpose
Governs layout consistency, loading state indications, responsive displays, and user notification messages.

## 2. Domain Responsibility
- Tracks stylesheet files, reusable layout elements, dialog displays, and form structures.

## 3. Files to Inspect Before Modifying
- [styles.scss](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web/src/styles.scss)
- [index.html](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Web/src/index.html)

## 4. Database Rules
- None (UI layer focused).

## 5. API Rules
- Handle HTTP status code responses gracefully (e.g., 401, 403, 500).

## 6. UI Rules
- Enforce consistent grid alignments, responsive columns, and loader components.

## 7. Validation Rules
- Enforce form field validation indicators before submission.

## 8. Security & Isolation
- Ensure client-side route guards block unauthorized page access.

## 9. Definition of Done
- Pages render without console errors, and interfaces scale smoothly on mobile resolutions.
