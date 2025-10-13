-- Actualizar el usuario admin@gmail.com para que sea administrador
UPDATE Usuarios 
SET EsAdministrador = 1 
WHERE Email = 'admin@gmail.com';

-- Verificar la actualización
SELECT Email, Nombre, Apellidos, EsAdministrador 
FROM Usuarios 
WHERE Email = 'admin@gmail.com';