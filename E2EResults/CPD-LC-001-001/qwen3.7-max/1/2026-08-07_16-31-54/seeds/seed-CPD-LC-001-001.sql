-- Seed data for LearningComponent table
-- Story: CPD-LC-001-001 - List learning components

-- Create table if it doesn't exist
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
        Orientation NVARCHAR(10) NOT NULL
    );
END

-- Insert sample learning components for learning space IF-0103
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES
    ('COMP-001', 'IF-0103', 2.0, 1.5, 0.5, 1.0, 2.0, 0.0, 'North'),
    ('COMP-002', 'IF-0103', 3.0, 2.0, 1.0, 3.0, 4.0, 0.0, 'South'),
    ('COMP-003', 'IF-0103', 1.5, 1.0, 0.3, 5.0, 6.0, 0.5, 'East');

-- Insert components for other learning spaces to test filtering
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES
    ('COMP-004', 'IF-0104', 2.5, 1.8, 0.6, 2.0, 3.0, 0.0, 'West'),
    ('COMP-005', 'IF-0105', 4.0, 2.5, 1.2, 4.0, 5.0, 1.0, 'North');
