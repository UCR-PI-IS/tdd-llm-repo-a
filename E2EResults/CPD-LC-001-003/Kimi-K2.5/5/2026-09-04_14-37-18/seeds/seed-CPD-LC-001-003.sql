-- Seed data for CPD-LC-001-003 (Whiteboard creation)
-- EF Core uses TPT (Table Per Type) for inheritance

IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
CREATE TABLE LearningSpace (
    LearningSpaceId INT NOT NULL PRIMARY KEY,
    Type NVARCHAR(50) NOT NULL,
    Height REAL NOT NULL,
    Width REAL NOT NULL,
    Length REAL NOT NULL
);

IF OBJECT_ID('dbo.LearningComponent', 'U') IS NULL
CREATE TABLE LearningComponent (
    ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
    LearningSpaceId NVARCHAR(50) NOT NULL,
    Width REAL NOT NULL,
    Height REAL NOT NULL,
    Depth REAL NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL,
    Orientation NVARCHAR(20) NOT NULL
);

IF OBJECT_ID('dbo.Whiteboard', 'U') IS NULL
CREATE TABLE Whiteboard (
    ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
    MarkerColor NVARCHAR(50) NOT NULL,
    FOREIGN KEY (ComponentId) REFERENCES LearningComponent(ComponentId)
);

-- Insert learning spaces
INSERT INTO LearningSpace (LearningSpaceId, Type, Height, Width, Length) VALUES
(1, 'Classroom', 5.0, 10.0, 10.0),
(2, 'Laboratory', 4.0, 8.0, 12.0),
(3, 'Auditorium', 6.0, 15.0, 20.0);

-- Insert learning components (base data)
INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation) VALUES
('WB-001', '1', 2.5, 1.5, 0.5, 0.0, 0.0, 0.0, 'North'),
('WB-002', '1', 3.0, 2.0, 0.5, 1.0, 0.0, 0.0, 'South'),
('WB-003', '1', 2.0, 1.0, 0.5, 0.0, 0.0, 1.0, 'East');

-- Insert whiteboard-specific data
INSERT INTO Whiteboard (ComponentId, MarkerColor) VALUES
('WB-001', 'Blue'),
('WB-002', 'Red'),
('WB-003', 'Green');
