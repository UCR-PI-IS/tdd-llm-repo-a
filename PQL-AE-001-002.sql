IF OBJECT_ID('dbo.Building','U') IS NULL
CREATE TABLE Building (
    InternalId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Color NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Length REAL NOT NULL,
    Width REAL NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL,
    AreaId INT NOT NULL
);

SET IDENTITY_INSERT Building ON;

INSERT INTO Building (InternalId, Name, Color, Height, Length, Width, X, Y, Z, AreaId)
VALUES 
(1, 'Engineering Building', 'Blue', 10.5, 20.0, 15.0, 100.0, 0.0, 200.0, 1),
(2, 'Science Building', 'Red', 15.0, 30.0, 20.0, 50.0, 0.0, 100.0, 1),
(3, 'Library', 'Green', 12.0, 25.0, 18.0, 200.0, 0.0, 300.0, 1);

SET IDENTITY_INSERT Building OFF;
