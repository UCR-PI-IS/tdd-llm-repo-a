-- Seed data for Person table for SPT-UM-001-003 end-to-end testing
-- Create Person table if not exists (for e2e ephemeral database)
IF OBJECT_ID('dbo.Person', 'U') IS NULL
CREATE TABLE dbo.Person (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    IdentityNumber NVARCHAR(50) NOT NULL UNIQUE,
    BirthDate DATETIME2 NOT NULL,
    Phone NVARCHAR(50) NULL
);

-- Insert seed data with past birth dates (satisfying domain validation)
INSERT INTO dbo.Person (Id, FirstName, LastName, Email, IdentityNumber, BirthDate, Phone)
VALUES 
    ('00000000-0000-0000-0000-000000000001', 'John', 'Doe', 'john.doe@example.com', 'ID-001-001', '1990-01-15', '+1234567890'),
    ('00000000-0000-0000-0000-000000000002', 'Jane', 'Smith', 'jane.smith@example.com', 'ID-002-002', '1985-05-20', NULL),
    ('00000000-0000-0000-0000-000000000003', 'Bob', 'Wilson', 'bob.wilson@example.com', 'ID-003-003', '1978-12-10', '+9876543210');
