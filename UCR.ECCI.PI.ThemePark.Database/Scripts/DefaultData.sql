-- Insert basic person records into Person table
INSERT INTO Person (Email, IdentityNumber, FirstName, LastName, BirthDate, Phone)
VALUES
('ana.gomez@ucr.ac.cr', '1-1234-5678', 'Ana', 'Gómez', '1995-04-12', '8888-0001'),
('carlos.rojas@ucr.ac.cr', '2-2345-6789', 'Carlos', 'Rojas', '1988-09-23', '8888-0002'),
('luisa.mora@ucr.ac.cr', '3-3456-7890', 'Luisa', 'Mora', '2002-01-15', '8888-0003'),
('pedro.martinez@ucr.ac.cr', '4-4567-8901', 'Pedro', 'Martínez', '1990-11-05', '8888-0004'),
('maria.lopez@ucr.ac.cr', '5-5678-9012', 'María', 'López', '1999-06-30', '8888-0005');

-- Link some persons to UserAccounts
INSERT INTO UserAccount (UserName, PersonId)
VALUES
('ana.gomez',      (SELECT Id FROM Person WHERE Email = 'ana.gomez@ucr.ac.cr')),
('pedro.martinez', (SELECT Id FROM Person WHERE Email = 'pedro.martinez@ucr.ac.cr')),
('maria.lopez',    (SELECT Id FROM Person WHERE Email = 'maria.lopez@ucr.ac.cr'));

-- Create roles in the system
INSERT INTO Role (Name) VALUES 
('Admin'),
('Editor'),
('Viewer');

-- Define system permissions
INSERT INTO Permission (Description)
VALUES
  ('Access Dashboard'),
  ('Edit Events'),
  ('Delete Users'),
  ('Manage Permissions'),
  ('View Reports');

-- Assign permissions to each role
INSERT INTO RolePermission (RoleId, PermissionId) VALUES (1, 1);
INSERT INTO RolePermission (RoleId, PermissionId) VALUES (1, 2);
INSERT INTO RolePermission (RoleId, PermissionId) VALUES (1, 3);
INSERT INTO RolePermission (RoleId, PermissionId) VALUES (1, 4);
INSERT INTO RolePermission (RoleId, PermissionId) VALUES (1, 5);

INSERT INTO RolePermission (RoleId, PermissionId) VALUES (2, 1);
INSERT INTO RolePermission (RoleId, PermissionId) VALUES (2, 2);
INSERT INTO RolePermission (RoleId, PermissionId) VALUES (2, 5);

INSERT INTO RolePermission (RoleId, PermissionId) VALUES (3, 5);

-- Assign roles to users
INSERT INTO UserRole (RoleId, UserId)
VALUES ((SELECT Id FROM Role WHERE Name = 'Admin'), (SELECT Id FROM UserAccount WHERE UserName = 'ana.gomez'));

INSERT INTO UserRole (RoleId, UserId)
VALUES ((SELECT Id FROM Role WHERE Name = 'Editor'), (SELECT Id FROM UserAccount WHERE UserName = 'pedro.martinez'));

INSERT INTO UserRole (RoleId, UserId)
VALUES ((SELECT Id FROM Role WHERE Name = 'Viewer'), (SELECT Id FROM UserAccount WHERE UserName = 'maria.lopez'));

-- Asuming schemas already exist

-- Insert university and campus data under schema 'Locations'
INSERT INTO [Locations].[University] ([Name], [Country])
VALUES 
    ('University of Costa Rica', 'Costa Rica');

-- Insert areas within each campus
INSERT INTO [Locations].[Campus] ([Name], [Location], [UniversityName])
VALUES 
    ('Rodrigo Facio', 'San Pedro', 'University of Costa Rica'),
    ('Pacifico', 'Puntarenas', 'University of Costa Rica');

-- Insert buildings and link them to areas
INSERT INTO [Locations].[Area] ([Name], [CampusName])
VALUES 
    ('Science and Technology', 'Rodrigo Facio'),
    ('Engineering', 'Rodrigo Facio'),
    ('Marine Biology', 'Pacifico');


INSERT INTO [Infrastructure].[Building] (
    [Name], [Color], [Length], [Width], [Height], [X], [Y], [Z], [AreaName]
)
VALUES 
    ('Physics Lab', 'Blue', 40.5, 20.0, 10.0, 10.123456, 20.654321, 5.000000, 'Science and Technology'),
    ('Electrical Engineering', 'Red', 50.0, 25.0, 12.0, 15.654321, 22.987654, 4.500000, 'Engineering'),
    ('Marine Life Exhibit', 'Green', 30.0, 18.5, 8.0, 12.123123, 25.456789, 3.750000, 'Marine Biology');

