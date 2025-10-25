-- Script para actualizar los banderines de los autos existentes
-- Asigna banderines aleatorios a los autos que tienen PlanVisibilidad > 1

-- Actualizar autos con diferentes banderines según su PlanVisibilidad
-- BanderinAdquirido puede ser de 1 a 16 según la lista en Auto.cs

-- Plan Super (PlanVisibilidad = 2): Asignar banderines básicos
UPDATE Autos 
SET BanderinAdquirido = 5  -- "PERFECTO ESTADO"
WHERE PlanVisibilidad = 2 AND BanderinAdquirido = 0 AND Id % 3 = 0;

UPDATE Autos 
SET BanderinAdquirido = 11  -- "IMPECABLE"
WHERE PlanVisibilidad = 2 AND BanderinAdquirido = 0 AND Id % 3 = 1;

UPDATE Autos 
SET BanderinAdquirido = 6  -- "AL DÍA"
WHERE PlanVisibilidad = 2 AND BanderinAdquirido = 0 AND Id % 3 = 2;

-- Plan Ultra (PlanVisibilidad = 3): Asignar banderines premium
UPDATE Autos 
SET BanderinAdquirido = 1  -- "Versión Americana"
WHERE PlanVisibilidad = 3 AND BanderinAdquirido = 0 AND Id % 4 = 0;

UPDATE Autos 
SET BanderinAdquirido = 2  -- "ÚNICO DUEÑO"
WHERE PlanVisibilidad = 3 AND BanderinAdquirido = 0 AND Id % 4 = 1;

UPDATE Autos 
SET BanderinAdquirido = 3  -- "FULL EXTRAS"
WHERE PlanVisibilidad = 3 AND BanderinAdquirido = 0 AND Id % 4 = 2;

UPDATE Autos 
SET BanderinAdquirido = 7  -- "BAJO KILOMETRAJE"
WHERE PlanVisibilidad = 3 AND BanderinAdquirido = 0 AND Id % 4 = 3;

-- Plan Profesional (PlanVisibilidad = 4): Asignar los mejores banderines
UPDATE Autos 
SET BanderinAdquirido = 4  -- "MANTENIMIENTO DE AGENCIA"
WHERE PlanVisibilidad = 4 AND BanderinAdquirido = 0 AND Id % 5 = 0;

UPDATE Autos 
SET BanderinAdquirido = 9  -- "FINANCIAMIENTO DISPONIBLE"
WHERE PlanVisibilidad = 4 AND BanderinAdquirido = 0 AND Id % 5 = 1;

UPDATE Autos 
SET BanderinAdquirido = 12  -- "CERO DETALLES"
WHERE PlanVisibilidad = 4 AND BanderinAdquirido = 0 AND Id % 5 = 2;

UPDATE Autos 
SET BanderinAdquirido = 14  -- "REGISTRO LIMPIO"
WHERE PlanVisibilidad = 4 AND BanderinAdquirido = 0 AND Id % 5 = 3;

UPDATE Autos 
SET BanderinAdquirido = 16  -- "RECIBO"
WHERE PlanVisibilidad = 4 AND BanderinAdquirido = 0 AND Id % 5 = 4;

-- Verificar los cambios
SELECT Id, Marca, Modelo, PlanVisibilidad, BanderinAdquirido 
FROM Autos 
WHERE Activo = 1 AND PlanVisibilidad > 1
ORDER BY PlanVisibilidad DESC, Id;
