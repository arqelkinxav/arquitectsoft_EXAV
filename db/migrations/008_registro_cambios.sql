-- =====================================================================
-- 008_registro_cambios.sql
--
-- ULTIMO CAMBIO DE LA BASE: una fila por tabla con cuando se toco por
-- ultima vez, que se hizo (INSERT/UPDATE/DELETE) y quien. El programa
-- muestra la mas reciente en la barra de arriba (Generals/RegistroCambios.cs).
--
-- Quien: el programa pone @arq_usuario al abrir cada conexion
-- (Generals/Conexion.cs). Si el cambio viene de Workbench u otro cliente,
-- queda el usuario de MySQL (USER()).
--
-- Lo llenan triggers en las tablas que se guardan de verdad. Las de
-- calculo (proyecto*, tbauxanchura) NO: se vacian en cada analisis y
-- taparian el cambio real. dbmanagments SI: ahi apunta el Importar, asi
-- que un Importar sale como ultimo cambio.
--
-- Cada trigger lleva un CONTINUE HANDLER: si el registro falla (p.ej. la
-- tabla beta_ultimo_cambio no esta), el guardado sigue adelante. Apuntar
-- nunca puede impedir trabajar.
--
-- No registra cambios de estructura ni de procedimientos (eso no pasa por
-- triggers). Tabla nueva que haya que vigilar = anadirla aqui y volver a
-- pasar el archivo.
--
-- Sin DEFINER a proposito: toma el usuario que ejecuta el script en ESE
-- servidor, que seguro existe.
--
-- Idempotente (DROP TRIGGER IF EXISTS + CREATE).
-- =====================================================================

