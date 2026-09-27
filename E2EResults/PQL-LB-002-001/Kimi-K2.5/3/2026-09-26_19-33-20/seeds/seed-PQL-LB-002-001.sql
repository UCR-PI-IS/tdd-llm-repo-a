IF OBJECT_ID('dbo.Building','U') IS NULL
CREATE TABLE dbo.Building (
    InternalId INT NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Color NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Length REAL NOT NULL,
    Width REAL NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL
);

INSERT INTO dbo.Building (InternalId, Name, Color, Height, Length, Width, X, Y, Z)
VALUES
    (1, 'Engineering Building', 'Red', 20.5, 50.0, 30.0, 100.0, 200.0, 0.0),
    (2, 'Science Building', 'Blue', 25.0, 60.0, 40.0, 150.0, 250.0, 0.0),
    (3, 'Arts Building', 'Green', 15.0, 40.0, 20.0, 80.0, 120.0, 0.0);
