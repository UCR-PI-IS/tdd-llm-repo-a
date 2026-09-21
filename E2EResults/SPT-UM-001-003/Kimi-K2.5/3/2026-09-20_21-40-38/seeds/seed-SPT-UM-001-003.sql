IF OBJECT_ID('dbo.Person','U') IS NULL
BEGIN
    CREATE TABLE Person (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        IdentityNumber NVARCHAR(50) NOT NULL,
        BirthDate DATETIME2 NOT NULL,
        Phone NVARCHAR(50) NULL
    );

    CREATE UNIQUE INDEX IX_Person_Email ON Person(Email);
    CREATE UNIQUE INDEX IX_Person_IdentityNumber ON Person(IdentityNumber);
END;

INSERT INTO Person (FirstName, LastName, Email, IdentityNumber, BirthDate, Phone)
VALUES 
    ('John', 'Doe', 'john.doe@example.com', '123456789', '1990-01-01', '+1234567890'),
    ('Jane', 'Smith', 'jane.smith@example.com', '987654321', '1985-05-15', NULL),
    ('Robert', 'Johnson', 'robert.johnson@example.com', '456789123', '1978-12-10', '+9876543210');
