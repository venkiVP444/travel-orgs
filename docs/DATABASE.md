# TravelOrgOS Database Documentation

> [!CAUTION]
> **STRICT DATABASE SAFETY RULE**:
> TravelOrgOS ONLY connects to:
> - Server: `(localdb)\MSSQLLocalDB`
> - Database: `TravelOrgOS_Dev`
> 
> Office database `10.50.6.6` / `dbEMMA_Restore` is STRICTLY FORBIDDEN and blocked programmatically by `DatabaseSafetyChecker`.

## Connection Strings

### Application connection string:
```
Server=(localdb)\MSSQLLocalDB;Database=TravelOrgOS_Dev;Trusted_Connection=True;TrustServerCertificate=True;
```

### Full SSMS connection string:
```
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TravelOrgOS_Dev;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;Application Name="SQL Server Management Studio";Command Timeout=0;
```

## Database Tables (17 Domain Tables)
1. `Organizations`
2. `OrganizationUsers`
3. `Travellers`
4. `Trips`
5. `TripItineraryDays`
6. `Hotels`
7. `TripHotels`
8. `Vehicles`
9. `TripVehicles`
10. `Vendors`
11. `TripVendors`
12. `TripMeals`
13. `Bookings`
14. `BookingTravellers`
15. `Payments`
16. `Notifications`
17. `AuditLogs`
