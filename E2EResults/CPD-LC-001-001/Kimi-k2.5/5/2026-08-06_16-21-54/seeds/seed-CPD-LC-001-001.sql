-- Seed data for CPD-LC-001-001: Learning Components
-- Create table if not exists (in case it's not already created by the database project)

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

-- Clear existing data for this test
DELETE FROM dbo.LearningComponent WHERE LearningSpaceId IN ('ls-001', 'ls-empty', 'ls-nonexistent');

-- Insert sample learning components for learning space 'ls-001'
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES 
    ('comp-001', 'ls-001', 2.5, 3.0, 2.0, 10.0, 5.0, 0.0, 'North'),
    ('comp-002', 'ls-001', 1.5, 2.0, 1.5, 15.0, 8.0, 0.0, 'South');

-- Insert sample learning components for other learning spaces
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES 
    ('comp-003', 'ls-002', 3.0, 2.5, 2.0, 5.0, 5.0, 0.0, 'East');
