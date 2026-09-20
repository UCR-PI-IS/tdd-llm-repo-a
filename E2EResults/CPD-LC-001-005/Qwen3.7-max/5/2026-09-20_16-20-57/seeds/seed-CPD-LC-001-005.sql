-- Seed data for story CPD-LC-001-005: Update a whiteboard
-- Creates tables if they don't exist and inserts sample whiteboard data
-- EF Core uses TPT (Table Per Type) inheritance: base table LearningComponent + derived table Whiteboard

IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE dbo.LearningSpace (
    LearningSpaceId INT NOT NULL PRIMARY KEY,
    Type NVARCHAR(50),
    Height REAL,
    Width REAL,
    Length REAL
);

IF OBJECT_ID('dbo.LearningComponent','U') IS NULL
CREATE TABLE dbo.LearningComponent (
    ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
    LearningSpaceId NVARCHAR(50),
    Width REAL,
    Height REAL,
    Depth REAL,
    X REAL,
    Y REAL,
    Z REAL,
    Orientation NVARCHAR(20),
    Discriminator NVARCHAR(50) NOT NULL DEFAULT 'LearningComponent'
);

IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
CREATE TABLE dbo.Whiteboard (
    ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
    MarkerColor NVARCHAR(50),
    CONSTRAINT FK_Whiteboard_LearningComponent FOREIGN KEY (ComponentId) REFERENCES dbo.LearningComponent(ComponentId)
);

-- Insert a learning space (ID=1 for reference)
IF NOT EXISTS (SELECT 1 FROM dbo.LearningSpace WHERE LearningSpaceId = 1)
INSERT INTO dbo.LearningSpace (LearningSpaceId, Type, Height, Width, Length)
VALUES (1, 'Classroom', 3.0, 10.0, 10.0);

-- Insert 3 whiteboards: base row in LearningComponent + derived row in Whiteboard
-- Valid orientations for Whiteboard: South, East, West
-- Valid marker colors: Red, Blue, Green, Black, White, Yellow, Orange, Purple, Brown, Pink

-- Whiteboard WB-001
IF NOT EXISTS (SELECT 1 FROM dbo.LearningComponent WHERE ComponentId = 'WB-001')
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, Discriminator)
VALUES ('WB-001', 'IF-0103', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0, 'South', 'Whiteboard');
IF NOT EXISTS (SELECT 1 FROM dbo.Whiteboard WHERE ComponentId = 'WB-001')
INSERT INTO dbo.Whiteboard (ComponentId, MarkerColor)
VALUES ('WB-001', 'Blue');

-- Whiteboard WB-002
IF NOT EXISTS (SELECT 1 FROM dbo.LearningComponent WHERE ComponentId = 'WB-002')
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, Discriminator)
VALUES ('WB-002', 'IF-0103', 2.0, 1.5, 0.1, 5.0, 0.0, 5.0, 'East', 'Whiteboard');
IF NOT EXISTS (SELECT 1 FROM dbo.Whiteboard WHERE ComponentId = 'WB-002')
INSERT INTO dbo.Whiteboard (ComponentId, MarkerColor)
VALUES ('WB-002', 'Green');

-- Whiteboard WB-003
IF NOT EXISTS (SELECT 1 FROM dbo.LearningComponent WHERE ComponentId = 'WB-003')
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, Discriminator)
VALUES ('WB-003', 'IF-0103', 1.5, 1.0, 0.1, 8.0, 0.0, 8.0, 'West', 'Whiteboard');
IF NOT EXISTS (SELECT 1 FROM dbo.Whiteboard WHERE ComponentId = 'WB-003')
INSERT INTO dbo.Whiteboard (ComponentId, MarkerColor)
VALUES ('WB-003', 'Red');
