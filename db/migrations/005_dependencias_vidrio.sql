-- =====================================================================
-- 005_dependencias_vidrio.sql   (BETA)
--
-- Dependencias de VIDRIO: en la base, cada componente tiene sus
-- subcomponentes creados como si todo el proyecto fuese el tipo de
-- vidrio ESTANDAR de su sistema (6+6, o 5+5/c/6+6 en los dobles).
-- Cuando un proyecto pide otro tipo, hay que sustituir un juego de
-- subcomponentes por otro DENTRO de ese sistema: el vidrio y todo lo
-- que arrastra (calces, gomas, cintas, perfiles).
--
-- Tres tablas:
--   beta_vidrio_tipo    catalogo de tipos (3+3, 5+5, 6+6, 5+5/c/6+6...)
--   beta_vidrio_sistema prefijo del sistema (DV, IT, AV...) + su estandar
--   beta_vidrio_regla   sustitucion subcomponente -> subcomponente,
--                       por (sistema, tipo destino)
--
-- Van con prefijo beta_ y SIN claves foraneas a las tablas del catalogo,
-- igual que beta_dependencias_acabado: el dump de la empresa es a nivel
-- de tabla (DROP/CREATE/INSERT por tabla) y no las referencia, asi que
-- sobreviven a los imports.
--
-- Idempotente: se puede ejecutar varias veces.
-- =====================================================================

-- Tipos de vidrio (catalogo)
CREATE TABLE IF NOT EXISTS beta_vidrio_tipo (
  Id      INT          NOT NULL AUTO_INCREMENT,
  Nombre  VARCHAR(60)  NOT NULL,
  Orden   INT          NOT NULL DEFAULT 0,
  PRIMARY KEY (Id),
  UNIQUE KEY uq_vidrio_tipo_nombre (Nombre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Sistemas: el prefijo con el que empieza el codigo del componente.
-- Id_Tipo_Estandar = lo que HOY esta cargado en la base para ese sistema.
CREATE TABLE IF NOT EXISTS beta_vidrio_sistema (
  Id               INT          NOT NULL AUTO_INCREMENT,
  Prefijo          VARCHAR(20)  NOT NULL,
  Descripcion      VARCHAR(120) NULL,
  Id_Tipo_Estandar INT          NULL,
  PRIMARY KEY (Id),
  UNIQUE KEY uq_vidrio_sistema_prefijo (Prefijo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Reglas: para (sistema, tipo destino), este subcomponente pasa a ser este otro.
-- Se guardan los Id_SubComponente (la clave real); el codigo y la descripcion
-- se sacan por JOIN, asi renombrar un subcomponente no rompe la regla.
CREATE TABLE IF NOT EXISTS beta_vidrio_regla (
  Id             INT NOT NULL AUTO_INCREMENT,
  Id_Sistema     INT NOT NULL,
  Id_Tipo        INT NOT NULL,
  Id_Sub_Origen  INT NOT NULL,
  Id_Sub_Destino INT NOT NULL,
  PRIMARY KEY (Id),
  UNIQUE KEY uq_vidrio_regla (Id_Sistema, Id_Tipo, Id_Sub_Origen)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Tipos de partida. Anadir aqui cualquier combinacion nueva que haga falta.
INSERT INTO beta_vidrio_tipo (Nombre, Orden) VALUES
  ('3+3',         10),
  ('5+5',         20),
  ('6+6',         30),
  ('5+5/c/6+6',   40)
ON DUPLICATE KEY UPDATE Orden = VALUES(Orden);
