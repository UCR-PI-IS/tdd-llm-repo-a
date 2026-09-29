IF OBJECT_ID('dbo.Building','U') IS NULL
CREATE TABLE [dbo].[Building]
(
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

INSERT INTO [dbo].[Building] ([Name], [Color], [Height], [Length], [Width], [X], [Y], [Z], [AreaId])
VALUES 
    ('Engineering Building', 'Blue', 15.0, 50.0, 30.0, 100.0, 200.0, 0.0, 1),
    ('Science Building', 'Red', 20.5, 45.0, 25.0, 150.0, 300.0, 0.0, 1),
    ('Library', 'Gray', 12.0, 60.0, 40.0, 200.0, 100.0, 0.0, 2);
