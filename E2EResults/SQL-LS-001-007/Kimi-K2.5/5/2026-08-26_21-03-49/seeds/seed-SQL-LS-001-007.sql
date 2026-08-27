-- Seed data for LearningSpace table
-- Story: SQL-LS-001-007 - Create learning spaces using a relational database and internal keys

-- Create table if it doesn't exist (without IDENTITY since application generates IDs)
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
BEGIN
    CREATE TABLE LearningSpace (
        LearningSpaceId INT PRIMARY KEY,
        Type NVARCHAR(50) NOT NULL,
        Height REAL NOT NULL,
        Width REAL NOT NULL,
        Length REAL NOT NULL
    );
END

-- Insert sample learning spaces with explicit IDs
INSERT INTO LearningSpace (LearningSpaceId, Type, Height, Width, Length) VALUES
    (1, 'Classroom', 3.0, 8.0, 10.0),
    (2, 'Auditorium', 5.0, 15.0, 20.0),
    (3, 'Laboratory', 3.5, 12.0, 15.0);