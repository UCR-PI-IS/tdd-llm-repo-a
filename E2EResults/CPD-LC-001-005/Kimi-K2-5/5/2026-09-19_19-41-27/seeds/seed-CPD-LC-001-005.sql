-- Seed data for CPD-LC-001-005: Update Whiteboard functionality
-- Creates a learning space and a whiteboard to test the update endpoint

-- Create LearningSpace table if not exists
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
CREATE TABLE LearningSpace (
    LearningSpaceId INT IDENTITY(1,1) PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height FLOAT NOT NULL,
    Width FLOAT NOT NULL,
    Length FLOAT NOT NULL
);

-- Create Whiteboard table if not exists
IF OBJECT_ID('dbo.Whiteboard', 'U') IS NULL
CREATE TABLE Whiteboard (
    ComponentId NVARCHAR(50) PRIMARY KEY,
    LearningSpaceId NVARCHAR(50) NOT NULL,
    Width FLOAT NOT NULL,
    Height FLOAT NOT NULL,
    Depth FLOAT NOT NULL,
    X FLOAT NOT NULL,
    Y FLOAT NOT NULL,
    Z FLOAT NOT NULL,
    Orientation NVARCHAR(20) NOT NULL,
    MarkerColor NVARCHAR(50) NOT NULL
);

-- Insert a learning space (IF-0103)
INSERT INTO LearningSpace (LearningSpaceId, Type, Height, Width, Length)
VALUES (103, 'Classroom', 3.0, 10.0, 10.0);

-- Insert a whiteboard (WB-001) in learning space IF-0103
INSERT INTO Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-001', 'IF-0103', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0, 'North', 'Blue');

-- Insert another whiteboard (WB-002) for overlap testing
INSERT INTO Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-002', 'IF-0103', 2.0, 1.5, 0.1, 5.0, 0.0, 5.0, 'South', 'Green');
