-- Seed data for SPT-UM-001-003: Create a Person
-- Table: Person (from PersonEntityConfiguration)

IF OBJECT_ID('dbo.Person', 'U') IS NULL
CREATE TABLE dbo.Person (
    Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    IdentityNumber NVARCHAR(50) NOT NULL,
    BirthDate DATETIME2 NOT NULL,
    Phone NVARCHAR(20) NULL
);

SET IDENTITY_INSERT dbo.Person ON;
INSERT INTO dbo.Person (Id, FirstName, LastName, Email, IdentityNumber, BirthDate, Phone)
VALUES
    (1, 'John', 'Doe', 'john.doe@example.com', 'ID-001', '2000-01-15', '+1234567890'),
    (2, 'Jane', 'Smith', 'jane.smith@example.com', 'ID-002', '1995-06-20', NULL),
    (3, 'Carlos', 'Ramirez', 'carlos.ramirez@example.com', 'ID-003', '1988-11-03', '+50688889999');
SET IDENTITY_INSERT dbo.Person OFF;
