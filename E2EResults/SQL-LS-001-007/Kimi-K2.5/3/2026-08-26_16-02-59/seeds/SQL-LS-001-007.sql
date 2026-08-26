-- Create LearningSpace table if it doesn't exist
IF OBJECT_ID('dbo.LearningSpace', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[LearningSpace]
    (
        [id] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [type] NVARCHAR(50) NOT NULL,
        [height] FLOAT NOT NULL,
        [width] FLOAT NOT NULL,
        [length] FLOAT NOT NULL
    );
END;

-- Insert sample learning spaces
INSERT INTO [dbo].[LearningSpace] ([id], [type], [height], [width], [length])
VALUES 
    ('IF-0101', 'Classroom', 3.0, 8.0, 10.0),
    ('IF-0201', 'Auditorium', 5.0, 15.0, 20.0),
    ('IF-0301', 'Laboratory', 3.5, 12.0, 15.0);
