-- Seed data for University entity
-- Table: University (Id, Name, Country)

IF OBJECT_ID('dbo.University', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.University (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Country NVARCHAR(100) NOT NULL
    );
END

-- Insert sample universities
INSERT INTO dbo.University (Name, Country) VALUES
('Universidad de Costa Rica', 'Costa Rica'),
('Massachusetts Institute of Technology', 'United States'),
('University of Oxford', 'United Kingdom');
