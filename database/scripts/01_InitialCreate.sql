-- ============================================================
-- TravelOrgOS Database Creation & Initial Schema Script
-- Strictly targets: (localdb)\MSSQLLocalDB
-- Database: TravelOrgOS_Dev
-- ============================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'TravelOrgOS_Dev')
BEGIN
    CREATE DATABASE [TravelOrgOS_Dev];
END
GO

USE [TravelOrgOS_Dev];
GO

-- 1. Organizations Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Organizations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Organizations] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [Slug] NVARCHAR(100) NOT NULL,
        [LegalName] NVARCHAR(200) NULL,
        [LogoUrl] NVARCHAR(500) NULL,
        [PrimaryColor] NVARCHAR(30) NOT NULL DEFAULT '#1E88E5',
        [SecondaryColor] NVARCHAR(30) NOT NULL DEFAULT '#0D47A1',
        [WelcomeMessage] NVARCHAR(500) NULL,
        [Email] NVARCHAR(200) NOT NULL,
        [Phone] NVARCHAR(50) NOT NULL,
        [Website] NVARCHAR(200) NULL,
        [Address] NVARCHAR(500) NULL,
        [City] NVARCHAR(100) NULL,
        [Country] NVARCHAR(100) NULL,
        [BusinessHours] NVARCHAR(200) NULL,
        [Description] NVARCHAR(MAX) NULL,
        [FacebookUrl] NVARCHAR(200) NULL,
        [InstagramUrl] NVARCHAR(200) NULL,
        [LinkedInUrl] NVARCHAR(200) NULL,
        [WhatsAppNumber] NVARCHAR(50) NULL,
        [Status] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL
    );

    CREATE UNIQUE INDEX [IX_Organizations_Slug] ON [dbo].[Organizations]([Slug]);
END
GO

-- 2. OrganizationUsers Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrganizationUsers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OrganizationUsers] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NULL,
        [FullName] NVARCHAR(150) NOT NULL,
        [Email] NVARCHAR(200) NOT NULL,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [Role] INT NOT NULL,
        [Status] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastLoginAt] DATETIME2 NULL,
        CONSTRAINT [FK_OrganizationUsers_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations]([Id])
    );

    CREATE UNIQUE INDEX [IX_OrganizationUsers_Email] ON [dbo].[OrganizationUsers]([Email]);
END
GO

-- 3. Travellers Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Travellers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Travellers] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(200) NOT NULL,
        [MobileNumber] NVARCHAR(50) NOT NULL,
        [DateOfBirth] DATETIME2 NULL,
        [Gender] NVARCHAR(20) NULL,
        [Nationality] NVARCHAR(100) NULL,
        [PassportNumber] NVARCHAR(50) NULL,
        [PassportExpiry] DATETIME2 NULL,
        [EmergencyContactName] NVARCHAR(150) NULL,
        [EmergencyContactNumber] NVARCHAR(50) NULL,
        [Address] NVARCHAR(500) NULL,
        [City] NVARCHAR(100) NULL,
        [Country] NVARCHAR(100) NULL,
        [Notes] NVARCHAR(MAX) NULL,
        [Status] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [FK_Travellers_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations]([Id])
    );
END
GO

-- 4. Trips Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Trips]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Trips] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [TripCode] NVARCHAR(50) NOT NULL,
        [TripName] NVARCHAR(200) NOT NULL,
        [ShortDescription] NVARCHAR(500) NOT NULL,
        [Description] NVARCHAR(MAX) NOT NULL,
        [Destination] NVARCHAR(200) NOT NULL,
        [StartLocation] NVARCHAR(200) NOT NULL,
        [EndLocation] NVARCHAR(200) NOT NULL,
        [StartDate] DATETIME2 NOT NULL,
        [EndDate] DATETIME2 NOT NULL,
        [DurationDays] INT NOT NULL,
        [DurationNights] INT NOT NULL,
        [TripType] INT NOT NULL,
        [Status] INT NOT NULL DEFAULT 1,
        [Visibility] INT NOT NULL DEFAULT 3,
        [CoverImageUrl] NVARCHAR(500) NOT NULL,
        [BasePrice] DECIMAL(18,2) NOT NULL,
        [Currency] NVARCHAR(10) NOT NULL DEFAULT 'USD',
        [TotalCapacity] INT NOT NULL,
        [AvailableSeats] INT NOT NULL,
        [MinimumTravellers] INT NOT NULL DEFAULT 1,
        [MaximumTravellers] INT NOT NULL,
        [HostGuide] NVARCHAR(150) NULL,
        [ViaRoute] NVARCHAR(250) NULL,
        [DriverName] NVARCHAR(150) NULL,
        [EstimatedCost] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [ContactPerson] NVARCHAR(150) NULL,
        [ContactNumber] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [PublishedAt] DATETIME2 NULL,
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [FK_Trips_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations]([Id])
    );

    CREATE UNIQUE INDEX [IX_Trips_Org_Code] ON [dbo].[Trips]([OrganizationId], [TripCode]);
