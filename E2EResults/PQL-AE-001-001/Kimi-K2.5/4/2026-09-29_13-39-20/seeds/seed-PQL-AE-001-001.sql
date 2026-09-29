IF OBJECT_ID('dbo.Building','U') IS NULL
    CREATE TABLE [dbo].[Building]
    (
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
VALUES
    (1, 'Engineering Building', 'Red', 20.5, 50.0, 30.0, 100.0, 200.0, 0.0, 1),
    (2, 'Science Building', 'Blue', 15.0, 40.0, 25.0, 200.0, 300.0, 0.0, 1),
    (3, 'Library', 'Gray', 12.0, 60.0, 40.0, 300.0, 100.0, 0.0, 2);
