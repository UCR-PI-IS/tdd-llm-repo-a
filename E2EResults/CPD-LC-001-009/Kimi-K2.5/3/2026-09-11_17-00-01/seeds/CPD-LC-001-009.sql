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
GO

INSERT INTO dbo.LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES 
    ('COMP-001', 'LS-001', 1.5, 1.0, 0.5, 10.0, 5.0, 0.0, 'North'),
    ('COMP-002', 'LS-001', 2.0, 1.5, 0.3, 20.0, 10.0, 0.0, 'East'),
    ('COMP-003', 'LS-002', 3.0, 2.0, 0.6, 5.0, 5.0, 1.0, 'South');
GO
