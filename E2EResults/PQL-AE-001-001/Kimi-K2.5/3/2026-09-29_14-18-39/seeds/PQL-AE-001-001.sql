SET NOCOUNT ON;

IF OBJECT_ID('dbo.Area', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Area]
    (
        [Id]   INT          NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL
    );
END;

IF OBJECT_ID('dbo.Building', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Building]
    (
        [InternalId] INT          NOT NULL PRIMARY KEY,
        [Name]       NVARCHAR(100) NOT NULL,
        [Color]      NVARCHAR(50)  NOT NULL,
        [Height]     REAL         NOT NULL,
        [Length]     REAL         NOT NULL,
        [Width]      REAL         NOT NULL,
        [X]          REAL         NOT NULL,
        [Y]          REAL         NOT NULL,
        [Z]          REAL         NOT NULL,
        [AreaId]     INT          NOT NULL
    );
END;

DELETE FROM [dbo].[Building] WHERE [InternalId] IN (1, 2, 3);
DELETE FROM [dbo].[Area] WHERE [Id] = 1;

INSERT INTO [dbo].[Area] ([Id], [Name]) VALUES (1, 'Main Campus');

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z], [AreaId]) VALUES
    (1, 'Engineering Building', 'Red',   20.5, 50.0, 30.0, 100.0, 200.0, 0.0, 1),
    (2, 'Science Building',     'Blue',  15.0, 40.0, 25.0, 150.0, 300.0, 0.0, 1),
    (3, 'Library',              'Green', 25.0, 60.0, 35.0, 200.0, 400.0, 0.0, 1);

SELECT 'Area' AS [table], COUNT(*) AS [rows] FROM [dbo].[Area]
UNION ALL
SELECT 'Building', COUNT(*) FROM [dbo].[Building];
