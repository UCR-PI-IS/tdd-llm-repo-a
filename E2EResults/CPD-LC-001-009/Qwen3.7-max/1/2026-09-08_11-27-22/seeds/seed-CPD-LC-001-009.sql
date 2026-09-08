-- Seed data for CPD-LC-001-009: Automatic ID generation for Learning Components
-- Table: LearningComponent (from LearningComponentEntityConfiguration.cs)

-- Ensure table exists
IF OBJECT_ID('dbo.LearningComponent', 'U') IS NULL
BEGIN
    CREATE TABLE LearningComponent (
        ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
        LearningSpaceId NVARCHAR(50) NOT NULL,
        Width FLOAT NOT NULL,
        Height FLOAT NOT NULL,
        Depth FLOAT NOT NULL,
        X FLOAT NOT NULL,
        Y FLOAT NOT NULL,
        Z FLOAT NOT NULL,
        Orientation NVARCHAR(20) NOT NULL
    );
END

-- Insert seed data with valid component IDs
-- These components will be used to test the GET endpoint and verify ID uniqueness
INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES 
    ('COMP-SEED-001', 'LS-001', 2.5, 1.5, 0.5, 10.0, 20.0, 0.0, 'North'),
    ('COMP-SEED-002', 'LS-001', 3.0, 2.0, 1.0, 15.0, 25.0, 0.0, 'South'),
    ('COMP-SEED-003', 'LS-002', 1.5, 1.0, 0.5, 5.0, 10.0, 0.0, 'East');
