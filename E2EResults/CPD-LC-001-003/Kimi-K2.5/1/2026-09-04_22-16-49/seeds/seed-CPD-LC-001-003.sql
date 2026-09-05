-- Seed data for CPD-LC-001-003: Create whiteboard in a learning space
-- This seed creates a learning space first, then whiteboards can be created via the API

-- Create LearningSpace table if it doesn't exist (for reference)
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
BEGIN
    CREATE TABLE LearningSpace (
        LearningSpaceId INT IDENTITY(1,1) PRIMARY KEY,
        Type NVARCHAR(50) NOT NULL,
        Height REAL NOT NULL,
        Width REAL NOT NULL,
        Length REAL NOT NULL
    );
END

-- Create Whiteboard table if it doesn't exist
IF OBJECT_ID('dbo.Whiteboard', 'U') IS NULL
BEGIN
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
END

-- Insert sample learning spaces
-- Note: LearningSpace uses IDENTITY, so we need to use the generated IDs
-- Insert a classroom with dimensions 5m x 5m x 3m (Width x Length x Height)
INSERT INTO LearningSpace (Type, Height, Width, Length)
VALUES ('Classroom', 3.0, 5.0, 5.0);

-- Insert a laboratory with dimensions 10m x 8m x 4m
INSERT INTO LearningSpace (Type, Height, Width, Length)
VALUES ('Laboratory', 4.0, 10.0, 8.0);

-- Insert an auditorium with dimensions 15m x 12m x 5m
INSERT INTO LearningSpace (Type, Height, Width, Length)
VALUES ('Auditorium', 5.0, 15.0, 12.0);

-- Insert sample whiteboards
-- Whiteboard 1: Small whiteboard in classroom (LearningSpaceId = 1)
INSERT INTO Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-001', '1', 2.0, 1.5, 0.5, 1.0, 1.0, 0.0, 'North', 'Blue');

-- Whiteboard 2: Large whiteboard in laboratory (LearningSpaceId = 2)
INSERT INTO Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-002', '2', 4.0, 2.0, 0.5, 2.0, 1.0, 0.0, 'South', 'Red');

-- Whiteboard 3: Medium whiteboard in auditorium (LearningSpaceId = 3)
INSERT INTO Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('WB-003', '3', 3.0, 2.0, 0.5, 1.0, 1.0, 0.0, 'East', 'Green');
