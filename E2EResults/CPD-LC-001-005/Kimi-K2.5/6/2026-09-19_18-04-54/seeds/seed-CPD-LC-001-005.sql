IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
BEGIN
    CREATE TABLE [dbo].[LearningSpace]
    (
        [LearningSpaceId] INT NOT NULL PRIMARY KEY,
        [Type] NVARCHAR(50) NOT NULL,
        [Height] REAL NOT NULL,
        [Width] REAL NOT NULL,
        [Length] REAL NOT NULL
    );
END;

IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Whiteboard]
    (
        [ComponentId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [LearningSpaceId] NVARCHAR(50) NOT NULL,
        [Width] REAL NOT NULL,
        [Height] REAL NOT NULL,
        [Depth] REAL NOT NULL,
        [X] REAL NOT NULL,
        [Y] REAL NOT NULL,
        [Z] REAL NOT NULL,
        [Orientation] NVARCHAR(20) NOT NULL,
        [MarkerColor] NVARCHAR(50) NOT NULL
    );
END;

-- Seed learning spaces
INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (1, N'Classroom', 3.0, 10.0, 10.0);

-- Seed whiteboards
INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES (N'WB-001', N'LS-001', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0, N'South', N'Blue');

INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES (N'WB-002', N'LS-001', 2.0, 1.5, 0.1, 3.0, 0.0, 4.0, N'North', N'Green');

INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES (N'WB-003', N'LS-001', 1.5, 1.0, 0.1, 5.0, 0.0, 1.0, N'East', N'Red');
