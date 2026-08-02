# TravelOrgOS REST API Specification

## Auth Endpoints
- `POST /api/auth/login`: Accepts `{ email, password }`, returns JWT token and organization metadata.

## Organization Endpoints
- `GET /api/organizations/me`: Retrieves current authenticated organization branding.
- `PUT /api/organizations/me`: Updates organization branding, colors, welcome message, and contact details.

## Traveller Endpoints
- `GET /api/travellers?search=`: Lists travellers with optional search query.
- `POST /api/travellers`: Creates new traveller profile.
- `PUT /api/travellers/{id}`: Updates existing traveller profile.
- `DELETE /api/travellers/{id}`: Deletes traveller profile.
- `POST /api/travellers/import`: Uploads CSV file for batch traveller import.
- `GET /api/travellers/import/template`: Downloads CSV import template.

## Trip & Builder Endpoints
- `GET /api/trips?search=&status=&publicOnly=`: Lists trips with filtering options.
- `GET /api/trips/{id}`: Gets trip details with itinerary, hotels, vehicles, vendors, meals.
- `POST /api/trips`: Creates new trip.
- `PUT /api/trips/{id}`: Updates trip basic details & capacity.
- `POST /api/trips/{id}/publish`: Publishes trip to Traveller Portal.
- `POST /api/trips/{id}/unpublish`: Saves trip back to draft.
- `POST /api/trips/{id}/duplicate`: Duplicates trip package.
- `POST /api/trips/{id}/itinerary`: Saves day-by-day itinerary.
- `POST /api/trips/{id}/hotels`: Saves hotel assignments.
- `POST /api/trips/{id}/vehicles`: Saves vehicle fleet assignments.
- `POST /api/trips/{id}/vendors`: Saves vendor partner allocations.
- `POST /api/trips/{id}/meals`: Saves meal plan inclusions.

## Booking & Payment Endpoints
- `GET /api/bookings?search=`: Lists booking ledger.
- `POST /api/bookings`: Creates admin booking with seat reservation.
- `POST /api/bookings/portal/{orgSlug}`: Public Traveller Portal booking endpoint.
- `POST /api/bookings/{id}/confirm`: Confirms booking.
- `POST /api/bookings/{id}/cancel`: Cancels booking and restores available seats.
- `POST /api/bookings/{id}/payment`: Records manual or card payment against booking.

## Dashboard & Report Endpoints
- `GET /api/dashboard`: Calculates real-time KPIs, upcoming trips, recent bookings, and 6-month trends.
- `GET /api/reports/bookings/export`: Downloads CSV report for bookings.
- `GET /api/reports/travellers/export`: Downloads CSV report for travellers.
- `GET /api/reports/revenue/export`: Downloads CSV report for revenue.
- `GET /api/reports/outstanding/export`: Downloads CSV report for outstanding balances.
