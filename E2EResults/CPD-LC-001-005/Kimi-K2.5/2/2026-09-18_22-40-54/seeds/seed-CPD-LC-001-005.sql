IF OBJECT_ID('dbo.LearningSpace','U') IS NULL
CREATE TABLE LearningSpace (
    LearningSpaceId INT IDENTITY(1,1) PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Width REAL NOT NULL,
    Length REAL NOT NULL
);
GO

IF OBJECT_ID('dbo.Whiteboard','U') IS NULL
CREATE TABLE Whiteboard (
    ComponentId NVARCHAR(50) PRIMARY KEY,
    LearningSpaceId NVARCHAR(50) NOT NULL,
    Orientation NVARCHAR(20) NOT NULL,
    MarkerColor NVARCHAR(50) NOT NULL,
    Width REAL NOT NULL,
    Height REAL NOT NULL,
    Depth REAL NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL
);
GO

-- Seed a learning space that will accommodate the whiteboards
SET IDENTITY_INSERT LearningSpace ON;
INSERT INTO LearningSpace (LearningSpaceId, Type, Height, Width, Length)
VALUES (1, 'Classroom', 3.0, 20.0, 20.0);
SET IDENTITY_INSERT LearningSpace OFF;

-- Seed at least 3 whiteboards using the integer learning space id
INSERT INTO Whiteboard (ComponentId, LearningSpaceId, Orientation, MarkerColor, Width, Height, Depth, X, Y, Z)
VALUES 
    ('WB-001', '1', 'South', 'Blue', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0),
    ('WB-002', '1', 'East', 'Green', 2.0, 1.5, 0.1, 5.0, 0.0, 5.0),
    ('WB-003', '1', 'West', 'Red', 3.0, 2.0, 0.2, 8.0, 0.0, 8.0);
