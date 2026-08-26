-- Seed data for LearningSpace table for end-to-end testing
-- These records satisfy all validation rules:
-- Type must be: Classroom, Auditorium, or Laboratory
-- Height, Width, Length must be positive and non-zero

IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
BEGIN
    CREATE TABLE [LearningSpace]
    (
        [LearningSpaceId] INT IDENTITY(1,1) PRIMARY KEY,
        [Type] NVARCHAR(50) NOT NULL,
        [Height] REAL NOT NULL,
        [Width] REAL NOT NULL,
        [Length] REAL NOT NULL
    );
END

-- Insert sample learning spaces
INSERT INTO [LearningSpace] ([Type], [Height], [Width], [Length])
VALUES 
    ('Classroom', 3.0, 8.0, 10.0),
    ('Auditorium', 5.0, 15.0, 20.0),
    ('Laboratory', 3.5, 12.0, 15.0);
