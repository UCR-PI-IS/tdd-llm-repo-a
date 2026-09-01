-- Seed data for CPD-LC-001-003: Create whiteboard in a learning space
-- LearningSpace table (parent - must be seeded first)
IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE [dbo].[LearningSpace] (
    [LearningSpaceId] INT NOT NULL PRIMARY KEY,
    [Type] NVARCHAR(50) NOT NULL,
    [Height] REAL NOT NULL,
    [Width] REAL NOT NULL,
    [Length] REAL NOT NULL
);

-- Whiteboard table
IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
CREATE TABLE [dbo].[Whiteboard] (
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

-- Seed LearningSpace rows
INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (1, 'Classroom', 3.0, 8.0, 10.0);

INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (2, 'Laboratory', 4.0, 12.0, 15.0);

INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (3, 'Auditorium', 6.0, 20.0, 30.0);

-- Seed Whiteboard rows (all fit within LearningSpace 1: 3.0h x 8.0w x 10.0l)
INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-001', '1', 2.0, 1.0, 0.1, 1.0, 0.5, 0.0, 'South', 'Blue');

INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-002', '1', 3.0, 1.5, 0.1, 0.0, 0.0, 0.0, 'East', 'Red');

INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-003', '2', 4.0, 2.0, 0.15, 2.0, 1.0, 0.0, 'West', 'Green');
