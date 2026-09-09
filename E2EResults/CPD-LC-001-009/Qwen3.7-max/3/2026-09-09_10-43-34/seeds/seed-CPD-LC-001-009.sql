-- Seed SQL for CPD-LC-001-009: Automatic ID generation for LearningComponent
-- Derived from Backend.Domain/Entities/LearningComponent.cs and
-- Backend.Infrastructure/EntityConfigurations/LearningComponentEntityConfiguration.cs
-- Table: LearningComponent, Key: ComponentId (NVARCHAR(50))

IF OBJECT_ID('dbo.LearningComponent','U') IS NULL
CREATE TABLE [dbo].[LearningComponent] (
    [ComponentId]     NVARCHAR(50)  NOT NULL,
    [LearningSpaceId] NVARCHAR(50)  NOT NULL,
    [Width]           REAL          NOT NULL,
    [Height]          REAL          NOT NULL,
    [Depth]           REAL          NOT NULL,
    [X]               REAL          NOT NULL,
    [Y]               REAL          NOT NULL,
    [Z]               REAL          NOT NULL,
    [Orientation]     NVARCHAR(20)  NOT NULL,
    CONSTRAINT [PK_LearningComponent] PRIMARY KEY CLUSTERED ([ComponentId] ASC)
);

-- Seed 3 rows with valid data (all dimensions non-negative, valid orientations)
INSERT INTO [dbo].[LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES
    ('COMP-SEED-001', 'LS-001', 2.5, 1.5, 0.5, 10.0, 20.0, 0.0, 'North'),
    ('COMP-SEED-002', 'LS-001', 3.0, 2.0, 1.0, 15.0, 25.0, 0.0, 'South'),
    ('COMP-SEED-003', 'LS-002', 1.5, 1.0, 0.5,  5.0, 10.0, 0.0, 'East');
