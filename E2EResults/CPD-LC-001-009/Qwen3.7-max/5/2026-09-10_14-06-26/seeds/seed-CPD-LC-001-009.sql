-- Seed data for story CPD-LC-001-009: LearningComponent auto-generated IDs
-- Table: LearningComponent (from EF configuration)
-- Columns: ComponentId (NVARCHAR(50) PK), LearningSpaceId (NVARCHAR(50)),
--          Width (REAL), Height (REAL), Depth (REAL),
--          X (REAL), Y (REAL), Z (REAL), Orientation (NVARCHAR(20))

IF OBJECT_ID('dbo.LearningComponent', 'U') IS NULL
CREATE TABLE [dbo].[LearningComponent] (
    [ComponentId]     NVARCHAR(50)  NOT NULL PRIMARY KEY,
    [LearningSpaceId] NVARCHAR(50)  NOT NULL,
    [Width]           REAL          NOT NULL,
    [Height]          REAL          NOT NULL,
    [Depth]           REAL          NOT NULL,
    [X]               REAL          NOT NULL,
    [Y]               REAL          NOT NULL,
    [Z]               REAL          NOT NULL,
    [Orientation]     NVARCHAR(20)  NOT NULL
);

INSERT INTO [dbo].[LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES
    (N'COMP-001', N'LS-001', 1.5, 1.0, 0.5, 10.0, 5.0, 0.0, N'North'),
    (N'COMP-002', N'LS-001', 2.0, 1.5, 0.8, 15.0, 10.0, 0.0, N'South'),
    (N'COMP-003', N'LS-002', 3.0, 2.0, 1.0, 20.0, 15.0, 0.0, N'East');
