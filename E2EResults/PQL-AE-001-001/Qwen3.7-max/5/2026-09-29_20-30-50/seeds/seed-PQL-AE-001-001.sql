-- Seed script for PQL-AE-001-001: Add Building
-- Creates the Building table if it does not exist and inserts sample data

IF OBJECT_ID('dbo.Building', 'U') IS NULL
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

-- Insert sample buildings that satisfy all validation rules
INSERT INTO dbo.Building (Name, Color, Height, Length, Width, X, Y, Z, AreaId)
VALUES
    ('Engineering Building', 'Red', 20.5, 50.0, 30.0, 100.0, 200.0, 0.0, 1),
    ('Science Hall', 'Blue', 15.0, 40.0, 25.0, 150.0, 250.0, 0.0, 2),
    ('Library', 'Green', 12.0, 60.0, 35.0, 300.0, 400.0, 0.0, 3);
