CREATE TABLE [dbo].[LearningComponent] (
    [ComponentId]      NVARCHAR (50)  NOT NULL,
    [LearningSpaceId]  NVARCHAR (50)  NOT NULL,
    [Width]            FLOAT (53)     NOT NULL,
    [Height]           FLOAT (53)     NOT NULL,
    [Depth]            FLOAT (53)     NOT NULL,
    [X]                FLOAT (53)     NOT NULL,
    [Y]                FLOAT (53)     NOT NULL,
    [Z]                FLOAT (53)     NOT NULL,
    [Orientation]      NVARCHAR (20)  NOT NULL,
    CONSTRAINT [PK_LearningComponent] PRIMARY KEY CLUSTERED ([ComponentId] ASC)
);
