-- Seed data for PQL-AE-001-002: Edit Building Information
-- This script creates sample buildings for end-to-end testing

-- Create Building table if it doesn't exist
IF OBJECT_ID('dbo.Building', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Building (
        InternalId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Color NVARCHAR(50) NOT NULL,
        Height REAL NOT NULL,
        Length REAL NOT NULL,
        Width REAL NOT NULL,
        X REAL NOT NULL,
        Y REAL NOT NULL,
        Z REAL NOT NULL,
        AreaId INT NOT NULL
    );
END

-- Insert sample buildings for testing
-- Building with ID 1 for update testing
IF NOT EXISTS (SELECT 1 FROM dbo.Building WHERE InternalId = 1)
BEGIN
    SET IDENTITY_INSERT dbo.Building ON;
    INSERT INTO dbo.Building (InternalId, Name, Color, Height, Length, Width, X, Y, Z, AreaId)
    VALUES (1, 'Engineering Building', 'Blue', 10.5, 20.0, 15.0, 100.0, 0.0, 200.0, 1);
    SET IDENTITY_INSERT dbo.Building OFF;
END

-- Additional buildings for list testing
IF NOT EXISTS (SELECT 1 FROM dbo.Building WHERE InternalId = 2)
BEGIN
    SET IDENTITY_INSERT dbo.Building ON;
    INSERT INTO dbo.Building (InternalId, Name, Color, Height, Length, Width, X, Y, Z, AreaId)
    VALUES (2, 'Science Building', 'Red', 15.0, 25.0, 20.0, 50.0, 10.0, 100.0, 1);
    SET IDENTITY_INSERT dbo.Building OFF;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Building WHERE InternalId = 3)
BEGIN
    SET IDENTITY_INSERT dbo.Building ON;
    INSERT INTO dbo.Building (InternalId, Name, Color, Height, Length, Width, X, Y, Z, AreaId)
    VALUES (3, 'Library', 'Gray', 12.0, 30.0, 25.0, 150.0, 20.0, 300.0, 1);
    SET IDENTITY_INSERT dbo.Building OFF;
END
