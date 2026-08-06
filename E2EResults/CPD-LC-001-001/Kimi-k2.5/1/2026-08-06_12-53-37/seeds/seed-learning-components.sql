-- Seed data for CPD-LC-001-001: Learning Components Listing

-- Create LearningSpace table if not exists
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
BEGIN
    CREATE TABLE LearningSpace (
        id NVARCHAR(50) NOT NULL PRIMARY KEY,
        type NVARCHAR(50) NOT NULL,
        height FLOAT NOT NULL,
        width FLOAT NOT NULL,
        length FLOAT NOT NULL
    );
END

-- Create LearningComponent table if not exists
IF OBJECT_ID('dbo.LearningComponent', 'U') IS NULL
BEGIN
    CREATE TABLE LearningComponent (
        ComponentId NVARCHAR(50) NOT NULL PRIMARY KEY,
        LearningSpaceId NVARCHAR(50) NOT NULL,
        Width REAL NOT NULL,
        Height REAL NOT NULL,
        Depth REAL NOT NULL,
        X REAL NOT NULL,
        Y REAL NOT NULL,
        Z REAL NOT NULL,
        Orientation NVARCHAR(10) NOT NULL
    );
END

-- Seed LearningSpace data
IF NOT EXISTS (SELECT 1 FROM LearningSpace WHERE id = 'LS-001')
BEGIN
    INSERT INTO LearningSpace (id, type, height, width, length)
    VALUES ('LS-001', 'Classroom', 3.5, 8.0, 10.0);
END

-- Seed LearningComponent data for LS-001
IF NOT EXISTS (SELECT 1 FROM LearningComponent WHERE ComponentId = 'COMP-001')
BEGIN
    INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
    VALUES ('COMP-001', 'LS-001', 2.0, 1.5, 1.0, 10.0, 0.0, 5.0, 'North');
END

IF NOT EXISTS (SELECT 1 FROM LearningComponent WHERE ComponentId = 'COMP-002')
BEGIN
    INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
    VALUES ('COMP-002', 'LS-001', 1.5, 1.0, 0.8, 15.0, 0.0, 8.0, 'East');
END

IF NOT EXISTS (SELECT 1 FROM LearningComponent WHERE ComponentId = 'COMP-003')
BEGIN
    INSERT INTO LearningComponent (ComponentId, LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
    VALUES ('COMP-003', 'LS-001', 1.0, 0.8, 0.5, 5.0, 0.0, 3.0, 'South');
END
