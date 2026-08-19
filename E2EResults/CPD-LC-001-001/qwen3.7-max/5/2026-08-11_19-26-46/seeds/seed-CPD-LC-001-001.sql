-- Seed script for CPD-LC-001-001: Learning Components
-- Creates the LearningComponent table and inserts sample data

IF OBJECT_ID('dbo.LearningComponent', 'U') IS NULL
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
GO

-- Insert sample learning components for learning space LS-001
INSERT INTO [dbo].[LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES
    (N'COMP-001', N'LS-001', 1.5, 1.0, 0.5, 0.0, 0.0, 0.0, N'North'),
    (N'COMP-002', N'LS-001', 2.0, 1.5, 0.3, 2.0, 0.0, 0.0, N'South'),
    (N'COMP-003', N'LS-001', 1.0, 1.0, 1.0, 4.0, 0.0, 0.0, N'East');
GO

-- Insert components for a different learning space to test filtering
INSERT INTO [dbo].[LearningComponent] ([ComponentId], [LearningSpaceId], [Width], [Height], [Depth], [X], [Y], [Z], [Orientation])
VALUES
    (N'COMP-004', N'LS-002', 3.0, 2.0, 0.5, 0.0, 0.0, 0.0, N'West');
GO
