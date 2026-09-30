-- Seed data for PQL-AE-001-001: Add Building
-- Creates the Building table if it does not exist and inserts sample data

IF OBJECT_ID('dbo.Building', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Building (
        InternalId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
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
GO

INSERT INTO dbo.Building (Name, Color, Height, Length, Width, X, Y, Z, AreaId)
VALUES ('Engineering Building', 'Red', 20.5, 50.0, 30.0, 100.0, 200.0, 0.0, 1);

INSERT INTO dbo.Building (Name, Color, Height, Length, Width, X, Y, Z, AreaId)
VALUES ('Science Hall', 'Blue', 15.0, 40.0, 25.0, 150.0, 250.0, 0.0, 1);

INSERT INTO dbo.Building (Name, Color, Height, Length, Width, X, Y, Z, AreaId)
VALUES ('Library Wing', 'Green', 12.0, 35.0, 20.0, 200.0, 300.0, 0.0, 2);
GO
