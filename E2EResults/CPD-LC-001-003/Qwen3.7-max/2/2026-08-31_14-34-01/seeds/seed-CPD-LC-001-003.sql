-- Seed data for CPD-LC-001-003: Create whiteboard in a learning space
-- This script seeds the LearningSpace and Whiteboard tables with sample data

-- Ensure LearningSpace table exists
IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE LearningSpace (
    LearningSpaceId INT IDENTITY(1,1) PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Width REAL NOT NULL,
    Length REAL NOT NULL
);

-- Ensure Whiteboard table exists
IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
CREATE TABLE Whiteboard (
    ComponentId NVARCHAR(50) PRIMARY KEY,
    LearningSpaceId NVARCHAR(50) NOT NULL,
    Width REAL NOT NULL,
    Height REAL NOT NULL,
    Depth REAL NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL,
    Orientation NVARCHAR(20) NOT NULL,
    MarkerColor NVARCHAR(50) NOT NULL
);

-- Seed LearningSpace data (3 rows)
INSERT INTO LearningSpace (Type, Height, Width, Length) VALUES
('Classroom', 3.0, 8.0, 10.0),
('Laboratory', 4.0, 12.0, 15.0),
('Auditorium', 6.0, 20.0, 30.0);

-- Seed Whiteboard data (3 rows with valid values)
INSERT INTO Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor) VALUES
('WB-001', 'IF-0103', 2.5, 1.5, 0.5, 1.0, 0.5, 1.0, 'South', 'Blue'),
('WB-002', 'IF-0104', 3.0, 2.0, 0.3, 0.0, 0.0, 0.0, 'East', 'Red'),
('WB-003', 'IF-0105', 4.0, 2.5, 0.4, 2.0, 1.0, 1.5, 'West', 'Green');
