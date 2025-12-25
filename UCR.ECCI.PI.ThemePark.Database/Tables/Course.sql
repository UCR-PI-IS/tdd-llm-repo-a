CREATE TABLE [dbo].[Course]
(
	[Id] NVARCHAR(50) NOT NULL PRIMARY KEY,
	
	[SemesterId] Nvarchar(50) not NULL,
	CONSTRAINT FK_CourseSemester FOREIGN KEY (SemesterId) REFERENCES [Semester]([Id]),
)
