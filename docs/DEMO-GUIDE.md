# TravelOrgOS Sales Demo Script & Guide (10-15 Minutes)

## Hero Demo Flow

```
Organization Login -> Dashboard -> Travellers -> CSV Import -> Create Trip -> Trip Builder (10 Steps) -> Add Inclusions -> Publish -> Open Branded Traveller Portal -> Traveller Books Trip -> Mock Payment -> Booking Confirmation -> Admin Ledger Updated -> Available Seats Updated -> Notification Generated
```

## Demo Script

### Step 1: Login & Executive Dashboard (2 Mins)
- Open `http://localhost:4400`
- Click **Org Owner** one-click login (`owner@demo-travel.com` / `Demo@123`).
- Show the executive KPIs: Active Trips, Total Travellers, Confirmed Bookings, Total Revenue, and Outstanding Balances.

### Step 2: Traveller Directory & CSV Import (3 Mins)
- Navigate to **Travellers**.
- Show existing traveller profiles with passport numbers and emergency contacts.
- Click **Import CSV**, download the reference CSV template, choose a sample CSV file, and click **Process CSV Import**.
- Point out the instant validation summary showing total, successful, duplicate, and error records.

### Step 3: Trip Management & Multi-Step Builder (4 Mins)
- Navigate to **Trips**.
- Click **Launch Trip Builder**.
- Walk through the 10-step Stepper:
  1. Basic Info & Code (`KER-2026-001`)
  2. Dates & Capacity
  3. Itinerary Days
  4. Hotels & Room Counts
  5. Vehicles & Fleet
  6. Meals & Dietary Options
  7. Vendors & Partner Contracts
  8. Pricing & Deposit Rules
  9. Live Card Preview
  10. Publish Action
- Click **Publish to Traveller Portal Now**.

### Step 4: Branded Mobile Traveller Portal & Booking (4 Mins)
- Click **Open Branded Portal** (opens `/portal/demo-travel`).
- Show organization logo, welcome message, and custom primary/secondary colors.
- Click on **Kerala Backwaters Escape**.
- View the day-by-day itinerary timeline.
- Click **Book This Trip Now**.
- Select 2 passengers, fill contact details, choose **Pay 30% Deposit Now**, and click **Confirm & Process Booking**.
- Show the **Booking Confirmed** receipt screen with reference code `BK-KER-XXXX`.

### Step 5: Admin Ledger & Real-Time Seat Updates (2 Mins)
- Return to Admin Operations Console.
- Open **Bookings** tab to see the new booking recorded with deposit amount paid and remaining balance.
- Open **Dashboard** to show seat availability automatically deducted and notification generated!
