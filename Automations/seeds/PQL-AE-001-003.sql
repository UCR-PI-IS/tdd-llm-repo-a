-- PQL-AE-001-003.sql
-- -----------------
-- Sample data for the University table. The table schema and column types are
-- derived from the EF Core mapping in Backend.Infrastructure/EntityConfigurations/
-- UniversityEntityConfiguration.cs (Id is auto-generated, Name is NVARCHAR(200),
-- Country is NVARCHAR(100)).
--
-- Re-runnable: creates the table when missing and replaces only the rows it owns.

SET NOCOUNT ON;

-- ---------------------------------------------------------------------------
-- Schema (only created when absent)
-- ---------------------------------------------------------------------------
IF OBJECT_ID('dbo.University', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[University]
    (
        [Id]      INT            IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name]    NVARCHAR(200)  NOT NULL,
        [Country] NVARCHAR(100)  NOT NULL
    );
END;

-- ---------------------------------------------------------------------------
-- Sample rows
-- ---------------------------------------------------------------------------
DELETE FROM [dbo].[University]
 WHERE [Name] IN ('Universidad de Costa Rica', 'Universidad Nacional', 'TEC');

INSERT INTO [dbo].[University] ([Name], [Country]) VALUES
    ('Universidad de Costa Rica', 'Costa Rica'),
    ('Universidad Nacional', 'Costa Rica'),
    ('TEC', 'Costa Rica');

SELECT 'University' AS [table], COUNT(*) AS [rows] FROM [dbo].[University];
