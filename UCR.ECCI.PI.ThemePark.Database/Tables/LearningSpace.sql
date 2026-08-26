CREATE TABLE [dbo].[LearningSpace](
    [LearningSpaceId] [int] NOT NULL,
    [Type] [nvarchar](50) NOT NULL,
    [Height] [float] NOT NULL,
    [Width] [float] NOT NULL,
    [Length] [float] NOT NULL,
    CONSTRAINT [PK_LearningSpace] PRIMARY KEY CLUSTERED ([LearningSpaceId] ASC)
);
