-- Seed data for CPD-LC-001-003 (Whiteboard creation)
-- LearningSpace table must be seeded first due to foreign key relationship

IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
CREATE TABLE LearningSpace (
    LearningSpaceId INT NOT NULL PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Width REAL NOT NULL,
    Length REAL NOT NULL
);

IF OBJECT_ID('dbo.Whiteboard', 'U') IS NULL
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

-- Insert learning spaces
INSERT INTO LearningSpace (LearningSpaceId, Type, Height, Width, Length) VALUES
(1, 'Classroom', 5.0, 10.0, 10.0),
(2, 'Laboratory', 4.0, 8.0, 12.0),
(3, 'Auditorium', 6.0, 15.0, 20.0);

-- Insert whiteboards
INSERT INTO Whiteboard (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation, MarkerColor) VALUES
('WB-001', 'IF-0103', 2.5, 1.5, 0.5, 0.0, 0.0, 0.0, 'North', 'Blue'),
('WB-002', 'IF-0103', 3.0, 2.0, 0.5, 1.0, 0.0, 0.0, 'South', 'Red'),
('WB-003', 'IF-0103', 2.0, 1.0, 0.5, 0.0, 0.0, 1.0, 'East', 'Green');
