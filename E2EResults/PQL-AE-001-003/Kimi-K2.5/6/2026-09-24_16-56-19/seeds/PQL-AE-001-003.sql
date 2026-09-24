-- Seed data for PQL-AE-001-003: Add University
-- University table with sample data

IF OBJECT_ID('dbo.University', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[University] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [Country] NVARCHAR(100) NOT NULL
    );
END

-- Insert sample universities
INSERT INTO [dbo].[University] ([Name], [Country])
VALUES 
    (N'Universidad de Costa Rica', N'Costa Rica'),
    (N'Technological Institute of Costa Rica', N'Costa Rica'),
    (N'National University of Costa Rica', N'Costa Rica');
