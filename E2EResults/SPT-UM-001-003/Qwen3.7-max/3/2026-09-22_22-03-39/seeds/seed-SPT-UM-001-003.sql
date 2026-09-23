IF OBJECT_ID('dbo.Person','U') IS NULL
CREATE TABLE [dbo].[Person] (
    [Id] INT NOT NULL PRIMARY KEY,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [IdentityNumber] NVARCHAR(50) NOT NULL,
    [BirthDate] DATETIME2 NOT NULL,
    [Phone] NVARCHAR(50) NULL
);

INSERT INTO [dbo].[Person] ([Id], [FirstName], [LastName], [Email], [IdentityNumber], [BirthDate], [Phone])
VALUES (1, 'John', 'Doe', 'john.doe@example.com', '123456789', '1990-01-15', '+506-8888-8888');

INSERT INTO [dbo].[Person] ([Id], [FirstName], [LastName], [Email], [IdentityNumber], [BirthDate], [Phone])
VALUES (2, 'Jane', 'Smith', 'jane.smith@example.com', '987654321', '1985-06-20', NULL);

INSERT INTO [dbo].[Person] ([Id], [FirstName], [LastName], [Email], [IdentityNumber], [BirthDate], [Phone])
VALUES (3, 'Carlos', 'Ramirez', 'carlos.ramirez@example.com', '456789123', '1995-11-03', '+506-7777-7777');
