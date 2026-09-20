-- Seed data for CPD-LC-001-005: Update a whiteboard in a learning space

-- Create LearningSpace table if it doesn't exist
IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE [dbo].[LearningSpace]
(
    [LearningSpaceId] INT NOT NULL PRIMARY KEY,
    [Type] NVARCHAR(50) NOT NULL,
    [Height] REAL NOT NULL,
    [Width] REAL NOT NULL,
    [Length] REAL NOT NULL
);

-- Create Whiteboard table if it doesn't exist
IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
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

-- Seed LearningSpace data
INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (1, 'Classroom', 3.0, 10.0, 10.0);

INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (2, 'Laboratory', 4.0, 15.0, 20.0);

INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (3, 'Auditorium', 5.0, 20.0, 30.0);

-- Seed Whiteboard data
INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-001', 'IF-0103', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0, 'South', 'Blue');

INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-002', 'IF-0103', 2.0, 1.5, 0.1, 5.0, 0.0, 5.0, 'South', 'Green');

INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-003', 'IF-0104', 3.0, 2.0, 0.15, 2.0, 0.0, 3.0, 'East', 'Red');
