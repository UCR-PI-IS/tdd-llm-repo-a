-- Seed script for PQL-AE-001-003: Add University
-- Creates the University table and inserts sample data

IF OBJECT_ID('dbo.University', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[University] (
        [UniversityId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [Country] NVARCHAR(100) NOT NULL
    );
END
GO

INSERT INTO [dbo].[University] ([Name], [Country]) VALUES (N'Universidad de Costa Rica', N'Costa Rica');
INSERT INTO [dbo].[University] ([Name], [Country]) VALUES (N'Massachusetts Institute of Technology', N'United States');
INSERT INTO [dbo].[University] ([Name], [Country]) VALUES (N'Universidad de Salamanca', N'Spain');
GO
