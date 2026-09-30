-- Seed data for PQL-AE-001-001 (Add Building)
-- This script seeds sample building data for end-to-end testing

-- Insert test buildings
INSERT INTO Building (InternalId, Name, Color, Height, Length, Width, X, Y, Z)
VALUES 
    (1, 'Engineering Building', 'Red', 20.5, 50.0, 30.0, 100.0, 200.0, 0.0),
    (2, 'Science Building', 'Blue', 15.0, 40.0, 25.0, 200.0, 300.0, 0.0),
    (3, 'Library', 'Green', 10.0, 60.0, 35.0, 300.0, 400.0, 0.0);
