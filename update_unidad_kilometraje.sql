-- Actualizar todos los registros existentes con UnidadKilometraje = 'Km'
UPDATE Autos
SET UnidadKilometraje = 'Km'
WHERE UnidadKilometraje = '' OR UnidadKilometraje IS NULL;

-- Verificar los cambios
SELECT COUNT(*) as TotalRegistros, UnidadKilometraje
FROM Autos
GROUP BY UnidadKilometraje;
