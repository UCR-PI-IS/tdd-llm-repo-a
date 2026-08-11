-- Seed data for LearningComponent entity
-- Story: CPD-LC-001-001 - List learning components in a learning space

-- Create LearningComponent table if it doesn't exist
IF OBJECT_ID('dbo.LearningComponent', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LearningComponent (
        ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
        LearningSpaceId NVARCHAR(50) NOT NULL,
        Width REAL NOT NULL,
        Height REAL NOT NULL,
        Depth REAL NOT NULL,
        X REAL NOT NULL,
        Y REAL NOT NULL,
        Z REAL NOT NULL,
        Orientation NVARCHAR(20) NOT NULL
    );
END
GO

-- Insert sample learning components for learning space IF-0103
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES
    ('LC-001', 'IF-0103', 2.5, 1.5, 0.5, 10.0, 20.0, 0.0, 'North'),
    ('LC-002', 'IF-0103', 3.0, 2.0, 1.0, 15.0, 25.0, 0.0, 'South'),
    ('LC-003', 'IF-0103', 1.8, 1.2, 0.3, 5.0, 10.0, 0.0, 'East');

-- Insert components for other learning spaces to test filtering
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES
    ('LC-004', 'IF-0104', 4.0, 3.0, 0.8, 12.0, 18.0, 0.0, 'West'),
    ('LC-005', 'IF-0105', 2.2, 1.6, 0.4, 8.0, 15.0, 0.0, 'North');
GO
