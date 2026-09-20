-- CPD-LC-001-005.sql
-- -----------------
-- Seed data for story CPD-LC-001-005: Update a whiteboard.
-- Creates the LearningSpace and Whiteboard tables (if absent) and inserts
-- sample rows for end-to-end validation of the PUT /api/whiteboards endpoint.

SET NOCOUNT ON;

-- ---------------------------------------------------------------------------
-- Schema
-- ---------------------------------------------------------------------------
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[LearningSpace]
    (
        [id]     NVARCHAR(50) NOT NULL PRIMARY KEY,
        [type]   NVARCHAR(50) NOT NULL,
        [height] REAL         NOT NULL,
        [width]  REAL         NOT NULL,
        [length] REAL         NOT NULL
    );
END;

IF OBJECT_ID('dbo.Whiteboard', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Whiteboard]
    (
        [ComponentId]     NVARCHAR(50)  NOT NULL PRIMARY KEY,
        [LearningSpaceId] NVARCHAR(50)  NOT NULL,
        [Width]           REAL          NOT NULL,
        [Height]          REAL          NOT NULL,
        [Depth]           REAL          NOT NULL,
        [X]               REAL          NOT NULL,
        [Y]               REAL          NOT NULL,
        [Z]               REAL          NOT NULL,
        [Orientation]     NVARCHAR(20)  NOT NULL,
        [MarkerColor]     NVARCHAR(50)  NOT NULL,
        CONSTRAINT [FK_Whiteboard_LearningSpace] FOREIGN KEY ([LearningSpaceId])
            REFERENCES [dbo].[LearningSpace] ([id])
    );
END;

-- ---------------------------------------------------------------------------
-- Sample rows
-- ---------------------------------------------------------------------------
DELETE FROM [dbo].[Whiteboard]
 WHERE [LearningSpaceId] IN ('LS-001', 'LS-002');

DELETE FROM [dbo].[LearningSpace]
 WHERE [id] IN ('LS-001', 'LS-002');

INSERT INTO [dbo].[LearningSpace] ([id], [type], [height], [width], [length]) VALUES
    ('LS-001', 'Classroom',  3.0, 10.0, 10.0),
    ('LS-002', 'Laboratory', 3.5, 12.0, 15.0);

INSERT INTO [dbo].[Whiteboard]
    ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation], [MarkerColor]) VALUES
    ('WB-001', 'LS-001', 2.0, 1.5, 0.1, 1.0, 0.0, 2.0, 'South', 'Blue'),
    ('WB-002', 'LS-001', 2.0, 1.5, 0.1, 5.0, 0.0, 5.0, 'South', 'Green'),
    ('WB-003', 'LS-002', 3.0, 2.0, 0.1, 1.0, 1.0, 1.0, 'North', 'Red');

SELECT 'LearningSpace' AS [table], COUNT(*) AS [rows] FROM [dbo].[LearningSpace]
UNION ALL
SELECT 'Whiteboard', COUNT(*) FROM [dbo].[Whiteboard];
