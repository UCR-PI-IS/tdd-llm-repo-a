-- Create Building table
IF OBJECT_ID('dbo.Building', 'U') IS NULL
BEGIN
    CREATE TABLE Building (
        InternalId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Color NVARCHAR(50) NOT NULL,
        Height FLOAT NOT NULL,
        Length FLOAT NOT NULL,
        Width FLOAT NOT NULL,
        X FLOAT NOT NULL,
        Y FLOAT NOT NULL,
        Z FLOAT NOT NULL
    );
END