CREATE TABLE IF NOT EXISTS beta_ultimo_cambio (
  Tabla    VARCHAR(64)  NOT NULL,
  Accion   VARCHAR(10)  NOT NULL,
  Fecha    DATETIME     NOT NULL,
  Usuario  VARCHAR(100) NULL,
  PRIMARY KEY (Tabla),
  KEY ix_beta_ultimo_cambio_fecha (Fecha)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

DELIMITER $$

DROP TRIGGER IF EXISTS `trg_acabados_ai_cambio`$$
CREATE TRIGGER `trg_acabados_ai_cambio` AFTER INSERT ON `acabados` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('acabados', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_acabados_au_cambio`$$
CREATE TRIGGER `trg_acabados_au_cambio` AFTER UPDATE ON `acabados` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('acabados', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_acabados_ad_cambio`$$
CREATE TRIGGER `trg_acabados_ad_cambio` AFTER DELETE ON `acabados` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('acabados', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_dependencias_acabado_ai_cambio`$$
CREATE TRIGGER `trg_beta_dependencias_acabado_ai_cambio` AFTER INSERT ON `beta_dependencias_acabado` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_dependencias_acabado', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_dependencias_acabado_au_cambio`$$
CREATE TRIGGER `trg_beta_dependencias_acabado_au_cambio` AFTER UPDATE ON `beta_dependencias_acabado` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_dependencias_acabado', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_dependencias_acabado_ad_cambio`$$
CREATE TRIGGER `trg_beta_dependencias_acabado_ad_cambio` AFTER DELETE ON `beta_dependencias_acabado` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_dependencias_acabado', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_perfil_ai_cambio`$$
CREATE TRIGGER `trg_beta_perfil_ai_cambio` AFTER INSERT ON `beta_perfil` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_perfil', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_perfil_au_cambio`$$
CREATE TRIGGER `trg_beta_perfil_au_cambio` AFTER UPDATE ON `beta_perfil` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_perfil', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_perfil_ad_cambio`$$
CREATE TRIGGER `trg_beta_perfil_ad_cambio` AFTER DELETE ON `beta_perfil` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_perfil', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_rol_boton_ai_cambio`$$
CREATE TRIGGER `trg_beta_rol_boton_ai_cambio` AFTER INSERT ON `beta_rol_boton` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_rol_boton', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_rol_boton_au_cambio`$$
CREATE TRIGGER `trg_beta_rol_boton_au_cambio` AFTER UPDATE ON `beta_rol_boton` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_rol_boton', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_rol_boton_ad_cambio`$$
CREATE TRIGGER `trg_beta_rol_boton_ad_cambio` AFTER DELETE ON `beta_rol_boton` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_rol_boton', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_vidrio_regla_ai_cambio`$$
CREATE TRIGGER `trg_beta_vidrio_regla_ai_cambio` AFTER INSERT ON `beta_vidrio_regla` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_vidrio_regla', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_vidrio_regla_au_cambio`$$
CREATE TRIGGER `trg_beta_vidrio_regla_au_cambio` AFTER UPDATE ON `beta_vidrio_regla` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_vidrio_regla', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_vidrio_regla_ad_cambio`$$
CREATE TRIGGER `trg_beta_vidrio_regla_ad_cambio` AFTER DELETE ON `beta_vidrio_regla` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_vidrio_regla', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_vidrio_sistema_ai_cambio`$$
CREATE TRIGGER `trg_beta_vidrio_sistema_ai_cambio` AFTER INSERT ON `beta_vidrio_sistema` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_vidrio_sistema', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_vidrio_sistema_au_cambio`$$
CREATE TRIGGER `trg_beta_vidrio_sistema_au_cambio` AFTER UPDATE ON `beta_vidrio_sistema` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_vidrio_sistema', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_vidrio_sistema_ad_cambio`$$
CREATE TRIGGER `trg_beta_vidrio_sistema_ad_cambio` AFTER DELETE ON `beta_vidrio_sistema` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_vidrio_sistema', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_vidrio_tipo_ai_cambio`$$
CREATE TRIGGER `trg_beta_vidrio_tipo_ai_cambio` AFTER INSERT ON `beta_vidrio_tipo` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_vidrio_tipo', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_vidrio_tipo_au_cambio`$$
CREATE TRIGGER `trg_beta_vidrio_tipo_au_cambio` AFTER UPDATE ON `beta_vidrio_tipo` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_vidrio_tipo', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_beta_vidrio_tipo_ad_cambio`$$
CREATE TRIGGER `trg_beta_vidrio_tipo_ad_cambio` AFTER DELETE ON `beta_vidrio_tipo` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('beta_vidrio_tipo', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_categorias_ai_cambio`$$
CREATE TRIGGER `trg_categorias_ai_cambio` AFTER INSERT ON `categorias` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('categorias', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_categorias_au_cambio`$$
CREATE TRIGGER `trg_categorias_au_cambio` AFTER UPDATE ON `categorias` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('categorias', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_categorias_ad_cambio`$$
CREATE TRIGGER `trg_categorias_ad_cambio` AFTER DELETE ON `categorias` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('categorias', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_ai_cambio`$$
CREATE TRIGGER `trg_componentes_ai_cambio` AFTER INSERT ON `componentes` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_au_cambio`$$
CREATE TRIGGER `trg_componentes_au_cambio` AFTER UPDATE ON `componentes` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_ad_cambio`$$
CREATE TRIGGER `trg_componentes_ad_cambio` AFTER DELETE ON `componentes` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_detalle_ai_cambio`$$
CREATE TRIGGER `trg_componentes_detalle_ai_cambio` AFTER INSERT ON `componentes_detalle` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes_detalle', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_detalle_au_cambio`$$
CREATE TRIGGER `trg_componentes_detalle_au_cambio` AFTER UPDATE ON `componentes_detalle` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes_detalle', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_detalle_ad_cambio`$$
CREATE TRIGGER `trg_componentes_detalle_ad_cambio` AFTER DELETE ON `componentes_detalle` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes_detalle', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_especial_ai_cambio`$$
CREATE TRIGGER `trg_componentes_especial_ai_cambio` AFTER INSERT ON `componentes_especial` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes_especial', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_especial_au_cambio`$$
CREATE TRIGGER `trg_componentes_especial_au_cambio` AFTER UPDATE ON `componentes_especial` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes_especial', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_especial_ad_cambio`$$
CREATE TRIGGER `trg_componentes_especial_ad_cambio` AFTER DELETE ON `componentes_especial` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes_especial', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_especial_detalle_ai_cambio`$$
CREATE TRIGGER `trg_componentes_especial_detalle_ai_cambio` AFTER INSERT ON `componentes_especial_detalle` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes_especial_detalle', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_especial_detalle_au_cambio`$$
CREATE TRIGGER `trg_componentes_especial_detalle_au_cambio` AFTER UPDATE ON `componentes_especial_detalle` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes_especial_detalle', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_componentes_especial_detalle_ad_cambio`$$
CREATE TRIGGER `trg_componentes_especial_detalle_ad_cambio` AFTER DELETE ON `componentes_especial_detalle` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('componentes_especial_detalle', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_cortes_ai_cambio`$$
CREATE TRIGGER `trg_cortes_ai_cambio` AFTER INSERT ON `cortes` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('cortes', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_cortes_au_cambio`$$
CREATE TRIGGER `trg_cortes_au_cambio` AFTER UPDATE ON `cortes` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('cortes', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_cortes_ad_cambio`$$
CREATE TRIGGER `trg_cortes_ad_cambio` AFTER DELETE ON `cortes` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('cortes', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_dbmanagments_ai_cambio`$$
CREATE TRIGGER `trg_dbmanagments_ai_cambio` AFTER INSERT ON `dbmanagments` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('dbmanagments', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_dbmanagments_au_cambio`$$
CREATE TRIGGER `trg_dbmanagments_au_cambio` AFTER UPDATE ON `dbmanagments` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('dbmanagments', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_dbmanagments_ad_cambio`$$
CREATE TRIGGER `trg_dbmanagments_ad_cambio` AFTER DELETE ON `dbmanagments` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('dbmanagments', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_mecanizados_ai_cambio`$$
CREATE TRIGGER `trg_mecanizados_ai_cambio` AFTER INSERT ON `mecanizados` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('mecanizados', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_mecanizados_au_cambio`$$
CREATE TRIGGER `trg_mecanizados_au_cambio` AFTER UPDATE ON `mecanizados` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('mecanizados', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_mecanizados_ad_cambio`$$
CREATE TRIGGER `trg_mecanizados_ad_cambio` AFTER DELETE ON `mecanizados` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('mecanizados', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_subcomponentes_ai_cambio`$$
CREATE TRIGGER `trg_subcomponentes_ai_cambio` AFTER INSERT ON `subcomponentes` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('subcomponentes', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_subcomponentes_au_cambio`$$
CREATE TRIGGER `trg_subcomponentes_au_cambio` AFTER UPDATE ON `subcomponentes` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('subcomponentes', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_subcomponentes_ad_cambio`$$
CREATE TRIGGER `trg_subcomponentes_ad_cambio` AFTER DELETE ON `subcomponentes` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('subcomponentes', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_unidades_calculadas_ai_cambio`$$
CREATE TRIGGER `trg_unidades_calculadas_ai_cambio` AFTER INSERT ON `unidades_calculadas` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('unidades_calculadas', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_unidades_calculadas_au_cambio`$$
CREATE TRIGGER `trg_unidades_calculadas_au_cambio` AFTER UPDATE ON `unidades_calculadas` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('unidades_calculadas', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_unidades_calculadas_ad_cambio`$$
CREATE TRIGGER `trg_unidades_calculadas_ad_cambio` AFTER DELETE ON `unidades_calculadas` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('unidades_calculadas', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_unidades_medidas_ai_cambio`$$
CREATE TRIGGER `trg_unidades_medidas_ai_cambio` AFTER INSERT ON `unidades_medidas` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('unidades_medidas', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_unidades_medidas_au_cambio`$$
CREATE TRIGGER `trg_unidades_medidas_au_cambio` AFTER UPDATE ON `unidades_medidas` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('unidades_medidas', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_unidades_medidas_ad_cambio`$$
CREATE TRIGGER `trg_unidades_medidas_ad_cambio` AFTER DELETE ON `unidades_medidas` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('unidades_medidas', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_usuario_ai_cambio`$$
CREATE TRIGGER `trg_usuario_ai_cambio` AFTER INSERT ON `usuario` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('usuario', 'INSERT', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_usuario_au_cambio`$$
CREATE TRIGGER `trg_usuario_au_cambio` AFTER UPDATE ON `usuario` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('usuario', 'UPDATE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DROP TRIGGER IF EXISTS `trg_usuario_ad_cambio`$$
CREATE TRIGGER `trg_usuario_ad_cambio` AFTER DELETE ON `usuario` FOR EACH ROW
BEGIN
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION BEGIN END;
  INSERT INTO beta_ultimo_cambio (Tabla, Accion, Fecha, Usuario)
  VALUES ('usuario', 'DELETE', NOW(), COALESCE(@arq_usuario, USER()))
  ON DUPLICATE KEY UPDATE Accion = VALUES(Accion), Fecha = VALUES(Fecha), Usuario = VALUES(Usuario);
END$$

DELIMITER ;
