-- Seed data for SQL-LS-001-007: Create learning spaces
IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE [dbo].[LearningSpace] (
    [LearningSpaceId] INT NOT NULL PRIMARY KEY,
    [Type] NVARCHAR(50) NOT NULL,
    [Height] REAL NOT NULL,
    [Width] REAL NOT NULL,
    [Length] REAL NOT NULL
);

INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length]) VALUES (100, 'Classroom', 3.0, 8.0, 10.0);
INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length]) VALUES (101, 'Auditorium', 5.0, 15.0, 20.0);
INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length]) VALUES (102, 'Laboratory', 3.5, 12.0, 15.0);
