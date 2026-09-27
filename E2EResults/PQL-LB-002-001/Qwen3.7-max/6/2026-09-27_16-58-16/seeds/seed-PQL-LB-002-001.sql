-- Seed data for PQL-LB-002-001: List Buildings
-- Derived from Backend.Domain/Entities/Building.cs and
-- Backend.Infrastructure/EntityConfigurations/BuildingEntityConfiguration.cs

IF OBJECT_ID('dbo.Building','U') IS NULL
CREATE TABLE [dbo].[Building] (
    [InternalId] INT NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL,
    [Color] NVARCHAR(50) NOT NULL,
    [Height] REAL NOT NULL,
    [Length] REAL NOT NULL,
    [Width] REAL NOT NULL,
    [X] REAL NOT NULL,
    [Y] REAL NOT NULL,
    [Z] REAL NOT NULL
);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z])
VALUES (1, 'Engineering Building', 'Red', 20.5, 50.0, 30.0, 100.0, 200.0, 0.0);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z])
VALUES (2, 'Science Building', 'Blue', 25.0, 60.0, 40.0, 150.0, 250.0, 0.0);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z])
VALUES (3, 'Arts Building', 'Green', 18.0, 45.0, 35.0, 120.0, 180.0, 0.0);
