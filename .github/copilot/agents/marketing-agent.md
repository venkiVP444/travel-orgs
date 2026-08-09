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
- Campaigns and subscriber logs must map to an OrganizationId.

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
