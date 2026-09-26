IF OBJECT_ID('dbo.Building', 'U') IS NULL
CREATE TABLE Building (
    InternalId INT PRIMARY KEY,
    Name NVARCHAR(100),
    Color NVARCHAR(50),
    Height REAL,
    Length REAL,
    Width REAL,
    X REAL,
    Y REAL,
    Z REAL
);

INSERT INTO Building (InternalId, Name, Color, Height, Length, Width, X, Y, Z) VALUES
(1, N'Engineering Building', N'Red', 20.5, 50.0, 30.0, 100.0, 200.0, 0.0),
(2, N'Science Building', N'Blue', 25.0, 60.0, 40.0, 150.0, 250.0, 0.0),
(3, N'Arts Building', N'Green', 15.0, 40.0, 25.0, 50.0, 100.0, 0.0);