END
GO

-- 5. TripItineraryDays Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TripItineraryDays]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TripItineraryDays] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [TripId] UNIQUEIDENTIFIER NOT NULL,
        [DayNumber] INT NOT NULL,
        [Date] DATETIME2 NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NOT NULL,
        [Location] NVARCHAR(200) NULL,
        [Activities] NVARCHAR(1000) NULL,
        [StartTime] NVARCHAR(20) NULL,
        [EndTime] NVARCHAR(20) NULL,
        [Notes] NVARCHAR(MAX) NULL,
        CONSTRAINT [FK_TripItineraryDays_Trips] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips]([Id]) ON DELETE CASCADE
    );
END
GO

-- 6. Hotels Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Hotels]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Hotels] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [HotelName] NVARCHAR(200) NOT NULL,
        [Location] NVARCHAR(200) NOT NULL,
        [Address] NVARCHAR(500) NULL,
        [ContactNumber] NVARCHAR(50) NULL,
        [ContactPerson] NVARCHAR(150) NULL,
        [DefaultRoomType] NVARCHAR(100) NULL,
        [Notes] NVARCHAR(MAX) NULL,
        [Status] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Hotels_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations]([Id])
    );
END
GO

-- 7. TripHotels Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TripHotels]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TripHotels] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [TripId] UNIQUEIDENTIFIER NOT NULL,
        [HotelId] UNIQUEIDENTIFIER NOT NULL,
        [RoomType] NVARCHAR(100) NOT NULL,
        [CheckIn] DATETIME2 NOT NULL,
        [CheckOut] DATETIME2 NOT NULL,
        [RoomCount] INT NOT NULL DEFAULT 1,
        [Notes] NVARCHAR(MAX) NULL,
        CONSTRAINT [FK_TripHotels_Trips] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips]([Id]),
        CONSTRAINT [FK_TripHotels_Hotels] FOREIGN KEY ([HotelId]) REFERENCES [dbo].[Hotels]([Id])
    );
END
GO

-- 8. Vehicles Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Vehicles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Vehicles] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [VehicleName] NVARCHAR(200) NOT NULL,
        [VehicleType] NVARCHAR(50) NOT NULL,
        [RegistrationNumber] NVARCHAR(50) NULL,
        [Capacity] INT NOT NULL DEFAULT 20,
        [DriverName] NVARCHAR(150) NULL,
        [DriverPhone] NVARCHAR(50) NULL,
        [VendorName] NVARCHAR(150) NULL,
        [Status] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Vehicles_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations]([Id])
    );
END
GO

-- 9. TripVehicles Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TripVehicles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TripVehicles] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [TripId] UNIQUEIDENTIFIER NOT NULL,
        [VehicleId] UNIQUEIDENTIFIER NOT NULL,
        [Notes] NVARCHAR(MAX) NULL,
        CONSTRAINT [FK_TripVehicles_Trips] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips]([Id]),
        CONSTRAINT [FK_TripVehicles_Vehicles] FOREIGN KEY ([VehicleId]) REFERENCES [dbo].[Vehicles]([Id])
    );
END
GO

