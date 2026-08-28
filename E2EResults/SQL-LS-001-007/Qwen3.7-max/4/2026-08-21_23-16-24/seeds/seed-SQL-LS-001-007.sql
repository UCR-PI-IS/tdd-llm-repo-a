-- Seed data for LearningSpace table (story SQL-LS-001-007)
-- Table: LearningSpace with columns: LearningSpaceId (IDENTITY), Type, Height, Width, Length

IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LearningSpace (
        LearningSpaceId INT IDENTITY(1,1) PRIMARY KEY,
        Type NVARCHAR(50) NOT NULL,
        Height REAL NOT NULL,
        Width REAL NOT NULL,
        Length REAL NOT NULL
    );
END

-- Insert 3 sample learning spaces with valid types and dimensions
INSERT INTO dbo.LearningSpace (Type, Height, Width, Length)
VALUES
    ('Classroom', 3.0, 8.0, 10.0),
    ('Auditorium', 5.0, 15.0, 20.0),
    ('Laboratory', 3.5, 12.0, 15.0);
