# TravelOrgOS - Travel Agency Onboarding Guide

This document describes how a new travel operator gets onboarded onto the **TravelOrgOS** SaaS platform.

---

## Onboarding Sequence

### Step 1: Account Provisioning
A new tenant organization is created in the system (slug is generated, e.g. `/portal/south-holidays`). An Organization Owner user account is generated and linked.

### Step 2: Configure Legal & Branding settings
Log in as the Owner and navigate to **Branding & Settings**:
- Configure Legal Business Name.
- Specify **GSTIN** (e.g. `29AAAAA1111A1Z1`).
- Specify **Registered State** (this dictates local CGST/SGST vs. interstate IGST splits calculations).
- Set primary and secondary brand colors (hex color pickers).
- Upload the company logo URL.
- Write the traveller portal welcome message.

### Step 3: Invite Team Members
Navigate to **Team Management**:
- Enter Name and Email.
- Assign their operational access role:
  - **Org Admin**: Manage trips, pricing, and travellers CRM.
  - **Trip Coordinator**: Manage guide and vehicle schedules.
  - **Finance User**: View invoices and register cash balances.
- Send the invitation. The user receives a stateless onboarding link to set their password.

### Step 4: Configure Fleet & Certified Guides
- Navigate to **Guides** and add certified guides (languages, licenses).
- Navigate to **Vehicles** (SSMS or settings) and add fleet vehicles (registrations, capacity).
- Navigate to **Vendors** and add partner operators (hotels, restaurants).
- Once configured, you are ready to construct and publish B2B travel packages!
