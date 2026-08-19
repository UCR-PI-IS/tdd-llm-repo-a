-- Seed data for CPD-LC-001-001: Learning Components
-- Creates tables and inserts sample data

-- Create LearningSpace table if it doesn't exist
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[LearningSpace]
    (
        [id] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [type] NVARCHAR(50) NULL,
        [height] REAL NOT NULL,
        [width] REAL NOT NULL,
        [length] REAL NOT NULL
    );
END

-- Create LearningComponent table if it doesn't exist
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
        [Orientation] NVARCHAR(10) NOT NULL
    );
END

-- Insert sample LearningSpace data
INSERT INTO [dbo].[LearningSpace] ([id], [type], [height], [width], [length])
VALUES
    ('ls-001', 'classroom', 3.0, 10.0, 15.0),
    ('ls-002', 'lab', 3.5, 8.0, 12.0),
    ('ls-003', 'auditorium', 5.0, 20.0, 30.0);

-- Insert sample LearningComponent data
INSERT INTO [dbo].[LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES
    ('comp-001', 'ls-001', 2.0, 3.0, 1.5, 10.0, 20.0, 0.0, 'North'),
    ('comp-002', 'ls-001', 1.0, 2.0, 1.0, 5.0, 10.0, 0.0, 'South'),
    ('comp-003', 'ls-002', 1.5, 2.5, 0.5, 3.0, 7.0, 0.0, 'East');
