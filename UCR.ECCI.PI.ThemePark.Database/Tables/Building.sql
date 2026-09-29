CREATE TABLE [dbo].[Building]
(
    [InternalId] INT NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Color] NVARCHAR(50) NOT NULL,
    [Height] REAL NOT NULL,
    [Length] REAL NOT NULL,
    [Width] REAL NOT NULL,
    [X] REAL NOT NULL,
    [Y] REAL NOT NULL,
    [Z] REAL NOT NULL,
    [AreaId] INT NOT NULL
)
