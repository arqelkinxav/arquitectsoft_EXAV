-- =====================================================================
-- 009_perfiles_ocultos.sql
--
-- PERFILES OCULTOS (no se ven: estructura interna, soporte). Ej.: el
-- ZOCALO OX (IMC0004). En el Analisis, sus tramos recopilatorios no usan
-- la Medida Base de 2960: usan la "Medida base ocultos", que el programa
-- propone para enviar lo minimo a obra (los empalmes dan igual porque no
-- se ven). Engine/PerfilesOcultos.cs.
--
-- Una fila por CODIGO BASE (subcomponentes.Codigo_Homologacion, sin el
-- acabado): marcar IMC0004 vale para IMC0004-72, -01, -50...
-- Se marca con la casilla "Oculto" de la pantalla Subcomponentes.
--
-- Prefijo beta_ como las demas: viaja con el Respaldo de Olimpo y el
-- Importar de la beta la sobrescribe (Olimpo manda). Si la tabla no
-- existe, el programa sigue como siempre (ningun perfil oculto).
--
-- Idempotente.
-- =====================================================================

CREATE TABLE IF NOT EXISTS beta_perfil_oculto (
  Codigo  VARCHAR(50)  NOT NULL,
  PRIMARY KEY (Codigo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- El primero: zocalo OX.
INSERT IGNORE INTO beta_perfil_oculto (Codigo) VALUES ('IMC0004');
