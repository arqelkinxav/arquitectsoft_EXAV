-- 002_roles_usuario.sql
-- Añade el nivel de permiso (rol) a la tabla usuario.
--   rol 0 = Administrador      -> todos los botones + gestión de usuarios
--   rol 1 = Técnico edición    -> todos MENOS Respaldo, Importar y Usuarios
--   rol 2 = Técnico básico     -> solo Análisis, Puertas y Acerca
-- Todos los usuarios (cualquier rol) tienen además "Mi cuenta" para cambiar su clave.
--
-- OJO: MySQL no soporta "ADD COLUMN IF NOT EXISTS"; si ya se aplicó, el ALTER
-- fallará con "Duplicate column name 'rol'" (se puede ignorar).
-- APLICAR también en el servidor de la empresa (10.11.0.254) y en la base beta.

ALTER TABLE `usuario`
  ADD COLUMN `rol` INT NOT NULL DEFAULT 2;

-- Los usuarios administradores existentes pasan a rol 0.
UPDATE `usuario` SET `rol` = 0 WHERE `usuario` IN ('admin', 'dba');
