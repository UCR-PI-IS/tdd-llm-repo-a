-- Seed data for LearningSpace table
-- Story: SQL-LS-001-007 (Create Learning Spaces)

-- Create LearningSpace table if it doesn't exist
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
CREATE TABLE [dbo].[LearningSpace] (
    [LearningSpaceId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Type] NVARCHAR(50) NOT NULL,
    [Height] REAL NOT NULL,
    [Width] REAL NOT NULL,
    [Length] REAL NOT NULL
);

-- Insert sample learning spaces with valid types (Classroom, Auditorium, Laboratory)
INSERT INTO [dbo].[LearningSpace] ([Type], [Height], [Width], [Length])
VALUES
    ('Classroom', 3.0, 8.0, 10.0),
    ('Auditorium', 5.0, 15.0, 20.0),
    ('Laboratory', 3.5, 12.0, 15.0);
