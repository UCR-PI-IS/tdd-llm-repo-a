-- Seed data for PQL-AE-001-002: Edit Building Information
-- Building table based on BuildingEntityConfiguration

IF OBJECT_ID('dbo.Building','U') IS NULL
CREATE TABLE [dbo].[Building] (
    [InternalId] INT NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Color] NVARCHAR(50) NOT NULL,
    [Height] REAL NOT NULL,
    [Length] REAL NOT NULL,
    [Width] REAL NOT NULL,
    [X] REAL NOT NULL,
    [Y] REAL NOT NULL,
    [Z] REAL NOT NULL,
    [AreaId] INT NOT NULL
);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z], [AreaId])
VALUES (1, 'Engineering Building', 'Blue', 10.5, 20.0, 15.0, 100.0, 0.0, 200.0, 1);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z], [AreaId])
VALUES (2, 'Science Hall', 'Red', 15.0, 30.0, 25.0, 150.0, 10.0, 250.0, 1);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z], [AreaId])
VALUES (3, 'Library', 'Green', 12.0, 40.0, 20.0, 200.0, 5.0, 300.0, 2);
