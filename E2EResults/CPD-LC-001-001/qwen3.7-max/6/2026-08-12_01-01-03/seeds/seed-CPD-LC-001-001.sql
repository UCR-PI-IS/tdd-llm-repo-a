-- Seed data for CPD-LC-001-001: Learning Components listing
-- Table: LearningComponent (from LearningComponentEntityConfiguration)

IF OBJECT_ID('dbo.LearningComponent','U') IS NULL
CREATE TABLE [dbo].[LearningComponent] (
    [ComponentId]     NVARCHAR(50)  NOT NULL,
    [LearningSpaceId] NVARCHAR(50)  NULL,
    [Width]           REAL          NOT NULL,
    [Height]          REAL          NOT NULL,
    [Depth]           REAL          NOT NULL,
    [X]               REAL          NOT NULL,
    [Y]               REAL          NOT NULL,
    [Z]               REAL          NOT NULL,
    [Orientation]     NVARCHAR(20)  NULL,
    CONSTRAINT [PK_LearningComponent] PRIMARY KEY ([ComponentId])
);

-- Seed 3 rows for learning space IF-0103 (the story's specific space)
INSERT INTO [dbo].[LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES
    ('LC-001', 'IF-0103', 2.0, 1.5, 0.5, 1.0, 2.0, 0.0, 'North'),
    ('LC-002', 'IF-0103', 1.0, 1.0, 0.3, 3.0, 4.0, 0.0, 'South'),
    ('LC-003', 'IF-0103', 0.8, 0.6, 0.2, 5.0, 6.0, 0.0, 'East');
