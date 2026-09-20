-- Seed data for CPD-LC-001-005: Update Whiteboard functionality
-- Creates a learning space and a whiteboard to test the update endpoint

-- Create LearningSpace table if not exists
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
CREATE TABLE LearningSpace (
    LearningSpaceId INT IDENTITY(1,1) PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Width REAL NOT NULL,
    Length REAL NOT NULL
);

-- Create LearningComponent table (TPH - Table Per Hierarchy) if not exists
IF OBJECT_ID('dbo.LearningComponent', 'U') IS NULL
CREATE TABLE LearningComponent (
    ComponentId NVARCHAR(50) PRIMARY KEY,
    LearningSpaceId NVARCHAR(50) NOT NULL,
    Width REAL NOT NULL,
    Height REAL NOT NULL,
    Depth REAL NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL,
    Orientation NVARCHAR(20) NOT NULL,
    Discriminator NVARCHAR(50) NOT NULL,
    MarkerColor NVARCHAR(50) NULL
);

-- Enable IDENTITY_INSERT for LearningSpace
SET IDENTITY_INSERT LearningSpace ON;

-- Insert a learning space (ID 103 for IF-0103)
INSERT INTO LearningSpace (LearningSpaceId, Type, Height, Width, Length)
VALUES (103, 'Classroom', 3.0, 10.0, 10.0);

-- Disable IDENTITY_INSERT
SET IDENTITY_INSERT LearningSpace OFF;

-- Insert a whiteboard (WB-001) in learning space IF-0103
INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, Discriminator, MarkerColor)
VALUES ('WB-001', '103', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0, 'North', 'Whiteboard', 'Blue');

-- Insert another whiteboard (WB-002) for overlap testing
INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, Discriminator, MarkerColor)
VALUES ('WB-002', '103', 2.0, 1.5, 0.1, 5.0, 0.0, 5.0, 'South', 'Whiteboard', 'Green');
