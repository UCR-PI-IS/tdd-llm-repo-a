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
ELSE
BEGIN
    -- Clear existing data to avoid ID conflicts with application-generated IDs
    DELETE FROM LearningSpace;
END

-- Insert sample learning spaces with explicit IDs
-- Using high IDs to avoid conflicts with application-generated IDs (starting from 1)
INSERT INTO LearningSpace (LearningSpaceId, Type, Height, Width, Length) VALUES
    (1001, 'Classroom', 3.0, 8.0, 10.0),
    (1002, 'Auditorium', 5.0, 15.0, 20.0),
    (1003, 'Laboratory', 3.5, 12.0, 15.0);