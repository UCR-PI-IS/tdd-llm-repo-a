-- Seed data for CPD-LC-001-009: Automatic ID generation for Learning Components
-- This script creates sample learning components to test the auto-generated ID feature

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

-- Insert sample learning components with explicit IDs for testing
-- These will be used to verify that the system can handle existing components
-- and generate unique IDs for new ones

-- Clear existing data to ensure clean state
DELETE FROM LearningComponent;

-- Insert sample components
INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES 
    ('COMP-EXISTING-001', 'LS-001', 2.5, 1.5, 0.5, 10.0, 20.0, 0.0, 'North'),
    ('COMP-EXISTING-002', 'LS-001', 3.0, 2.0, 1.0, 15.0, 25.0, 0.0, 'South'),
    ('COMP-EXISTING-003', 'LS-002', 1.5, 1.0, 0.5, 5.0, 10.0, 0.0, 'East');
