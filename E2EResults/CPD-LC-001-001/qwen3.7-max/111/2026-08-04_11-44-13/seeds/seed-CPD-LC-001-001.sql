IF OBJECT_ID('dbo.LearningComponent','U') IS NULL
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

INSERT INTO [dbo].[LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES
    ('COMP-001', 'LS-001', 2.0, 3.0, 1.0, 5.0, 10.0, 0.0, 'North'),
    ('COMP-002', 'LS-001', 1.0, 2.0, 0.5, 8.0, 12.0, 0.0, 'South'),
    ('COMP-003', 'LS-002', 1.5, 1.5, 0.8, 3.0, 7.0, 1.0, 'East');
