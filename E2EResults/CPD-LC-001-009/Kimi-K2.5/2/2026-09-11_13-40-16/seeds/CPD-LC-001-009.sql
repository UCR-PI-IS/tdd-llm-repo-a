IF OBJECT_ID('dbo.LearningComponent','U') IS NULL
CREATE TABLE dbo.LearningComponent (
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

INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES
('COMP-SEED-001', 'LS-001', 1.5, 1.0, 0.5, 10.0, 5.0, 0.0, 'North'),
('COMP-SEED-002', 'LS-001', 2.0, 1.5, 0.8, 15.0, 10.0, 0.0, 'South'),
('COMP-SEED-003', 'LS-002', 1.0, 1.0, 0.3, 5.0, 5.0, 0.0, 'East');
