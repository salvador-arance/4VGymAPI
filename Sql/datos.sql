-- =====================================================================
-- Datos de prueba de la BBDD 4vGym
-- Las tablas las crea Entity Framework (dotnet ef database update).
-- Ejecutar UNA vez sobre la BBDD recién creada: así los Id autonuméricos
-- empiezan en 1 y coinciden con los que usa el fichero 4VGymAPI.http.
-- =====================================================================

-- 1. Tipos de actividad (Id 1 a 4)
INSERT INTO ActivityTypes (Name, NumberMonitors) VALUES
    (N'BodyPump', 2),
    (N'Spinning', 1),
    (N'Pilates',  1),
    (N'CrossFit', 3);

-- 2. Monitores (Id 1 a 5)
INSERT INTO Instructors (Name, Email, PhotoUrl, Phone) VALUES
    (N'Ana García',   N'ana@example.com',    N'https://example.com/ana.jpg',    N'+34600111222'),
    (N'Luis Pérez',   N'luis@example.com',   N'https://example.com/luis.jpg',   N'+34600333444'),
    (N'Marta López',  N'marta@example.com',  N'https://example.com/marta.jpg',  N'+34600555666'),
    (N'Miguel Ruiz',  N'miguel@example.com', N'https://example.com/miguel.jpg', N'+34600777888'),
    (N'María Martín', N'maria@example.com',  N'https://example.com/maria.jpg',  NULL); -- el teléfono es opcional en la BBDD

-- 3. Actividades (Id 1 a 4)
INSERT INTO Activities (ActivityTypeId, MaxParticipants, DateStart, DateEnd) VALUES
    (1, 20, '2030-01-15T10:00:00', '2030-01-15T11:00:00'), -- BodyPump
    (2, 15, '2030-01-15T10:00:00', '2030-01-15T11:00:00'), -- Spinning
    (3, 10, '2030-01-15T12:00:00', '2030-01-15T13:00:00'), -- Pilates
    (4, 25, '2030-01-16T18:00:00', '2030-01-16T19:30:00'); -- CrossFit

-- 4. Monitores de cada actividad (tabla intermedia de la relación M:N)
INSERT INTO ActivityInstructor (ActivitiesId, InstructorsId) VALUES
    (1, 1), (1, 2),          -- BodyPump: Ana y Luis
    (2, 3),                  -- Spinning: Marta
    (3, 1),                  -- Pilates: Ana (después de BodyPump, no se solapa)
    (4, 2), (4, 3), (4, 4);  -- CrossFit: Luis, Marta y Miguel