-- Store building IDs in variables for later use
DECLARE @BuildingPhysicsLab INT = (SELECT BuildingInternalId FROM [Infrastructure].[Building] WHERE Name = 'Physics Lab');
DECLARE @BuildingEngineering INT = (SELECT BuildingInternalId FROM [Infrastructure].[Building] WHERE Name = 'Electrical Engineering');
DECLARE @BuildingMarine INT = (SELECT BuildingInternalId FROM [Infrastructure].[Building] WHERE Name = 'Marine Life Exhibit');

-- Add floors to each building
INSERT INTO [Infrastructure].[Floor] (BuildingId, Number)
VALUES 
(@BuildingPhysicsLab, 1),
(@BuildingPhysicsLab, 2),
(@BuildingEngineering, 1),
(@BuildingEngineering, 2),
(@BuildingEngineering, 3),
(@BuildingMarine, 1);

-- Obtener IDs de los pisos
DECLARE @FloorPhysics1 INT = (SELECT FloorId FROM [Infrastructure].[Floor] WHERE BuildingId = @BuildingPhysicsLab AND Number = 1);
DECLARE @FloorPhysics2 INT = (SELECT FloorId FROM [Infrastructure].[Floor] WHERE BuildingId = @BuildingPhysicsLab AND Number = 2);
DECLARE @FloorEngineering1 INT = (SELECT FloorId FROM [Infrastructure].[Floor] WHERE BuildingId = @BuildingEngineering AND Number = 1);
DECLARE @FloorEngineering2 INT = (SELECT FloorId FROM [Infrastructure].[Floor] WHERE BuildingId = @BuildingEngineering AND Number = 2);
DECLARE @FloorEngineering3 INT = (SELECT FloorId FROM [Infrastructure].[Floor] WHERE BuildingId = @BuildingEngineering AND Number = 3);
DECLARE @FloorMarine1 INT = (SELECT FloorId FROM [Infrastructure].[Floor] WHERE BuildingId = @BuildingMarine AND Number = 1);

-- Store floor IDs for reference
INSERT INTO [Infrastructure].[LearningSpace] (FloorId, Name, MaxCapacity, Type, Width, Height, Length)
VALUES
(@FloorPhysics1, 'Aula Física 101', 40, 'Classroom', 7.00, 3.20, 9.00),
(@FloorPhysics1, 'Laboratorio Óptica', 30, 'Laboratory', 6.50, 3.40, 8.00),
(@FloorPhysics2, 'Sala de Simulación Cuántica', 15, 'Auditorium', 5.50, 3.30, 6.50),
(@FloorEngineering1, 'Aula Ingeniería 201', 45, 'Classroom', 7.50, 3.20, 9.50),
(@FloorEngineering2, 'Laboratorio Circuitos', 25, 'Laboratory', 6.00, 3.30, 8.00),
(@FloorEngineering3, 'Sala de Robótica', 20, 'Auditorium', 6.00, 3.40, 7.50),
(@FloorMarine1, 'Sala Acuática Interactiva', 35, 'Auditorium', 8.00, 3.00, 10.00);

-- Insert learning spaces (classrooms, labs, auditoriums) per floor
DECLARE @LS_Fisica101 INT = (SELECT LearningSpaceId FROM [Infrastructure].[LearningSpace] WHERE Name = 'Aula Física 101');
DECLARE @LS_Optica INT = (SELECT LearningSpaceId FROM [Infrastructure].[LearningSpace] WHERE Name = 'Laboratorio Óptica');
DECLARE @LS_Cuantica INT = (SELECT LearningSpaceId FROM [Infrastructure].[LearningSpace] WHERE Name = 'Sala de Simulación Cuántica');
DECLARE @LS_Ing201 INT = (SELECT LearningSpaceId FROM [Infrastructure].[LearningSpace] WHERE Name = 'Aula Ingeniería 201');
DECLARE @LS_Circuitos INT = (SELECT LearningSpaceId FROM [Infrastructure].[LearningSpace] WHERE Name = 'Laboratorio Circuitos');
DECLARE @LS_Robotica INT = (SELECT LearningSpaceId FROM [Infrastructure].[LearningSpace] WHERE Name = 'Sala de Robótica');
DECLARE @LS_Acuatica INT = (SELECT LearningSpaceId FROM [Infrastructure].[LearningSpace] WHERE Name = 'Sala Acuática Interactiva');

