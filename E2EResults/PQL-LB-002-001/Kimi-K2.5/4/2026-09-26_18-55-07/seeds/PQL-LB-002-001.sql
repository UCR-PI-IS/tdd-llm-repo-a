IF OBJECT_ID('dbo.Building','U') IS NULL
CREATE TABLE [dbo].[Building](
    [InternalId] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](200) NOT NULL,
    [Color] [nvarchar](50) NOT NULL,
    [Height] [real] NOT NULL,
    [Length] [real] NOT NULL,
    [Width] [real] NOT NULL,
    [X] [real] NOT NULL,
    [Y] [real] NOT NULL,
    [Z] [real] NOT NULL,
    CONSTRAINT [PK_Building] PRIMARY KEY CLUSTERED ([InternalId] ASC)
);
GO

SET IDENTITY_INSERT [dbo].[Building] ON;

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z])
VALUES (1, 'Engineering Building', 'Red', 20.5, 50.0, 30.0, 100.0, 200.0, 0.0);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z])
VALUES (2, 'Science Building', 'Blue', 25.0, 60.0, 40.0, 150.0, 250.0, 0.0);

INSERT INTO [dbo].[Building] ([InternalId], [Name], [Color], [Height], [Length], [Width], [X], [Y], [Z])
VALUES (3, 'Library', 'Green', 15.0, 40.0, 25.0, 200.0, 300.0, 0.0);

SET IDENTITY_INSERT [dbo].[Building] OFF;
GO
