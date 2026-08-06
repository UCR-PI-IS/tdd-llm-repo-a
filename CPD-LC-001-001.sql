-- Seed data for CPD-LC-001-001 end-to-end testing
-- Learning Space with ID matching the test expectations

-- First ensure LearningSpace table exists and has the test space
IF NOT EXISTS (SELECT * FROM LearningSpace WHERE id = 'SPACE-001')
BEGIN
    INSERT INTO LearningSpace (id, type, height, width, length)
    VALUES ('SPACE-001', 'Classroom', 3.0, 10.0, 8.0);
END

-- Insert learning components for SPACE-001
IF NOT EXISTS (SELECT * FROM LearningComponent WHERE ComponentId = 'COMP-001')
BEGIN
    INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
    VALUES ('COMP-001', 'SPACE-001', 2.0, 1.5, 1.0, 10.0, 5.0, 0.0, 'North');
END

IF NOT EXISTS (SELECT * FROM LearningComponent WHERE ComponentId = 'COMP-002')
BEGIN
    INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
    VALUES ('COMP-002', 'SPACE-001', 1.5, 1.0, 0.8, 15.0, 8.0, 0.0, 'South');
END

-- Insert a learning space with no components for testing empty list scenario
IF NOT EXISTS (SELECT * FROM LearningSpace WHERE id = 'EMPTY-SPACE')
BEGIN
    INSERT INTO LearningSpace (id, type, height, width, length)
    VALUES ('EMPTY-SPACE', 'Laboratory', 3.5, 12.0, 10.0);
END
