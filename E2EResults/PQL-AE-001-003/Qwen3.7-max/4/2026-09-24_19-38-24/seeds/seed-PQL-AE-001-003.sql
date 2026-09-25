-- Seed data for University table (story PQL-AE-001-003)
IF OBJECT_ID('dbo.University','U') IS NULL
CREATE TABLE dbo.University (
    UniversityId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Country NVARCHAR(100) NOT NULL
);

INSERT INTO dbo.University (Name, Country) VALUES ('UCR', 'Costa Rica');
INSERT INTO dbo.University (Name, Country) VALUES ('UNA', 'Costa Rica');
INSERT INTO dbo.University (Name, Country) VALUES ('TEC', 'Costa Rica');
