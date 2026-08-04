-- Create LearningSpace table if it doesn't exist
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[LearningSpace]
    (
        [id] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [type] NVARCHAR(50) NOT NULL,
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

-- Insert sample learning spaces
INSERT INTO [dbo].[LearningSpace] ([id], [type], [height], [width], [length])
VALUES 
    ('LS-001', 'Classroom', 3.0, 8.0, 10.0),
    ('LS-002', 'Laboratory', 3.5, 12.0, 15.0),
    ('LS-003', 'Auditorium', 5.0, 20.0, 25.0);

-- Insert sample learning components for LS-001
INSERT INTO [dbo].[LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES 
    ('COMP-001', 'LS-001', 2.5, 1.8, 0.5, 1.0, 0.0, 2.0, 'North'),
    ('COMP-002', 'LS-001', 1.5, 1.5, 0.4, 3.0, 0.0, 2.0, 'South'),
    ('COMP-003', 'LS-001', 2.0, 1.2, 0.3, 5.0, 0.0, 2.0, 'East');

-- Insert sample learning components for LS-002
INSERT INTO [dbo].[LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES 
    ('COMP-004', 'LS-002', 3.0, 2.0, 0.6, 2.0, 0.0, 3.0, 'West');
