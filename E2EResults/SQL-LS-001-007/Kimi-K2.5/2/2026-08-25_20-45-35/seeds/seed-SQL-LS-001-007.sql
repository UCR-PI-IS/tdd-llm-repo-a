-- Seed data for SQL-LS-001-007 end-to-end testing
-- Insert 3 learning spaces with valid data

IF NOT EXISTS (SELECT 1 FROM [dbo].[LearningSpace] WHERE [LearningSpaceId] = 1)
BEGIN
    INSERT INTO [dbo].[LearningSpace] ([LearningSpaceId], [Type], [Height], [Width], [Length])
    VALUES 
        (1, 'Classroom', 3.0, 8.0, 10.0),
        (2, 'Auditorium', 5.0, 15.0, 20.0),
        (3, 'Laboratory', 3.5, 12.0, 15.0);
END
