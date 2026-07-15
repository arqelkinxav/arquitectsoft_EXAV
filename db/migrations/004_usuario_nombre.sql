-- =============================================================
-- Migración 004 — Columna `Nombre` en usuario (arquitectdb_beta)
-- Fecha: 2026-07-15
--
-- CONTEXTO: la beta añade edición del "nombre legible" del usuario
-- (pantalla "Mi cuenta" -> UsuarioDto.CambiarNombre, QUERY_CAMBIAR_NOMBRE
-- hace UPDATE usuario SET Nombre = ...). Si la tabla `usuario` NO tiene
-- la columna `Nombre`, esa función falla.
--
-- Correr SOLO si el diagnóstico (check_estado_beta.sql) mostró que
-- `Nombre` no existe. Se ejecuta contra `arquitectdb_beta`.
--
-- MySQL no soporta ADD COLUMN IF NOT EXISTS: si ya existe, este ALTER
-- falla con "Duplicate column name 'Nombre'"; ignorar y seguir.
-- =============================================================

USE arquitectdb_beta;

ALTER TABLE `usuario`
  ADD COLUMN `Nombre` VARCHAR(100) NOT NULL DEFAULT '';

-- Opcional: precargar el nombre igual al login para que no salga vacío.
UPDATE `usuario` SET `Nombre` = `usuario` WHERE `Nombre` = '';
