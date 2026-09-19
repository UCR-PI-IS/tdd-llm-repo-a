IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE LearningSpace (
    LearningSpaceId INT PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Width REAL NOT NULL,
    Length REAL NOT NULL
);

IF OBJECT_ID('dbo.LearningComponent','U') IS NULL
CREATE TABLE LearningComponent (
    ComponentId NVARCHAR(50) PRIMARY KEY,
    LearningSpaceId NVARCHAR(50) NOT NULL,
    Width REAL NOT NULL,
    Height REAL NOT NULL,
    Depth REAL NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL,
    Orientation NVARCHAR(20) NOT NULL
);

IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
CREATE TABLE Whiteboard (
    ComponentId NVARCHAR(50) PRIMARY KEY,
    LearningSpaceId NVARCHAR(50) NOT NULL,
    Width REAL NOT NULL,
    Height REAL NOT NULL,
    Depth REAL NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL,
    Orientation NVARCHAR(20) NOT NULL,
    MarkerColor NVARCHAR(50) NOT NULL
);

INSERT INTO LearningSpace (LearningSpaceId, Type, Height, Width, Length)
VALUES (1, 'Classroom', 10.0, 100.0, 100.0),
       (2, 'Auditorium', 15.0, 200.0, 200.0),
       (3, 'Laboratory', 12.0, 150.0, 150.0);

INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES ('LC-001', '1', 1.0, 1.0, 1.0, 20.0, 0.0, 20.0, 'West'),
       ('LC-002', '1', 2.0, 1.5, 0.5, 50.0, 0.0, 50.0, 'South');

INSERT INTO Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES ('{id}', '1', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0, 'North', 'Blue'),
       ('WB-001', '1', 2.5, 1.5, 0.1, 5.0, 0.0, 5.0, 'South', 'Red'),
       ('WB-002', '1', 3.0, 2.0, 0.2, 10.0, 0.0, 10.0, 'East', 'Green');
