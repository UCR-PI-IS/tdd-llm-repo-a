-- Seed data for PQL-AE-001-001 Add Building story
-- Derived from Backend.Domain/Entities/Building.cs and BuildingEntityConfiguration.cs
-- Table: Building, Key: InternalId (INT IDENTITY), Columns mapped from entity properties

IF OBJECT_ID('dbo.Building','U') IS NULL
CREATE TABLE [dbo].[Building] (
    [InternalId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
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

SET IDENTITY_INSERT [dbo].[Building] ON;

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z], [AreaId])
VALUES (1, N'Engineering Building', N'Red', 20.5, 50.0, 30.0, 100.0, 200.0, 0.0, 1);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z], [AreaId])
VALUES (2, N'Science Hall', N'Blue', 15.0, 40.0, 25.0, 150.0, 250.0, 0.0, 1);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z], [AreaId])
VALUES (3, N'Library Tower', N'Green', 30.0, 60.0, 35.0, 200.0, 300.0, 0.0, 2);

SET IDENTITY_INSERT [dbo].[Building] OFF;
