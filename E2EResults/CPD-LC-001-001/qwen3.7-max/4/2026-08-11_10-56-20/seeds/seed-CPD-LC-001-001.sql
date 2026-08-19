-- Seed data for LearningComponent entity
-- Creates the table if it doesn't exist, then inserts sample data

IF OBJECT_ID('dbo.LearningComponent', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[LearningComponent]
    (
        [ComponentId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [LearningSpaceId] NVARCHAR(50) NOT NULL,
        [Width] REAL NOT NULL,
        [Height] REAL NOT NULL,
        [Depth] REAL NOT NULL,
        [X] REAL NOT NULL,
        [Y] REAL NOT NULL,
        [Z] REAL NOT NULL,
        [Orientation] NVARCHAR(20) NOT NULL
    );
END
GO

-- Insert sample learning components for learning space LS001
INSERT INTO [dbo].[LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES
    ('C001', 'LS001', 2.0, 1.5, 0.5, 1.0, 2.0, 0.0, 'North'),
    ('C002', 'LS001', 1.0, 1.0, 0.3, 3.0, 4.0, 0.0, 'South'),
    ('C003', 'LS002', 3.0, 2.0, 0.8, 5.0, 6.0, 0.0, 'East');
GO
