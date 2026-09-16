-- Seed data for CPD-LC-001-009: Automatic ID Generation
-- LearningComponent table seed data

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
    ('COMP-001', 'IF-0103', 2.5, 1.5, 0.5, 10.0, 20.0, 0.0, 'North'),
    ('COMP-002', 'IF-0103', 3.0, 2.0, 1.0, 15.0, 25.0, 0.0, 'South'),
    ('COMP-003', 'LS-001', 1.5, 1.0, 0.5, 10.0, 5.0, 0.0, 'East');
