-- Seed data for Building entity (story PQL-AE-001-001)
-- Creates the Building table if it doesn't exist and inserts sample data

IF OBJECT_ID('dbo.Building', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Building] (
        [InternalId] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Color] NVARCHAR(50) NOT NULL,
        [Height] REAL NOT NULL,
        [Length] REAL NOT NULL,
        [Width] REAL NOT NULL,
        [X] REAL NOT NULL,
        [Y] REAL NOT NULL,
        [Z] REAL NOT NULL,
        [AreaId] INT NOT NULL,
        CONSTRAINT [PK_Building] PRIMARY KEY CLUSTERED ([InternalId] ASC)
    );
END
GO

-- Insert sample buildings
INSERT INTO [dbo].[Building] ([Name], [Color], [Height], [Length], [Width], [X], [Y], [Z], [AreaId])
VALUES
    (N'Engineering Building', N'Red', 20.5, 50.0, 30.0, 100.0, 200.0, 0.0, 1),
    (N'Science Hall', N'Blue', 15.0, 40.0, 25.0, 150.0, 250.0, 0.0, 1),
    (N'Library', N'Green', 12.0, 60.0, 35.0, 200.0, 300.0, 0.0, 2);
GO
