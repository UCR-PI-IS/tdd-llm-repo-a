-- Seed data for SPT-UM-001-003: Person creation testing
-- Create the Person table if it doesn't exist
IF OBJECT_ID('dbo.Person', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Person (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        IdentityNumber NVARCHAR(50) NOT NULL UNIQUE,
        BirthDate DATETIME2 NOT NULL,
        Phone NVARCHAR(50) NULL
    );
END

-- Insert test persons
-- Note: Using IDENTITY_INSERT OFF since Id is auto-generated
INSERT INTO dbo.Person (FirstName, LastName, Email, IdentityNumber, BirthDate, Phone)
VALUES 
    ('Jane', 'Smith', 'jane.smith@example.com', '987654321', '1995-05-15', '+1234567890'),
    ('Bob', 'Johnson', 'bob.johnson@example.com', '456789123', '1988-12-03', NULL),
    ('Alice', 'Williams', 'alice.williams@example.com', '789123456', '1992-08-22', '+0987654321');
