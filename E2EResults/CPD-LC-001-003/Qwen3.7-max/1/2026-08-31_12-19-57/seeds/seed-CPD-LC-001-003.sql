-- Seed SQL for CPD-LC-001-003: Create whiteboard in a learning space
-- Create LearningSpace table if it does not exist
IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE dbo.LearningSpace (
    LearningSpaceId INT NOT NULL PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Width REAL NOT NULL,
    Length REAL NOT NULL
);

-- Create Whiteboard table if it does not exist
IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
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

-- Seed LearningSpace rows (IDs must be ints since SqlLearningSpaceReadRepository parses string->int)
IF NOT EXISTS (SELECT 1 FROM dbo.LearningSpace WHERE LearningSpaceId = 1)
INSERT INTO dbo.LearningSpace (LearningSpaceId, Type, Height, Width, Length)
VALUES (1, 'Classroom', 5.0, 10.0, 12.0);

IF NOT EXISTS (SELECT 1 FROM dbo.LearningSpace WHERE LearningSpaceId = 2)
INSERT INTO dbo.LearningSpace (LearningSpaceId, Type, Height, Width, Length)
VALUES (2, 'Laboratory', 4.0, 8.0, 10.0);

IF NOT EXISTS (SELECT 1 FROM dbo.LearningSpace WHERE LearningSpaceId = 3)
INSERT INTO dbo.LearningSpace (LearningSpaceId, Type, Height, Width, Length)
VALUES (3, 'Auditorium', 8.0, 20.0, 25.0);

-- Seed Whiteboard rows
IF NOT EXISTS (SELECT 1 FROM dbo.Whiteboard WHERE ComponentId = 'WB-001')
INSERT INTO dbo.Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-001', '1', 2.0, 1.5, 0.5, 1.0, 1.0, 0.0, 'South', 'Blue');

IF NOT EXISTS (SELECT 1 FROM dbo.Whiteboard WHERE ComponentId = 'WB-002')
INSERT INTO dbo.Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-002', '2', 3.0, 2.0, 0.3, 0.0, 0.0, 0.0, 'East', 'Red');

IF NOT EXISTS (SELECT 1 FROM dbo.Whiteboard WHERE ComponentId = 'WB-003')
INSERT INTO dbo.Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-003', '3', 5.0, 3.0, 0.4, 2.0, 1.0, 0.0, 'West', 'Green');
