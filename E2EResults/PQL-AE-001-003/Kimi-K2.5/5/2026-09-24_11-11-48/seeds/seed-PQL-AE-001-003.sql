IF OBJECT_ID('dbo.Universities','U') IS NULL
CREATE TABLE Universities (
    Name NVARCHAR(200) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Universities PRIMARY KEY (Name)
);

INSERT INTO Universities (Name, Country) VALUES
('Universidad de Costa Rica', 'Costa Rica'),
('Tecnologico de Costa Rica', 'Costa Rica'),
('Universidad Nacional', 'Costa Rica');
