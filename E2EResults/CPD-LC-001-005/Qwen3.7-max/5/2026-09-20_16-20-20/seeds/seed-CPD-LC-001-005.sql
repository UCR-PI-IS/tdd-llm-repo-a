-- Seed data for story CPD-LC-001-005: Update a whiteboard
-- Creates tables if they don't exist and inserts sample whiteboard data

IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE dbo.LearningSpace (
    LearningSpaceId INT NOT NULL PRIMARY KEY,
    Type NVARCHAR(50),
    Height REAL,
    Width REAL,
    Length REAL
);

IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
CREATE TABLE dbo.Whiteboard (
    ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
    LearningSpaceId NVARCHAR(50),
    Width REAL,
    Height REAL,
    Depth REAL,
    X REAL,
    Y REAL,
    Z REAL,
    Orientation NVARCHAR(20),
    MarkerColor NVARCHAR(50)
);

-- Insert a learning space (ID=1 for reference)
IF NOT EXISTS (SELECT 1 FROM dbo.LearningSpace WHERE LearningSpaceId = 1)
INSERT INTO dbo.LearningSpace (LearningSpaceId, Type, Height, Width, Length)
VALUES (1, 'Classroom', 3.0, 10.0, 10.0);

-- Insert 3 whiteboards with valid data
-- Valid orientations for Whiteboard: South, East, West
-- Valid marker colors: Red, Blue, Green, Black, White, Yellow, Orange, Purple, Brown, Pink
IF NOT EXISTS (SELECT 1 FROM dbo.Whiteboard WHERE ComponentId = 'WB-001')
INSERT INTO dbo.Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-001', 'IF-0103', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0, 'South', 'Blue');

IF NOT EXISTS (SELECT 1 FROM dbo.Whiteboard WHERE ComponentId = 'WB-002')
INSERT INTO dbo.Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-002', 'IF-0103', 2.0, 1.5, 0.1, 5.0, 0.0, 5.0, 'East', 'Green');

IF NOT EXISTS (SELECT 1 FROM dbo.Whiteboard WHERE ComponentId = 'WB-003')
INSERT INTO dbo.Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-003', 'IF-0103', 1.5, 1.0, 0.1, 8.0, 0.0, 8.0, 'West', 'Red');
