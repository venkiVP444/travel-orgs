# TravelOrgOS - Environment Configuration

This document specifies the configuration variables and settings required to run **TravelOrgOS** in development and production environments.

---

## 1. Database Connection Strings

Target database connections are resolved from:
- `ConnectionStrings:DefaultConnection`
- **Development**: `"Server=(localdb)\\MSSQLLocalDB;Initial Catalog=TravelOrgOS_Dev;Integrated Security=True;"`
- **Production**: Encrypted Azure SQL or enterprise SQL Server endpoints.

---

## 2. JWT Configuration

Used to sign and validate stateless user session tokens.
- `Jwt:Secret` (Min 256-bit security key).
- `Jwt:Issuer` (e.g. `TravelOrgOS.Api`).
- `Jwt:Audience` (e.g. `TravelOrgOS.Web`).
- `Jwt:ExpiryDays` (Token lifetime, default `7`).

---

## 3. Payment Gateway Webhook secrets

Must be configured to verify webhook events from gateways:
- `PaymentGateway:Stripe:WebhookSecret` (Stripe signing webhook secret).
- `PaymentGateway:Razorpay:KeySecret` (Razorpay signing key secret).

---

## 4. Structured Logs

LogLevel thresholds:
- `Logging:LogLevel:Default` (Default `Information`).
- `Logging:LogLevel:Microsoft.AspNetCore` (Default `Warning`).
