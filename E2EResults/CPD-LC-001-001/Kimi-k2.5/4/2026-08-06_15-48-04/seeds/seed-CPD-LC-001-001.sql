-- Seed data for LearningComponent table for CPD-LC-001-001
-- Create table if not exists
IF OBJECT_ID('dbo.LearningComponent', 'U') IS NULL
BEGIN
    CREATE TABLE [LearningComponent] (
        [ComponentId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [LearningSpaceId] NVARCHAR(50) NOT NULL,
        [Width] REAL NOT NULL,
        [Height] REAL NOT NULL,
        [Depth] REAL NOT NULL,
        [X] REAL NOT NULL,
        [Y] REAL NOT NULL,
        [Z] REAL NOT NULL,
        [Orientation] NVARCHAR(10) NOT NULL
    );
END

-- Clear existing data for test learning space
DELETE FROM [LearningComponent] WHERE [LearningSpaceId] = 'space-001';

-- Insert sample learning components for space-001
INSERT INTO [LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES 
    ('comp-001', 'space-001', 2.5, 3.0, 2.0, 1.0, 0.5, 2.0, 'North'),
    ('comp-002', 'space-001', 1.5, 2.0, 1.5, 3.0, 0.5, 4.0, 'South'),
    ('comp-003', 'space-001', 2.0, 2.5, 1.0, 5.0, 0.5, 2.0, 'East');

-- Insert sample learning components for another space
INSERT INTO [LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES 
    ('comp-004', 'space-002', 3.0, 2.0, 1.5, 1.0, 0.0, 1.0, 'West');