-- 10. Vendors Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Vendors]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Vendors] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [VendorName] NVARCHAR(200) NOT NULL,
        [VendorType] INT NOT NULL,
        [ContactPerson] NVARCHAR(150) NULL,
        [Phone] NVARCHAR(50) NULL,
        [Email] NVARCHAR(200) NULL,
        [Address] NVARCHAR(500) NULL,
        [Notes] NVARCHAR(MAX) NULL,
        [Status] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Vendors_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations]([Id])
    );
END
GO

-- 11. TripVendors Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TripVendors]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TripVendors] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [TripId] UNIQUEIDENTIFIER NOT NULL,
        [VendorId] UNIQUEIDENTIFIER NOT NULL,
        [ContractAmount] DECIMAL(18,2) NOT NULL,
        [ServiceDescription] NVARCHAR(MAX) NULL,
        CONSTRAINT [FK_TripVendors_Trips] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips]([Id]),
        CONSTRAINT [FK_TripVendors_Vendors] FOREIGN KEY ([VendorId]) REFERENCES [dbo].[Vendors]([Id])
    );
END
GO

-- 12. TripMeals Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TripMeals]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TripMeals] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [TripId] UNIQUEIDENTIFIER NOT NULL,
        [MealType] INT NOT NULL,
        [MealOption] INT NOT NULL,
        [Description] NVARCHAR(200) NULL,
        [DietaryOptions] NVARCHAR(200) NULL,
        CONSTRAINT [FK_TripMeals_Trips] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips]([Id])
    );
END
GO

-- 13. Bookings Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Bookings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Bookings] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [TripId] UNIQUEIDENTIFIER NOT NULL,
        [BookedByUserId] UNIQUEIDENTIFIER NULL,
        [BookingReference] NVARCHAR(50) NOT NULL,
        [BookingDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [NumberOfTravellers] INT NOT NULL DEFAULT 1,
        [TotalAmount] DECIMAL(18,2) NOT NULL,
        [PaidAmount] DECIMAL(18,2) NOT NULL,
        [BalanceAmount] DECIMAL(18,2) NOT NULL,
        [PaymentStatus] INT NOT NULL DEFAULT 1,
        [BookingStatus] INT NOT NULL DEFAULT 1,
        [ContactEmail] NVARCHAR(200) NOT NULL,
        [ContactPhone] NVARCHAR(50) NOT NULL,
        [SpecialRequests] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [FK_Bookings_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations]([Id]),
        CONSTRAINT [FK_Bookings_Trips] FOREIGN KEY ([TripId]) REFERENCES [dbo].[Trips]([Id])
    );

    CREATE UNIQUE INDEX [IX_Bookings_Reference] ON [dbo].[Bookings]([BookingReference]);
END
GO

-- 14. BookingTravellers Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BookingTravellers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BookingTravellers] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [BookingId] UNIQUEIDENTIFIER NOT NULL,
        [TravellerId] UNIQUEIDENTIFIER NOT NULL,
        [RoomPreference] NVARCHAR(50) NOT NULL DEFAULT 'Single',
        [DietaryPreference] NVARCHAR(100) NOT NULL DEFAULT 'Regular',
        CONSTRAINT [FK_BookingTravellers_Bookings] FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Bookings]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BookingTravellers_Travellers] FOREIGN KEY ([TravellerId]) REFERENCES [dbo].[Travellers]([Id])
    );
END
GO

-- 15. Payments Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Payments] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [BookingId] UNIQUEIDENTIFIER NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [PaymentMethod] NVARCHAR(50) NOT NULL,
        [TransactionReference] NVARCHAR(100) NOT NULL,
        [Status] INT NOT NULL DEFAULT 3,
        [PaymentDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [Notes] NVARCHAR(MAX) NULL,
        CONSTRAINT [FK_Payments_Bookings] FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Bookings]([Id]) ON DELETE CASCADE
    );
END
GO

-- 16. Notifications Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Notifications] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [UserId] UNIQUEIDENTIFIER NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [Type] INT NOT NULL,
        [IsRead] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- 17. AuditLogs Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditLogs] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [User] NVARCHAR(100) NOT NULL,
        [Action] NVARCHAR(100) NOT NULL,
        [Entity] NVARCHAR(100) NOT NULL,
        [EntityId] NVARCHAR(100) NULL,
        [Details] NVARCHAR(MAX) NULL,
        [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO
