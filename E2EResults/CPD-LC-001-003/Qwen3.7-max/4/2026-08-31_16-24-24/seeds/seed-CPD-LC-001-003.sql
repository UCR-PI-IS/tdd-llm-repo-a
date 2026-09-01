-- Seed data for CPD-LC-001-003: Create whiteboard in a learning space

-- Create LearningSpace table if not exists
IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE [dbo].[LearningSpace] (
    [LearningSpaceId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Type] NVARCHAR(50) NOT NULL,
    [Height] REAL NOT NULL,
    [Width] REAL NOT NULL,
    [Length] REAL NOT NULL
);

-- Create Whiteboard table if not exists
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

-- Seed a learning space (large enough for whiteboards)
-- Type=Classroom, Height=3.0, Width=10.0, Length=8.0
SET IDENTITY_INSERT [dbo].[LearningSpace] ON;
INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
VALUES (1, 'Classroom', 3.0, 10.0, 8.0);
SET IDENTITY_INSERT [dbo].[LearningSpace] OFF;

-- Seed 3 whiteboards that fit in the learning space
INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-001', 'IF-0103', 2.0, 1.5, 0.1, 0.0, 0.0, 0.0, 'South', 'Blue');

INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-002', 'IF-0103', 3.0, 2.0, 0.1, 3.0, 0.0, 0.0, 'East', 'Red');

INSERT INTO [dbo].[Whiteboard] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor])
VALUES ('WB-003', 'IF-0103', 1.5, 1.0, 0.1, 0.0, 0.0, 1.0, 'West', 'Green');
