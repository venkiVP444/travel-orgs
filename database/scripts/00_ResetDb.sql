USE [master];
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'TravelOrgOS_Dev')
BEGIN
    ALTER DATABASE [TravelOrgOS_Dev] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [TravelOrgOS_Dev];
    PRINT 'Dropped existing database TravelOrgOS_Dev.';
END
GO
