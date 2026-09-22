-- Create Person table if it does not exist
IF OBJECT_ID('dbo.Person', 'U') IS NULL
BEGIN
    CREATE TABLE Person (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(200) NOT NULL,
        IdentityNumber NVARCHAR(50) NOT NULL,
        BirthDate DATETIME2 NOT NULL,
        Phone NVARCHAR(20) NULL
    );
END
GO

-- Seed sample persons
INSERT INTO Person (Id, FirstName, LastName, Email, IdentityNumber, BirthDate, Phone)
VALUES
    ('A1111111-1111-1111-1111-111111111111', 'John', 'Doe', 'john.doe@example.com', '123456789', '1990-01-15T00:00:00', '555-1234'),
    ('B2222222-2222-2222-2222-222222222222', 'Jane', 'Smith', 'jane.smith@example.com', '987654321', '1985-03-22T00:00:00', NULL),
    ('C3333333-3333-3333-3333-333333333333', 'Bob', 'Johnson', 'bob.johnson@example.com', '456789123', '1978-11-05T00:00:00', '555-5678');
GO
