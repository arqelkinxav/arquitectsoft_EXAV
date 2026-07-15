-- =============================================================
-- Migración 003 — Puesta a punto de la base BETA (arquitectdb_beta)
-- Fecha: 2026-07-06
--
-- CONTEXTO: la beta 4.0.2 se instala EN PARALELO a la estable en la
-- empresa, apuntando a una base APARTE `arquitectdb_beta` en el mismo
-- servidor 10.11.0.254. Así las tablas scratch (proyecto/proyecto_pt)
-- y el esquema propio de la beta NUNCA tocan los datos reales.
--
-- Este script se ejecuta CONTRA `arquitectdb_beta` DESPUÉS de haberla
-- llenado con una copia de `arquitectdb` (ver instrucciones de despliegue).
-- Añade lo que la estable NO tiene y la beta necesita:
--   1) columna `rol` en usuario   (OBLIGATORIO: el login de la beta la lee)
--   2) tabla beta_dependencias_acabado  (para dependencias de acabado MOD)
--   3) índices de rendimiento (001)     (recomendado)
--   4) [OPCIONAL] renombrar placeholders a MOD01/MOD02
--
-- Es idempotente en la práctica: si una parte ya se aplicó, ese ALTER
-- fallará con "Duplicate..."; se puede ignorar y seguir con el resto.
-- =============================================================

USE arquitectdb_beta;

-- --- 1) ROL DE USUARIO (OBLIGATORIO) --------------------------------
-- Sin esta columna el login de la beta falla (QUERY_EXITS_USUARIO lee `rol`).
-- OJO: MySQL no soporta ADD COLUMN IF NOT EXISTS. Si ya existe, ignorar el error.
ALTER TABLE `usuario`
  ADD COLUMN `rol` INT NOT NULL DEFAULT 2;

-- Administradores existentes -> rol 0 (todo + gestión de usuarios).
UPDATE `usuario` SET `rol` = 0 WHERE `usuario` IN ('admin', 'dba');


-- --- 2) TABLA DE DEPENDENCIAS DE ACABADO (feature beta) --------------
CREATE TABLE IF NOT EXISTS `beta_dependencias_acabado` (
  `Id`              int NOT NULL AUTO_INCREMENT,
  `Cod_Placeholder` varchar(20) NOT NULL,
  `Cod_Perfileria`  varchar(20) NOT NULL,
  `Cod_Resultado`   varchar(20) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `uq_dep` (`Cod_Placeholder`, `Cod_Perfileria`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


-- --- 3) ÍNDICES DE RENDIMIENTO (recomendado) ------------------------
-- Si ya existen, cada ALTER falla con "Duplicate key name"; ignorar.
ALTER TABLE componentes_detalle ADD INDEX idx_cd_componente      (Id_Componente);
ALTER TABLE componentes_detalle ADD INDEX idx_cd_subcomponente   (Id_Subcomponente);
ALTER TABLE componentes         ADD INDEX idx_comp_codigo        (Codigo);
ALTER TABLE proyecto            ADD INDEX idx_proy_subcomponente (Id_Subcomponente);


-- --- 4) [OPCIONAL] RENOMBRAR PLACEHOLDERS A MOD01 / MOD02 ------------
-- Solo si quieres adoptar la convención MOD en esta base. Se hace por
-- CÓDIGO (no por Id) para no depender de que los Id coincidan con local.
-- Descomenta si lo quieres. Las reglas de dependencia las defines luego
-- desde la pantalla "Dependencias de acabado" de la beta.
--
-- UPDATE `acabados` SET `Codigo_Homologacion` = 'MOD01' WHERE `Codigo_Homologacion` = '000051';
-- UPDATE `acabados` SET `Codigo_Homologacion` = 'MOD02' WHERE `Codigo_Homologacion` = '00051';
