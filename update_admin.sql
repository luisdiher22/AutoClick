-- Actualizar el usuario admin@admin.com para que sea administrador
UPDATE Usuarios 
SET EsAdministrador = 1 
WHERE Email = 'admin@admin.com';

-- Verificar la actualización
SELECT Email, Nombre, Apellidos, EsAdministrador 
FROM Usuarios 
WHERE Email = 'admin@admin.com';