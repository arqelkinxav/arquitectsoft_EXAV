-- =====================================================================
-- 007_botones_por_perfil.sql
--
-- PERFILES de permiso: cada usuario lleva uno (columna usuario.rol, que
-- ya existia) y cada perfil dice que botones de la barra lateral ve.
-- Los edita, crea y borra el administrador desde la pantalla Usuarios.
--
--   rol 0 = Administrador: fijo, lo ve todo, no se guarda aqui.
--   rol 1 = Tecnico (edicion) y rol 2 = Tecnico (basico): los de siempre,
--           ahora editables. Los nuevos toman Id 3, 4...
--
-- beta_rol_boton: una fila por (perfil, boton). Si falta la fila, el
-- boton se ve como antes de existir esto (Generals/BotonesPerfil.cs,
-- PorDefecto): asi los botones nuevos no desaparecen solos.
--
-- Prefijo beta_ como las demas: viajan con el Respaldo de Olimpo y el
-- Importar de la beta las sobrescribe (Olimpo manda). La tabla usuario
-- no se toca.
--
-- Idempotente.
-- =====================================================================

CREATE TABLE IF NOT EXISTS beta_perfil (
  Id      INT          NOT NULL AUTO_INCREMENT,
  Nombre  VARCHAR(60)  NOT NULL,
  PRIMARY KEY (Id),
  UNIQUE KEY uq_beta_perfil_nombre (Nombre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO beta_perfil (Id, Nombre) VALUES
  (1, 'Técnico (edición)'),
  (2, 'Técnico (básico)');

CREATE TABLE IF NOT EXISTS beta_rol_boton (
  Rol      INT          NOT NULL,
  Boton    VARCHAR(40)  NOT NULL,
  Visible  TINYINT(1)   NOT NULL DEFAULT 1,
  PRIMARY KEY (Rol, Boton)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
