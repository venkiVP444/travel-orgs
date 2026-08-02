USE [TravelOrgOS_Dev];
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Trips]') AND name = 'ViaRoute')
BEGIN
    ALTER TABLE [dbo].[Trips] ADD [ViaRoute] NVARCHAR(250) NULL;
    PRINT 'Added ViaRoute column to Trips table.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Trips]') AND name = 'DriverName')
BEGIN
    ALTER TABLE [dbo].[Trips] ADD [DriverName] NVARCHAR(150) NULL;
    PRINT 'Added DriverName column to Trips table.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Trips]') AND name = 'EstimatedCost')
BEGIN
    ALTER TABLE [dbo].[Trips] ADD [EstimatedCost] DECIMAL(18,2) NOT NULL DEFAULT 0;
    PRINT 'Added EstimatedCost column to Trips table.';
END
GO
