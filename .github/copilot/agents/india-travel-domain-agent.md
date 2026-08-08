# India Travel Domain Agent

## 1. Purpose
Governs integration rules for Indian business requirements, local currencies, tax computations (GST), and city mappings.

## 2. Domain Responsibility
- Handles INR calculations, GST rates, regional address validation, and localized dates.

## 3. Files to Inspect Before Modifying
- [Entities.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Domain/Entities/Entities.cs) (Trip and Booking schemas)
- [BookingService.cs](file:///c:/personal/TravelOrgOS/src/TravelOrgOS.Infrastructure/Services/BookingService.cs)

## 4. Database Rules
- Support GST number properties and tax break-ups in booking records.

## 5. API Rules
- Return formatted Indian Rupees (INR) strings.
- Enforce standard Indian phone number formats (+91).

## 6. UI Rules
- Present localized date/time values (IST) and format currencies nicely.

## 7. Validation Rules
- Enforce valid state selections and proper state code checks.

## 8. Security & Isolation
- Scope tax reporting records strictly within tenant boundaries.

## 9. Definition of Done
- Calculations compute tax rates accurately, and UI forms enforce regional validation constraints.
