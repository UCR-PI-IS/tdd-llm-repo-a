-- Seed data for CPD-LC-001-001: Learning Components
-- This seed creates learning components in learning spaces for end-to-end testing

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
        Orientation NVARCHAR(10) NOT NULL
    );
END

-- Clear existing data
DELETE FROM dbo.LearningComponent;

-- Seed learning components for space-001 (has 2 components)
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES 
    ('comp-001', 'space-001', 2.0, 3.0, 1.0, 10.0, 5.0, 0.0, 'North'),
    ('comp-002', 'space-001', 1.5, 2.5, 1.0, 15.0, 8.0, 0.0, 'South');

-- Seed learning component for space-002 (has 1 component)
INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES 
    ('comp-003', 'space-002', 3.0, 2.0, 1.5, 5.0, 10.0, 0.0, 'East');

-- space-empty intentionally has no components (for testing empty list scenario)
