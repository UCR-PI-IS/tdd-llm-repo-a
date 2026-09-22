IF OBJECT_ID('dbo.Person','U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Person]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [IdentityNumber] NVARCHAR(50) NOT NULL,
        [BirthDate] DATETIME2 NOT NULL,
        [Phone] NVARCHAR(50) NULL
    );
END;
GO

INSERT INTO [dbo].[Person] ([FirstName], [LastName], [Email], [IdentityNumber], [BirthDate], [Phone])
VALUES (N'Alice', N'Anderson', N'alice@example.com', N'111111111', '1985-03-15', N'+1111111111');

INSERT INTO [dbo].[Person] ([FirstName], [LastName], [Email], [IdentityNumber], [BirthDate], [Phone])
VALUES (N'Bob', N'Brown', N'bob@example.com', N'222222222', '1990-07-20', NULL);

INSERT INTO [dbo].[Person] ([FirstName], [LastName], [Email], [IdentityNumber], [BirthDate], [Phone])
VALUES (N'Carol', N'Clark', N'carol@example.com', N'333333333', '1978-11-05', N'+3333333333');
GO
