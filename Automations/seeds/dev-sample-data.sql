-- dev-sample-data.sql
-- --------------------
-- Sample data for every table the backend actually reads: LearningSpace and
-- LearningComponent. Both the tables and the column types are derived from the
-- EF Core mappings in Backend.Infrastructure/EntityConfigurations/ (C# `float`
-- maps to SQL `real`, `HasMaxLength(50)` to NVARCHAR(50), and so on).
--
-- Apply with:
--   ./Automations/insert-sample-data.sh --file Automations/seeds/dev-sample-data.sql
--
-- Re-runnable: it creates the tables when they are missing and replaces only the
-- rows it owns, so anything you insert by hand survives.
--
-- Two constraints come from the domain constructors rather than the database:
--   * every float must be >= 0 (the LearningComponent constructor throws otherwise)
--   * Orientation must be exactly 'North', 'South', 'East', or 'West' (case-sensitive)
-- Break either one and EF throws while materialising the row, so the endpoint
-- answers 500 even though the INSERT itself succeeded.

SET NOCOUNT ON;

-- ---------------------------------------------------------------------------
-- Schema (only created when absent; these two tables have no .sql file under
-- UCR.ECCI.PI.ThemePark.Database/Tables/, which is why a fresh database has
-- neither of them)
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

IF OBJECT_ID('dbo.LearningComponent', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[LearningComponent]
    (
        [ComponentId]     NVARCHAR(50) NOT NULL PRIMARY KEY,
        [LearningSpaceId] NVARCHAR(50) NOT NULL,
        [Width]           REAL         NOT NULL,
        [Height]          REAL         NOT NULL,
        [Depth]           REAL         NOT NULL,
        [X]               REAL         NOT NULL,
        [Y]               REAL         NOT NULL,
        [Z]               REAL         NOT NULL,
        [Orientation]     NVARCHAR(10) NOT NULL,
        -- The domain model has no navigation property; the foreign key just
        -- keeps the seeded data coherent and documents the relationship the
        -- repository query relies on (filtering components by LearningSpaceId).
        CONSTRAINT [FK_LearningComponent_LearningSpace] FOREIGN KEY ([LearningSpaceId])
            REFERENCES [dbo].[LearningSpace] ([id])
    );
END;

-- ---------------------------------------------------------------------------
-- Sample rows
-- ---------------------------------------------------------------------------
DELETE FROM [dbo].[LearningComponent]
 WHERE [LearningSpaceId] IN ('IF-0103', 'IF-0104', 'IF-0201', 'IF-0301');

DELETE FROM [dbo].[LearningSpace]
 WHERE [id] IN ('IF-0103', 'IF-0104', 'IF-0201', 'IF-0301');

-- 'IF-0103' is required by SqlLearningSpaceListRepository.GetCurrentLearningSpaceListAsync,
-- which uses FirstAsync and therefore throws when that row is missing.
INSERT INTO [dbo].[LearningSpace] ([id], [type], [height], [width], [length]) VALUES
    ('IF-0103', 'Classroom',   3.2,  8.5, 12.0),
    ('IF-0104', 'Laboratory',  3.2,  9.0, 14.5),
    ('IF-0201', 'Auditorium',  5.5, 18.0, 24.0),
    ('IF-0301', 'Classroom',   3.0,  7.0, 10.0);   -- deliberately has no components

INSERT INTO [dbo].[LearningComponent]
    ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation]) VALUES
    -- IF-0103: four components, one per orientation
    ('WB-0103-01', 'IF-0103', 4.0,  1.2, 0.05, 0.5, 1.0,  0.1, 'North'),
    ('PR-0103-01', 'IF-0103', 0.35, 0.12, 0.28, 4.2, 2.8,  6.0, 'South'),
    ('SC-0103-01', 'IF-0103', 2.4,  1.8, 0.1,  8.0, 1.5,  6.0, 'East'),
    ('DK-0103-01', 'IF-0103', 1.6,  0.75, 0.8,  0.2, 0.0,  6.0, 'West'),
    -- IF-0104
    ('WB-0104-01', 'IF-0104', 3.5,  1.2, 0.05, 0.5, 1.0,  0.1, 'North'),
    ('PR-0104-01', 'IF-0104', 0.35, 0.12, 0.28, 4.5, 2.9,  7.0, 'South'),
    -- IF-0201
    ('SC-0201-01', 'IF-0201', 6.0,  3.4, 0.15, 9.0, 2.0,  0.2, 'North');

SELECT 'LearningSpace' AS [table], COUNT(*) AS [rows] FROM [dbo].[LearningSpace]
UNION ALL
SELECT 'LearningComponent', COUNT(*) FROM [dbo].[LearningComponent];
