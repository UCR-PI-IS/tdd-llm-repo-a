-- Seed data for CPD-LC-001-001: Learning Components
-- This script seeds sample learning components for end-to-end testing

-- First, ensure the LearningSpace table exists and has a test space
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NOT NULL
BEGIN
    -- Insert test learning spaces if they don't exist
    IF NOT EXISTS (SELECT 1 FROM LearningSpace WHERE id = 'LS-001')
        INSERT INTO LearningSpace (id, type, height, width, length) VALUES ('LS-001', 'Classroom', 3.0, 10.0, 8.0);
    
    IF NOT EXISTS (SELECT 1 FROM LearningSpace WHERE id = 'LS-002')
        INSERT INTO LearningSpace (id, type, height, width, length) VALUES ('LS-002', 'Laboratory', 3.5, 12.0, 10.0);
END

-- Seed LearningComponent table
IF OBJECT_ID('dbo.LearningComponent', 'U') IS NOT NULL
BEGIN
    -- Insert test learning components for LS-001
    IF NOT EXISTS (SELECT 1 FROM LearningComponent WHERE ComponentId = 'COMP-001')
        INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
        VALUES ('COMP-001', 'LS-001', 2.0, 1.5, 1.0, 0.0, 0.0, 0.0, 'North');
    
    IF NOT EXISTS (SELECT 1 FROM LearningComponent WHERE ComponentId = 'COMP-002')
        INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
        VALUES ('COMP-002', 'LS-001', 3.0, 2.0, 1.5, 5.0, 0.0, 2.0, 'East');
    
    IF NOT EXISTS (SELECT 1 FROM LearningComponent WHERE ComponentId = 'COMP-003')
        INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
        VALUES ('COMP-003', 'LS-001', 1.5, 1.0, 0.8, 8.0, 0.0, 5.0, 'South');
END
