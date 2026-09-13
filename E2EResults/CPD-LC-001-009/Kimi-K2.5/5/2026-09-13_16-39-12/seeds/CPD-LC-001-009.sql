-- Seed data for CPD-LC-001-009: Automatic ID generation for Learning Components
-- This seed creates the LearningComponent table and sample data

-- Create LearningComponent table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LearningComponent')
BEGIN
    CREATE TABLE LearningComponent (
        ComponentId NVARCHAR(50) PRIMARY KEY,
        LearningSpaceId NVARCHAR(50) NOT NULL,
        Width FLOAT NOT NULL,
        Height FLOAT NOT NULL,
        Depth FLOAT NOT NULL,
        X FLOAT NOT NULL,
        Y FLOAT NOT NULL,
        Z FLOAT NOT NULL,
        Orientation NVARCHAR(20) NOT NULL,
        CreatedAt DATETIME2 DEFAULT GETDATE()
    );
END

-- Insert sample learning components with explicit IDs
-- These represent existing components that the auto-generation must avoid duplicating
IF NOT EXISTS (SELECT * FROM LearningComponent WHERE ComponentId = 'COMP-EXISTING')
BEGIN
    INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
    VALUES ('COMP-EXISTING', 'LS-001', 1.5, 1.0, 0.5, 10.0, 5.0, 0.0, 'North');
END

IF NOT EXISTS (SELECT * FROM LearningComponent WHERE ComponentId = 'COMP-12345')
BEGIN
    INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
    VALUES ('COMP-12345', 'LS-001', 2.0, 1.5, 0.8, 15.0, 10.0, 0.0, 'East');
END

IF NOT EXISTS (SELECT * FROM LearningComponent WHERE ComponentId = 'COMP-99999')
BEGIN
    INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
    VALUES ('COMP-99999', 'LS-002', 1.2, 0.9, 0.4, 5.0, 3.0, 0.0, 'South');
END
