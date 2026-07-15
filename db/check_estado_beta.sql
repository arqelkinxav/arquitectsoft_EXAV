-- =============================================================
-- DIAGNÓSTICO (solo lectura) de la base BETA `arquitectdb_beta`
-- Correr EN LA PC DE LA EMPRESA (por escritorio remoto), contra el
-- servidor local de allá. NO modifica nada: solo consulta.
--
-- Objetivo: confirmar que la beta tiene lo que el .exe nuevo espera:
--   1) la base arquitectdb_beta existe
--   2) usuario.rol      (login por roles)
--   3) usuario.Nombre   (nombre editable / "Mi cuenta")
--   4) tabla beta_dependencias_acabado (dependencias de acabado)
-- =============================================================

-- 1) ¿Existe la base beta?
SHOW DATABASES LIKE 'arquitectdb_beta';

USE arquitectdb_beta;

-- 2 y 3) Columnas de la tabla usuario: buscamos 'rol' y 'Nombre'
SELECT COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'arquitectdb_beta'
  AND TABLE_NAME   = 'usuario'
  AND COLUMN_NAME IN ('rol', 'Nombre');
-- Esperado: 2 filas (rol y Nombre). Si falta alguna, ver la sección FIX abajo.

-- 4) ¿Existe la tabla de dependencias?
SHOW TABLES LIKE 'beta_dependencias_acabado';

-- Extra: ver los usuarios y su rol actual
SELECT id, usuario, Nombre, rol FROM usuario;
