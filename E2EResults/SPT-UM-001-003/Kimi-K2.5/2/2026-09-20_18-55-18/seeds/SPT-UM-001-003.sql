-- Seed data for SPT-UM-001-003: Create Person feature
-- This script creates sample person records for end-to-end testing

IF OBJECT_ID('dbo.Person', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Person] (
        [Id] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(255) NOT NULL,
        [IdentityNumber] NVARCHAR(50) NOT NULL,
        [BirthDate] DATETIME2 NOT NULL,
        [Phone] NVARCHAR(50) NULL
    );

    CREATE UNIQUE INDEX [IX_Person_Email] ON [dbo].[Person] ([Email]);
    CREATE UNIQUE INDEX [IX_Person_IdentityNumber] ON [dbo].[Person] ([IdentityNumber]);
END

-- Insert sample person records
IF NOT EXISTS (SELECT 1 FROM [dbo].[Person] WHERE [Id] = 'PER-001')
BEGIN
    INSERT INTO [dbo].[Person] ([Id], [FirstName], [LastName], [Email], [IdentityNumber], [BirthDate], [Phone])
    VALUES ('PER-001', 'John', 'Doe', 'john.doe@example.com', '123456789', '1990-01-01', '555-1234');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Person] WHERE [Id] = 'PER-002')
BEGIN
    INSERT INTO [dbo].[Person] ([Id], [FirstName], [LastName], [Email], [IdentityNumber], [BirthDate], [Phone])
    VALUES ('PER-002', 'Jane', 'Smith', 'jane.smith@example.com', '987654321', '1985-05-15', NULL);
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Person] WHERE [Id] = 'PER-003')
BEGIN
    INSERT INTO [dbo].[Person] ([Id], [FirstName], [LastName], [Email], [IdentityNumber], [BirthDate], [Phone])
    VALUES ('PER-003', 'Bob', 'Johnson', 'bob.johnson@example.com', '456789123', '1978-12-20', '555-5678');
END
