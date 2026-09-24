-- Seed data for University table
-- Create table if not exists
IF OBJECT_ID('dbo.University', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[University]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [Name] NVARCHAR(200) NOT NULL,
        [Country] NVARCHAR(100) NOT NULL
    )
END

-- Insert seed data
INSERT INTO [dbo].[University] ([Name], [Country])
VALUES 
    ('Universidad de Costa Rica', 'Costa Rica'),
    ('Massachusetts Institute of Technology', 'United States'),
    ('Stanford University', 'United States');
