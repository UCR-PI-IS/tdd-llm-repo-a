-- PQL-LB-002-001-seed.sql
-- Seed data for the Building table used by story PQL-LB-002-001 (List Buildings).
--
-- The Building table schema is derived from the EF Core entity configuration in
-- Backend.Infrastructure/EntityConfigurations/BuildingEntityConfiguration.cs:
--   InternalId  INT          NOT NULL PRIMARY KEY
--   Name        NVARCHAR(200)
--   Color       NVARCHAR(50)
--   Height      REAL         NOT NULL
--   Length      REAL         NOT NULL
--   Width       REAL         NOT NULL
--   X           REAL         NOT NULL
--   Y           REAL         NOT NULL
--   Z           REAL         NOT NULL

SET NOCOUNT ON;

-- Create the Building table when absent
IF OBJECT_ID('dbo.Building', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Building]
    (
        [InternalId] INT           NOT NULL PRIMARY KEY,
        [Name]       NVARCHAR(200) NOT NULL,
        [Color]      NVARCHAR(50)  NOT NULL,
        [Height]     REAL          NOT NULL,
        [Length]     REAL          NOT NULL,
        [Width]      REAL          NOT NULL,
        [X]          REAL          NOT NULL,
        [Y]          REAL          NOT NULL,
        [Z]          REAL          NOT NULL
    );
END;

-- Seed sample buildings
DELETE FROM [dbo].[Building] WHERE [InternalId] IN (1, 2, 3);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z]) VALUES
    (1, 'Engineering Building', 'Red',   20.5, 50.0, 30.0, 100.0, 200.0, 0.0),
    (2, 'Science Building',     'Blue',  25.0, 60.0, 40.0, 150.0, 250.0, 0.0),
    (3, 'Arts Building',        'Green', 15.0, 40.0, 25.0, 200.0, 300.0, 0.0);

SELECT 'Building' AS [table], COUNT(*) AS [rows] FROM [dbo].[Building];
