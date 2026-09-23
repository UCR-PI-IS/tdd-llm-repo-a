-- SPT-UM-001-003-seed.sql
-- Seed data for the Person table (Create Person story).
-- Derived from PersonEntityConfiguration: table "Person", columns from entity properties.

SET NOCOUNT ON;

IF OBJECT_ID('dbo.Person', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Person]
    (
        [Id]             INT            IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [FirstName]      NVARCHAR(100)  NOT NULL,
        [LastName]       NVARCHAR(100)  NOT NULL,
        [Email]          NVARCHAR(255)  NOT NULL,
        [IdentityNumber] NVARCHAR(50)   NOT NULL,
        [BirthDate]      DATETIME2      NOT NULL,
        [Phone]          NVARCHAR(20)   NULL
    );
END;

-- Seed one existing person so duplicate-detection can be tested
DELETE FROM [dbo].[Person] WHERE [Email] = 'existing.person@themepark.ucr.ac.cr';

INSERT INTO [dbo].[Person] ([FirstName], [LastName], [Email], [IdentityNumber], [BirthDate], [Phone])
VALUES ('Existing', 'Person', 'existing.person@themepark.ucr.ac.cr', '1-0234-5678', '1990-05-15', '8888-0000');

SELECT 'Person' AS [table], COUNT(*) AS [rows] FROM [dbo].[Person];
