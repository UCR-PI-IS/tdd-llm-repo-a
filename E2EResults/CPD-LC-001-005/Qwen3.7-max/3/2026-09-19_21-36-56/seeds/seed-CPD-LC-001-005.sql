-- Seed data for story CPD-LC-001-005: Update a whiteboard
-- Creates the LearningSpace and Whiteboard tables and inserts sample data

IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
CREATE TABLE dbo.LearningSpace (
    LearningSpaceId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Width REAL NOT NULL,
    Length REAL NOT NULL
);

IF OBJECT_ID('dbo.Whiteboard', 'U') IS NULL
CREATE TABLE dbo.Whiteboard (
    ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
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

-- Insert sample learning spaces
INSERT INTO dbo.LearningSpace (Type, Height, Width, Length)
VALUES
    ('Classroom', 3.0, 10.0, 10.0),
    ('Laboratory', 3.5, 15.0, 20.0);

-- Insert 3 sample whiteboards with valid values
INSERT INTO dbo.Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES
    ('WB-001', 'IF-0103', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0, 'South', 'Blue'),
    ('WB-002', 'IF-0103', 3.0, 2.0, 0.2, 5.0, 0.0, 5.0, 'East', 'Green'),
    ('WB-003', 'IF-0104', 1.5, 1.0, 0.1, 0.0, 0.0, 0.0, 'West', 'Red');
