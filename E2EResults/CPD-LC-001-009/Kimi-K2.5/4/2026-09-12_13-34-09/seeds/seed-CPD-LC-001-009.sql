IF OBJECT_ID('dbo.LearningComponent','U') IS NULL
CREATE TABLE LearningComponent (
    ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
    LearningSpaceId NVARCHAR(50) NULL,
    Orientation NVARCHAR(20) NULL,
    Width REAL NOT NULL,
    Height REAL NOT NULL,
    Depth REAL NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL
);

INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Orientation, Width, Height, Depth, X, Y, Z)
VALUES
('C-001', 'LS-001', 'North', 1.5, 1.0, 0.5, 0, 0, 0),
('C-002', 'LS-001', 'South', 2.0, 1.2, 0.4, 1, 0, 0),
('C-003', 'LS-002', 'East', 1.0, 1.0, 1.0, 0, 1, 0);
