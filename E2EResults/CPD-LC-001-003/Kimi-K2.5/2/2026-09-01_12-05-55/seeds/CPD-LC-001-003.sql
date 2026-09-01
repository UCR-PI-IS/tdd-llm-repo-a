IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE dbo.LearningSpace (
    LearningSpaceId INT NOT NULL PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Width REAL NOT NULL,
    Length REAL NOT NULL
);

IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
CREATE TABLE dbo.Whiteboard (
    ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
    LearningSpaceId NVARCHAR(50) NOT NULL,
    Width REAL NOT NULL,
    Height REAL NOT NULL,
    Depth REAL NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL,
    Orientation NVARCHAR(10) NOT NULL,
    MarkerColor NVARCHAR(50) NOT NULL
);

INSERT INTO dbo.LearningSpace (LearningSpaceId, Type, Height, Width, Length)
VALUES 
    (1, 'Classroom', 3.0, 10.0, 10.0),
    (2, 'Laboratory', 4.0, 15.0, 20.0),
    (3, 'Auditorium', 5.0, 25.0, 30.0);

INSERT INTO dbo.Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor)
VALUES 
    ('WB-001', '1', 5.0, 2.0, 0.5, 0.0, 0.0, 0.0, 'North', 'Blue'),
    ('WB-002', '2', 8.0, 3.0, 0.5, 1.0, 0.0, 0.0, 'South', 'Red'),
    ('WB-003', '3', 10.0, 2.5, 0.5, 0.0, 0.0, 0.0, 'East', 'Green');
