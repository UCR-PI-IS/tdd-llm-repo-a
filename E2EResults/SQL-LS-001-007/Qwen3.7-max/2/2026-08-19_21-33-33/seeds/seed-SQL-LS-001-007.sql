-- Seed data for LearningSpace table
-- Story: SQL-LS-001-007 (Create Learning Spaces)

-- Create LearningSpace table if it doesn't exist
-- Note: LearningSpaceId is NOT an identity column because the domain entity generates IDs
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
CREATE TABLE [dbo].[LearningSpace] (
    [LearningSpaceId] INT NOT NULL PRIMARY KEY,
    [Type] NVARCHAR(50) NOT NULL,
    [Height] REAL NOT NULL,
    [Width] REAL NOT NULL,
    [Length] REAL NOT NULL
);

-- Insert sample learning spaces with valid types (Classroom, Auditorium, Laboratory)
INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES
    (1, 'Classroom', 3.0, 8.0, 10.0),
    (2, 'Auditorium', 5.0, 15.0, 20.0),
    (3, 'Laboratory', 3.5, 12.0, 15.0);
