-- Seed SQL for story PQL-AE-001-002 (Edit Building Information)
-- Creates the Building table and inserts sample data

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

-- Enable explicit identity inserts
SET IDENTITY_INSERT dbo.Building ON;

-- Insert 3 sample buildings with valid data
INSERT INTO dbo.Building (InternalId, Name, Color, Height, Length, Width, X, Y, Z, AreaId)
VALUES (1, 'Engineering Building', 'Blue', 10.5, 20.0, 15.0, 100.0, 0.0, 200.0, 1);

INSERT INTO dbo.Building (InternalId, Name, Color, Height, Length, Width, X, Y, Z, AreaId)
VALUES (2, 'Science Library', 'Red', 15.0, 30.0, 25.0, 50.0, 10.0, 100.0, 1);

INSERT INTO dbo.Building (InternalId, Name, Color, Height, Length, Width, X, Y, Z, AreaId)
VALUES (3, 'Student Center', 'Green', 8.0, 40.0, 35.0, 200.0, 5.0, 150.0, 2);

-- Disable explicit identity inserts
SET IDENTITY_INSERT dbo.Building OFF;
