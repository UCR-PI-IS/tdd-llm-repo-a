-- Seed data for University table (story PQL-AE-001-003)
-- Derived from Backend.Domain/Entities/University.cs and
-- Backend.Infrastructure/EntityConfigurations/UniversityEntityConfiguration.cs

IF OBJECT_ID('dbo.University', 'U') IS NULL
CREATE TABLE [dbo].[University] (
    [Id]      INT            IDENTITY(1,1) NOT NULL,
    [Name]    NVARCHAR(200)  NOT NULL,
    [Country] NVARCHAR(100)  NOT NULL,
    CONSTRAINT [PK_University] PRIMARY KEY CLUSTERED ([Id] ASC)
);

INSERT INTO [dbo].[University] ([Name], [Country]) VALUES (N'Universidad de Costa Rica', N'Costa Rica');
INSERT INTO [dbo].[University] ([Name], [Country]) VALUES (N'Universidad Nacional', N'Costa Rica');
INSERT INTO [dbo].[University] ([Name], [Country]) VALUES (N'Tecnologico de Costa Rica', N'Costa Rica');
