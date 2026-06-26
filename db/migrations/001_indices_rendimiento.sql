-- =============================================================
-- Migración 001 — Índices de rendimiento
-- Fecha: 2026-06-26
--
-- Problema: las tablas no tenían índices en las columnas usadas
-- en los JOIN/WHERE de los procedimientos del análisis. Cada
-- búsqueda hacía un escaneo completo (ej. componentes_detalle con
-- 8.000+ filas), y al ocurrir dentro de bucles por fila, se
-- multiplicaba. En proyectos grandes el auto-join de `proyecto`
-- por Id_Subcomponente era cuadrático (posible causa del error
-- con proyectos gigantes).
--
-- Medición (join componentes_detalle): 8.427 filas escaneadas -> 14.
--
-- Aplicar en CUALQUIER servidor donde corra la base (local,
-- empresa, Hetzner). Es reversible (DROP INDEX) y no toca datos.
-- =============================================================

ALTER TABLE componentes_detalle ADD INDEX idx_cd_componente    (Id_Componente);
ALTER TABLE componentes_detalle ADD INDEX idx_cd_subcomponente (Id_Subcomponente);
ALTER TABLE componentes          ADD INDEX idx_comp_codigo      (Codigo);
ALTER TABLE proyecto             ADD INDEX idx_proy_subcomponente (Id_Subcomponente);

-- Para revertir:
-- ALTER TABLE componentes_detalle DROP INDEX idx_cd_componente;
-- ALTER TABLE componentes_detalle DROP INDEX idx_cd_subcomponente;
-- ALTER TABLE componentes          DROP INDEX idx_comp_codigo;
-- ALTER TABLE proyecto             DROP INDEX idx_proy_subcomponente;
