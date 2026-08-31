-- Seed SQL for CPD-LC-001-003: Create whiteboard in a learning space
-- Creates LearningSpace and Whiteboard tables if they don't exist, then seeds sample data.

IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE [dbo].[LearningSpace] (
    [LearningSpaceId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Type] NVARCHAR(50) NOT NULL,
    [Height] REAL NOT NULL,
    [Width] REAL NOT NULL,
    [Length] REAL NOT NULL
);

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

-- Seed learning spaces (IDs will be auto-generated as 1, 2, 3)
SET IDENTITY_INSERT [dbo].[LearningSpace] ON;
INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (1, 'Classroom', 3.0, 10.0, 8.0);
INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (2, 'Laboratory', 4.0, 12.0, 10.0);
INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (3, 'Classroom', 3.5, 8.0, 6.0);
SET IDENTITY_INSERT [dbo].[LearningSpace] OFF;

-- Seed whiteboards
INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-001', '1', 2.5, 1.5, 0.5, 1.0, 0.0, 1.0, 'South', 'Blue');
INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-002', '1', 3.0, 2.0, 0.3, 5.0, 0.0, 2.0, 'East', 'Red');
INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-003', '2', 4.0, 2.5, 0.4, 2.0, 0.5, 1.0, 'West', 'Green');
