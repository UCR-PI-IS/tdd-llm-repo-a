IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE LearningSpace (
    LearningSpaceId INT NOT NULL PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Width REAL NOT NULL,
    Length REAL NOT NULL
);

IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
CREATE TABLE Whiteboard (
    ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
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

INSERT INTO LearningSpace (LearningSpaceId, Type, Height, Width, Length) VALUES
(1, 'Classroom', 10.0, 10.0, 10.0),
(2, 'Laboratory', 5.0, 5.0, 5.0),
(3, 'Auditorium', 15.0, 20.0, 15.0);

INSERT INTO Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor) VALUES
('WB-001', '1', 2.5, 1.5, 0.5, 1.0, 0.0, 2.0, 'North', 'Blue'),
('WB-002', '1', 3.0, 2.0, 0.5, 0.0, 0.0, 0.0, 'South', 'Green'),
('WB-003', '2', 1.5, 1.0, 0.3, 0.0, 0.0, 0.0, 'East', 'Red');
