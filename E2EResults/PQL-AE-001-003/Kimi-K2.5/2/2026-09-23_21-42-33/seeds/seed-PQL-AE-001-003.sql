-- Seed data for PQL-AE-001-003: Add University
-- Creates University table and seeds sample data

IF OBJECT_ID('dbo.University', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.University (
        Name NVARCHAR(200) NOT NULL,
        Country NVARCHAR(100) NOT NULL,
        CONSTRAINT PK_University PRIMARY KEY (Name)
    );
END

-- Insert sample universities
INSERT INTO dbo.University (Name, Country)
VALUES 
    ('UCR', 'Costa Rica'),
    ('MIT', 'United States'),
    ('Stanford', 'United States');