-- Get LearningSpace IDs for component insertion
INSERT INTO [Infrastructure].[LearningComponent] (LearningSpaceId, Width, Height, Depth, X, Y, Z, Orientation)
VALUES 
(@LS_Fisica101, 0.50, 0.20, 0.40, 1.00, 2.00, 0.00, 'North'), -- Proyector
(@LS_Fisica101, 1.20, 0.90, 0.05, 2.00, 1.00, 0.00, 'East'),   -- Pizarra
(@LS_Fisica101, 1.30, 1.00, 0.05, 2.10, 0.90, 0.00, 'West'),  -- Pizarra
(@LS_Fisica101, 2.50, 1.00, 0.05, 2.10, 0.90, 0.00, 'South'),  -- Pizarra
(@LS_Robotica, 0.60, 0.25, 0.45, 1.50, 2.50, 0.00, 'North'), -- Proyector
(@LS_Robotica, 1.50, 1.00, 0.05, 3.00, 1.50, 0.00, 'West'),  -- Pizarra
(@LS_Ing201, 0.55, 0.22, 0.42, 1.20, 2.30, 0.00, 'South'), -- Proyector
(@LS_Ing201, 0.55, 0.22, 0.42, 1.20, 2.30, 0.00, 'East'); -- Pizarra

-- Get LearningSpace IDs for component insertion
DECLARE @LC_ProyectorFisica INT = (
    SELECT ComponentId FROM [Infrastructure].[LearningComponent]
    WHERE LearningSpaceId = @LS_Fisica101 AND Width = 0.50 AND Height = 0.20 AND Depth = 0.40 
          AND X = 1.00 AND Y = 2.00 AND Z = 0.00 AND Orientation = 'North'
);
DECLARE @LC_PizarraFisica1 INT = (
    SELECT ComponentId FROM [Infrastructure].[LearningComponent]
    WHERE LearningSpaceId = @LS_Fisica101 AND Width = 1.20 AND Height = 0.90 AND Depth = 0.05 
          AND X = 2.00 AND Y = 1.00 AND Z = 0.00 AND Orientation = 'East'
);
DECLARE @LC_PizarraFisica2 INT = (
    SELECT ComponentId FROM [Infrastructure].[LearningComponent]
    WHERE LearningSpaceId = @LS_Fisica101 AND Width = 1.30 AND Height = 1.00 AND Depth = 0.05 
          AND X = 2.10 AND Y = 0.90 AND Z = 0.00 AND Orientation = 'West'
);
DECLARE @LC_PizarraFisica3 INT = (
    SELECT ComponentId FROM [Infrastructure].[LearningComponent]
    WHERE LearningSpaceId = @LS_Fisica101 AND Width = 2.50 AND Height = 1.00 AND Depth = 0.05 
          AND X = 2.10 AND Y = 0.90 AND Z = 0.00 AND Orientation = 'South'
);

DECLARE @LC_ProyectorRobotica INT = (
    SELECT ComponentId FROM [Infrastructure].[LearningComponent]
    WHERE LearningSpaceId = @LS_Robotica AND Width = 0.60 AND Height = 0.25 AND Depth = 0.45 
          AND X = 1.50 AND Y = 2.50 AND Z = 0.00 AND Orientation = 'North'
);
DECLARE @LC_PizarraRobotica INT = (
    SELECT ComponentId FROM [Infrastructure].[LearningComponent]
    WHERE LearningSpaceId = @LS_Robotica AND Width = 1.50 AND Height = 1.00 AND Depth = 0.05 
          AND X = 3.00 AND Y = 1.50 AND Z = 0.00 AND Orientation = 'West'
);

DECLARE @LC_ProyectorIng201 INT = (
    SELECT ComponentId FROM [Infrastructure].[LearningComponent]
    WHERE LearningSpaceId = @LS_Ing201 AND Width = 0.55 AND Height = 0.22 AND Depth = 0.42 
          AND X = 1.20 AND Y = 2.30 AND Z = 0.00 AND Orientation = 'South'
);
DECLARE @LC_PizarraIng201 INT = (
    SELECT ComponentId FROM [Infrastructure].[LearningComponent]
    WHERE LearningSpaceId = @LS_Ing201 AND Width = 0.55 AND Height = 0.22 AND Depth = 0.42 
          AND X = 1.20 AND Y = 2.30 AND Z = 0.00 AND Orientation = 'East'
);

-- Insert projectors
INSERT INTO [Infrastructure].[Projector] (ComponentId, ProjectedContent, ProjectedHeight, ProjectedWidth)
VALUES 
(@LC_ProyectorFisica, 'Presentación de Física', 2.00, 3.00),
(@LC_ProyectorRobotica, 'Simulación de Robótica', 2.20, 3.20),
(@LC_ProyectorIng201, 'Contenido de Ingeniería', 2.10, 3.10);

-- Insert Whiteboards
INSERT INTO [Infrastructure].[Whiteboard] (ComponentId, MarkerColor)
VALUES 
(@LC_PizarraFisica1, 'Blue'),
(@LC_PizarraFisica2, 'Black'),
(@LC_PizarraFisica3, 'Green'),
(@LC_PizarraRobotica, 'Red'),
(@LC_PizarraIng201, 'Black');

