-- Seed data for University table (story PQL-AE-001-003)
IF OBJECT_ID('dbo.University', 'U') IS NULL
CREATE TABLE [dbo].[University] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Country] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_University] PRIMARY KEY CLUSTERED ([Id] ASC)
);

INSERT INTO [dbo].[University] ([Name], [Country]) VALUES ('Universidad de Costa Rica', 'Costa Rica');
INSERT INTO [dbo].[University] ([Name], [Country]) VALUES ('Massachusetts Institute of Technology', 'United States');
INSERT INTO [dbo].[University] ([Name], [Country]) VALUES ('Universidad de Salamanca', 'Spain');
