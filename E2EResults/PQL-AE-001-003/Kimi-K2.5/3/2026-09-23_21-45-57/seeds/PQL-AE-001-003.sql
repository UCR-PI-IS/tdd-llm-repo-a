IF OBJECT_ID('dbo.University','U') IS NULL
CREATE TABLE [dbo].[University]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL,
    [Country] NVARCHAR(100) NOT NULL
);

INSERT INTO [dbo].[University] ([Name], [Country]) VALUES
(N'Universidad de Costa Rica', N'Costa Rica'),
(N'Technische Universiteit Delft', N'Netherlands'),
(N'Massachusetts Institute of Technology', N'United States');
