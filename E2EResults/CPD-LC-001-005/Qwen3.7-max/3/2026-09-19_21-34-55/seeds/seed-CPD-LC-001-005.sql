-- Seed data for story CPD-LC-001-005: Update a whiteboard
-- Creates the Whiteboard table and inserts sample data

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

-- Insert 3 sample whiteboards with valid values
INSERT INTO dbo.Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES
    ('WB-001', 'IF-0103', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0, 'South', 'Blue'),
    ('WB-002', 'IF-0103', 3.0, 2.0, 0.2, 5.0, 0.0, 5.0, 'East', 'Green'),
    ('WB-003', 'IF-0104', 1.5, 1.0, 0.1, 0.0, 0.0, 0.0, 'West', 'Red');
