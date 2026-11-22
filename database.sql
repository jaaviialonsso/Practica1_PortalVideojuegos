-- =====================================================================
-- database.sql - Práctica 3 Desarrollo de Interfaces
-- Catálogo de videojuegos: acceso a BBDD MySQL
-- Ejecutar completo en MySQL Workbench (como root) o desde consola:
--     mysql -u root -p < database.sql
-- =====================================================================

-- Se borra y se vuelve a crear para poder repetir el script sin errores.
-- OJO: elimina la base de datos si ya existía.
DROP DATABASE IF EXISTS catalogo_videojuegos;

CREATE DATABASE catalogo_videojuegos CHARACTER SET utf8mb4;
USE catalogo_videojuegos;

CREATE TABLE Usuarios (
    id             INT AUTO_INCREMENT PRIMARY KEY,
    nombre_usuario VARCHAR(50)  NOT NULL,
    -- utf8mb4_bin compara byte a byte: la contraseña distingue mayúsculas
    -- y minúsculas ("DAM" no vale para "dam").
    password       VARCHAR(255) NOT NULL COLLATE utf8mb4_bin,
    -- No se permiten nombres de usuario repetidos. Si se intenta,
    -- MySQL devuelve el error 1062, que la aplicación captura.
    CONSTRAINT uq_usuarios_nombre UNIQUE (nombre_usuario)
);

-- Usuarios de prueba (los mismos que tenía la aplicación en memoria)
INSERT INTO Usuarios (nombre_usuario, password) VALUES
    ('admin',  '1234'),
    ('alumno', 'dam');

-- Usuario de MySQL que usa la aplicación, solo con los permisos necesarios
DROP USER IF EXISTS 'catalogo_app'@'localhost';
CREATE USER 'catalogo_app'@'localhost' IDENTIFIED BY 'Catalogo_2025!';
GRANT SELECT, INSERT ON catalogo_videojuegos.Usuarios TO 'catalogo_app'@'localhost';

SELECT * FROM Usuarios;
