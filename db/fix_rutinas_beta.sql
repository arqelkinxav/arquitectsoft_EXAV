-- =============================================================
-- FIX rutinas de arquitectdb_beta   (fecha: 2026-07-15, v2)
-- Recrea las rutinas quitando el prefijo de base incrustado
-- (tanto 'arquitectdb.' como `arquitectdb`.) para que operen
-- sobre la base en la que viven (arquitectdb_beta).
-- Solo recrea rutinas: NO toca datos. NO toca arquitectdb.
-- Es idempotente: se puede volver a correr sin problema.
-- =============================================================
USE `arquitectdb_beta`;

-- Dumping routines for database 'arquitectdb'
--
/*!50003 DROP FUNCTION IF EXISTS `fnUnidadCalculada` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `fnUnidadCalculada`(

idUnidadCalculada int

) RETURNS varchar(100) CHARSET utf8mb3
BEGIN

DECLARE unidad nvarchar(100);

SET unidad = "";

IF idUnidadCalculada = 1 THEN

SET unidad = "Longitud Recompilacón";

ELSEIF idUnidadCalculada = 2 THEN

SET unidad = "Unidad";

ELSEIF idUnidadCalculada = 3 THEN

SET unidad = "Cantidad";

ELSEIF idUnidadCalculada = 4 THEN

SET unidad = "Medida Exacta";

ELSEIF idUnidadCalculada = 5 THEN

SET unidad = "Longitud sin Recompilacón";

ELSEIF idUnidadCalculada = 6 THEN

SET unidad = "Altura-Anchura";

ELSEIF idUnidadCalculada = 7 THEN

SET unidad = "Metro Lineal";

END IF;

RETURN unidad;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP FUNCTION IF EXISTS `fnUnidadCalculadaVidrioPanel` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `fnUnidadCalculadaVidrioPanel`(

idUnidadCalculada int

) RETURNS varchar(100) CHARSET utf8mb3
BEGIN

DECLARE unidad nvarchar(100);

SET unidad = "";

IF idUnidadCalculada = 1 THEN

SET unidad = "1|Columna primera";

ELSEIF idUnidadCalculada = 2 THEN

SET unidad = "2|Columna segunda";

ELSEIF idUnidadCalculada = 3 THEN

SET unidad = "3|Columna tercera";

ELSEIF idUnidadCalculada = 4 THEN

SET unidad = "4|Columna cuarta";

ELSEIF idUnidadCalculada = 5 THEN

SET unidad = "5|Columna quinta";

ELSEIF idUnidadCalculada = 6 THEN

SET unidad = "6|Longitud";

ELSEIF idUnidadCalculada = 7 THEN

SET unidad = "7|Unidad";

END IF;

RETURN unidad;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componenteDetalleCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componenteDetalleCargar`(

idComponente int

)
BEGIN

SELECT detalle.Id_Subcomponente,

CONCAT(subcomponente.Codigo_Homologacion , "-" , acabados.Codigo_Homologacion) codigo,

subcomponente.Descripcion Descripcion_Ori,

CONCAT(subcomponente.Descripcion , " (" , acabados.Descripcion,") ") Descripcion,

CONCAT(detalle.Id_Unidad_Calculada, "|", fnUnidadCalculada(detalle.Id_Unidad_Calculada)) unidad,

detalle.Id_Unidad_Calculada,

Cantidad_Default ,

Cantidad_Adicional ,

Aplica_Decremento,

detalle.elevado,

CASE WHEN corte.Id_Corte IS NULL THEN 0 ELSE corte.Id_Corte END AS corte,

detalle.extra,

detalle.medida,
detalle.Mecanizado AS Cod_Mecanizado,
CASE WHEN meca.Descripcion IS NULL THEN '' ELSE meca.Descripcion END AS Mecanizado,

detalle.Asignacion_puertas

FROM componentes_detalle detalle

JOIN subcomponentes subcomponente ON detalle.Id_Subcomponente = subcomponente.Id_Subcomponente

JOIN acabados ON subcomponente.Id_Acabado = acabados.Id_Acabado

LEFT JOIN cortes corte ON corte.Id_Corte = detalle.idCorte
LEFT JOIN mecanizados meca on meca.Id_Mecanizado = detalle.Mecanizado

WHERE Id_Componente = idComponente
ORDER BY subcomponente.Descripcion ASC;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componenteEspecialDetalleCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componenteEspecialDetalleCargar`(

idComponente int

)
BEGIN

SELECT detalle.Id_Subcomponente,

CONCAT(subcomponente.Codigo_Homologacion , "-" , acabados.Codigo_Homologacion) codigo,

CONCAT(subcomponente.Descripcion , " (" , acabados.Descripcion,") ") Descripcion,

fnUnidadCalculadaVidrioPanel(detalle.select_Columna) select_Columna,

detalle.select_Columna Id_Columna,

Cantidad_Default ,

Cantidad_Adicional ,

Aplica_Decremento,

detalle.elevado,

CASE WHEN corte.Descripcion IS NULL THEN '-- Seleccionar --' ELSE corte.Descripcion END AS corte

FROM componentes_Especial_detalle detalle

JOIN subcomponentes subcomponente ON detalle.Id_Subcomponente = subcomponente.Id_Subcomponente

JOIN acabados ON subcomponente.Id_Acabado = acabados.Id_Acabado

LEFT JOIN cortes corte ON corte.Id_Corte = detalle.idCorte

WHERE Id_Componente_Especial = idComponente;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componentePuertaCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componentePuertaCargar`(

pCodigo    nvarchar(50)

)
BEGIN

SELECT concat(comp.codigo,'-',acab.Codigo_homologacion) Codigo, concat(comp.descripcion,' (',acab.Descripcion,')') Descripcion

FROM componentes Comp

join acabados acab on comp.AcabadoPrincipal = acab.Id_Acabado

where concat(comp.codigo,'-',acab.Codigo_homologacion) = pCodigo;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componentePuertaDetalleCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componentePuertaDetalleCargar`(

pCodigo    nvarchar(50),

plogitud      float,

pAnchura      float,

pPuerta    nvarchar(1000),

pSwHerraje int,

pSwAP int

)
BEGIN

DECLARE idUnidadCalculada INT;

DECLARE aplicaDecremento BIT;

DECLARE Aplica_Decremento_Anch BIT;

DECLARE idv Int;

DECLARE idSubComponentev Int;

DECLARE count INT;

DECLARE contador INT default 0;

DECLARE cantidad_U Int;

DECLARE pmedida float;

DECLARE selecMedida Int;

DECLARE plogitudAnchura float;

DECLARE plogitudAdi float;

DECLARE pAnchuraAdi float;

SET SQL_SAFE_UPDATES = 0;

CREATE TEMPORARY TABLE tableResult (id_subcomponente Int,

id_unidad_Calculada Int,

codigo nvarchar(50),

descripcion nvarchar(200),

cantidad Int,

medida nvarchar(10),

cantidadAdicional Int,

Extra int,

Corte nvarchar(50) null,

Mecanizado nvarchar(50) null,

subcomponenteidv Int);

CREATE TEMPORARY TABLE tbSubcomponente (id Int, idSubComponente Int);

INSERT tbSubcomponente (id, idSubComponente)

SELECT componentes_detalle.id, Id_subcomponente FROM  componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo and componentes_detalle.Asignacion_puertas = pSwAP;

SET count = (SELECT count(*) FROM tbSubcomponente);

WHILE contador < count DO

SET idv = (SELECT id FROM tbSubcomponente order by id LIMIT contador,1);

SET idSubComponentev = (SELECT idSubComponente FROM tbSubcomponente order by id LIMIT contador,1);

SET idUnidadCalculada =  (SELECT  id_Unidad_calculada FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

SET selecMedida = (SELECT medida FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

SET plogitudAnchura = plogitud;

IF  (selecMedida = 2) THEN

SET plogitudAnchura = pAnchura;

END IF;

IF (idUnidadCalculada = 1) THEN 

SET aplicaDecremento = (SELECT aplica_decremento FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

IF  (aplicaDecremento = 0) THEN

SET pmedida = plogitudAnchura + (SELECT cantidad_adicional FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

ELSE

SET pmedida = plogitudAnchura - (SELECT cantidad_adicional FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

END IF;

INSERT tableResult

SELECT subcomponentes.id_subcomponente,

idUnidadCalculada,

CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,

subcomponentes.descripcion,

Cantidad_default AS cantidad,

0 ,

CEILING(pmedida * Cantidad_default)  AS medida,

componentes_detalle.extra,

c.descripcion,

m.descripcion Mecanizado,

idv

FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente

JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente

JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte

LEFT JOIN mecanizados m on m.Id_mecanizado = componentes_detalle.mecanizado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

ELSEIF (idUnidadCalculada = 2) THEN  

INSERT tableResult

SELECT  subcomponentes.id_subcomponente,

idUnidadCalculada,

CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,

subcomponentes.descripcion,

CEILING((plogitudAnchura * cantidad_default)/1000) AS cantidad,

"0" AS medida,

0,

componentes_detalle.extra,

c.descripcion,

m.descripcion Mecanizado,

idv

FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente

JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente

JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte

LEFT JOIN mecanizados m on m.Id_mecanizado = componentes_detalle.mecanizado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

ELSEIF (idUnidadCalculada = 3) THEN  

INSERT tableResult

SELECT  subcomponentes.id_subcomponente,

idUnidadCalculada,

CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,

subcomponentes.descripcion,

cantidad_default AS cantidad,

"0" AS medida,

0,

componentes_detalle.extra,

c.descripcion,

m.descripcion Mecanizado,

idv

FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente

JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente

JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte

LEFT JOIN mecanizados m on m.Id_mecanizado = componentes_detalle.mecanizado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

ELSEIF (idUnidadCalculada = 4) THEN 

CREATE TEMPORARY TABLE tbCantidad (idSubcomponente Int, cantidad int);

INSERT tbCantidad (idSubcomponente, cantidad)

SELECT  Id_Subcomponente, cantidad_default

FROM  componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

WHERE CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

SET cantidad_U = (SELECT sum(cantidad) FROM tbCantidad WHERE tbCantidad.idSubcomponente = idSubComponente);

INSERT tableResult

SELECT DISTINCT  subcomponentes.id_subcomponente,

idUnidadCalculada,

CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,

subcomponentes.descripcion,

cantidad_U AS cantidad,

"0" AS medida,

cantidad_adicional,

componentes_detalle.extra,

c.descripcion,

m.descripcion Mecanizado,

idv

FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente

JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente

JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte

LEFT JOIN mecanizados m on m.Id_mecanizado = componentes_detalle.mecanizado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;



DROP TABLE tbCantidad;

ELSEIF (idUnidadCalculada = 5) THEN  

SET aplicaDecremento = (SELECT aplica_decremento FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

SET pmedida = 0;

IF  (aplicaDecremento = 0) THEN

SET pmedida = plogitudAnchura + (SELECT cantidad_adicional FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

WHERE componentes_detalle.id = idv

AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

ELSE

SET pmedida = plogitudAnchura - (SELECT cantidad_adicional FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

WHERE componentes_detalle.id = idv

AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

END IF;

CREATE TEMPORARY TABLE tbLongitudSinRecopilar (idSubcomponente Int, cantidad int);

INSERT tbLongitudSinRecopilar (idSubcomponente, cantidad)

SELECT  Id_Subcomponente, cantidad_default

FROM  componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

WHERE componentes_detalle.id = idv

AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

SET cantidad_U = (SELECT sum(cantidad) FROM tbLongitudSinRecopilar WHERE tbLongitudSinRecopilar.idSubcomponente = idSubComponente);

INSERT tableResult

SELECT DISTINCT  subcomponentes.id_subcomponente,

idUnidadCalculada,

CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,

subcomponentes.descripcion,

cantidad_U,

case when selecMedida = 2 then pmedida else 0 end ,

case when selecMedida = 2 then 0 else pmedida end AS medida,

componentes_detalle.extra,

c.descripcion,

m.descripcion Mecanizado,

idv

FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente

JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente

JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte

LEFT JOIN mecanizados m on m.Id_mecanizado = componentes_detalle.mecanizado

WHERE componentes_detalle.id = idv

AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;



DROP TABLE tbLongitudSinRecopilar;

ELSEIF (idUnidadCalculada = 6) THEN  

SET aplicaDecremento = (SELECT aplica_decremento FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

SET Aplica_Decremento_Anch = (SELECT Aplica_Decremento_Anch FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

IF  (aplicaDecremento = 0) THEN

SET plogitudAdi = plogitud + (SELECT cantidad_adicional FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

ELSE

SET plogitudAdi = plogitud - (SELECT cantidad_adicional FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

END IF;

IF  (Aplica_Decremento_Anch = 0) THEN

SET pAnchuraAdi = pAnchura + (SELECT Cantidad_Adicional_Anch FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

ELSE

SET pAnchuraAdi = pAnchura - (SELECT Cantidad_Adicional_Anch FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

END IF;

INSERT tableResult

SELECT subcomponentes.id_subcomponente,

idUnidadCalculada,

CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,

subcomponentes.descripcion,

Cantidad_default AS cantidad,

pAnchuraAdi AS medida,

plogitudAdi,

componentes_detalle.extra,

c.descripcion,

m.descripcion Mecanizado,

idv

FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente

JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente

JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte

LEFT JOIN mecanizados m on m.Id_mecanizado = componentes_detalle.mecanizado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

ELSEIF (idUnidadCalculada = 7) THEN  

SET aplicaDecremento = (SELECT aplica_decremento FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

IF  (aplicaDecremento = 0) THEN

SET pmedida = plogitudAnchura + (SELECT cantidad_adicional FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

ELSE

SET pmedida = plogitudAnchura - (SELECT cantidad_adicional FROM componentes_detalle

JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente

LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

END IF;

INSERT tableResult

SELECT subcomponentes.id_subcomponente,

idUnidadCalculada,

CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,

subcomponentes.descripcion,

Cantidad_default AS cantidad,

0 ,

CEILING(pmedida * Cantidad_default)  AS medida,

componentes_detalle.extra,

c.descripcion,

m.descripcion Mecanizado,

idv

FROM componentes_detalle JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente

JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente

JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado

LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado

LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte

LEFT JOIN mecanizados m on m.Id_mecanizado = componentes_detalle.mecanizado

WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

END IF;

SET contador = contador + 1;

END WHILE;

IF (pSwHerraje = 0) THEN

insert into proyecto_Pt

SELECT subcomponenteidv,

id_unidad_Calculada,

sum(cantidad)  cantidad,

cantidadAdicional AS medidaBase,

medida,

extra,

corte,

mecanizado,

pPuerta,

pCodigo

FROM tableResult

where id_unidad_Calculada not in (2,3,7)

group by subcomponenteidv,id_unidad_Calculada,codigo , descripcion,

cantidadAdicional, medida,extra,corte,mecanizado;

ELSE

insert into proyecto_Pt

SELECT subcomponenteidv,

id_unidad_Calculada,

sum(cantidad)  cantidad,

cantidadAdicional AS medidaBase,

medida,

extra,

corte,

mecanizado,

pPuerta,

pCodigo

FROM tableResult

where id_unidad_Calculada  in (2,3,7)

group by subcomponenteidv,id_unidad_Calculada,codigo , descripcion,

cantidadAdicional, medida,extra,corte,mecanizado;

END IF;

DROP TABLE tableResult;

DROP TABLE tbSubcomponente;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componenteRelationSub` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componenteRelationSub`(

idComponente    nvarchar(50)

)
BEGIN

select distinct c.Codigo,c.descripcion,if(sc.especial=1,'SI','NO') as 'Vidrio/Panel' from subcomponentes sc

inner join componentes_detalle cpd on sc.Id_Subcomponente = cpd.Id_Subcomponente

inner join componentes c on c.Id_Componente = cpd.Id_Componente

where sc.Codigo_Homologacion = idComponente

union all

select c.Codigo,c.descripcion,if(sc.especial=1,'SI','NO') as 'Vidrio/Panel' from subcomponentes sc

inner join componentes_especial_detalle cpd on sc.Id_Subcomponente = cpd.Id_Subcomponente

inner join componentes c on c.Id_Componente = cpd.Id_Componente_especial

where sc.Codigo_Homologacion = idComponente;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componentesConsultar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componentesConsultar`(

pCadena nvarchar(100)

)
BEGIN

SELECT Id_Componente,Codigo,Descripcion

FROM componentes

WHERE CONCAT(Codigo , "-" , Descripcion) lIKE concat('%',  pCadena , '%');

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `componentesEspecialConsultar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `componentesEspecialConsultar`(

pCadena nvarchar(100)

)
BEGIN

SELECT Id_Componente_Especial,Codigo,Descripcion

FROM componentes_Especial

WHERE CONCAT(Codigo , "-" , Descripcion) lIKE concat('%',  pCadena , '%');

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteActualizar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteActualizar`(

pCodigo nvarchar(50),

pDescripcion nvarchar(300),

pNoSubcomponente boolean,

pIdComponente int

)
BEGIN

UPDATE componentes SET codigo = pCodigo,

descripcion = pDescripcion,

NoSubcomponente = pNoSubcomponente

WHERE Id_Componente = pIdComponente;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteCargar`(

pidComponente Int,

plogitud      Int,

panchura      Int,

paltura       Int ,

parea         Int

)
BEGIN

DECLARE idUnidadCalculada INT;

DECLARE cantidad DECIMAL;

DECLARE aplicaDecremento BIT;

SET @idUnidadCalculada = (SELECT id_Unidad_calculada FROM subcomponentes where id_subcomponente = pidComponente);

IF (@idUnidadCalculada = 1) THEN



SET @aplicaDecremento = (SELECT aplica_decremento FROM subcomponentes where id_subcomponente = pidComponente);

IF  (@aplicaDecremento = 0) THEN

SET @medida = (SELECT cantidad_adicional FROM subcomponentes where id_subcomponente = pidComponente) + plogitud;

ELSE

SET @medida = (SELECT cantidad_adicional FROM subcomponentes where id_subcomponente = pidComponente) - plogitud;

END IF;

SELECT id_subcomponente,

codigo_homologacion codigo,

descripcion,

Cantidad_defaultd cantidad,

@medida medida

FROM  subcomponentes where id_subcomponente = pidComponente;

ELSEIF (@idUnidadCalculada = 2) THEN



SET @cantidad = plogitud/1000;

SELECT  id_subcomponente,

codigo_homologacion codigo,

descripcion,

CEILING(@cantidad) cantidad,

plogitud

FROM subcomponentes where id_subcomponente = pidComponente;

ELSE

SELECT  id_subcomponente,

codigo_homologacion codigo,

descripcion,

Cantidad_defaultd,

plogitud

FROM subcomponentes where id_subcomponente = pidComponente;

END IF;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteEspecialActualizar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteEspecialActualizar`(

pCodigo nvarchar(50),

pDescripcion nvarchar(300),

pIdComponente int

)
BEGIN

UPDATE componentes_Especial SET codigo = pCodigo,

descripcion = pDescripcion

WHERE Id_Componente_Especial = pIdComponente;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteGetCodigoAcabado` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteGetCodigoAcabado`(

pCodigo       nvarchar(50)

)
BEGIN

SELECT concat(codigo,'-',ifnull(acabados.codigo_homologacion,"00")) Codigo

FROM componentes

join acabados on componentes.AcabadoPrincipal = acabados.Id_Acabado

where codigo = pCodigo;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponentePerfilesCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponentePerfilesCargar`(

	pCodigo       nvarchar(50),
	plogitud      float,
	pSwHerraje int,
	pMedidaBase int,
	pAnchura      float
)
BEGIN

DECLARE idUnidadCalculada INT;
DECLARE aplicaDecremento BIT;
DECLARE idv Int;
DECLARE idSubComponentev Int;
DECLARE count INT;
DECLARE contador INT default 0;
DECLARE cantidad_U Int;
DECLARE Sel_Cantidad_adicional INT;
DECLARE pmedida float;
DECLARE Sel_medida INT;
DECLARE Value_Medida float;
SET SQL_SAFE_UPDATES = 0;

CREATE TEMPORARY TABLE tableResult (id_subcomponente Int,id_unidad_Calculada Int,codigo nvarchar(50),descripcion nvarchar(200),
									cantidad Int,medida nvarchar(10),cantidadAdicional Int,Corte nvarchar(50), Mecanizado nvarchar(100) null);

CREATE TEMPORARY TABLE tbSubcomponente (id Int, idSubComponente Int);

INSERT tbSubcomponente (id, idSubComponente)
SELECT componentes_detalle.id, Id_subcomponente 
FROM  componentes_detalle
JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado
WHERE CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo and componentes_detalle.Asignacion_puertas = 0;

SET count = (SELECT count(*) FROM tbSubcomponente);

WHILE contador < count DO

	SET idv = (SELECT id FROM tbSubcomponente order by id LIMIT contador,1);
	SET idSubComponentev = (SELECT idSubComponente FROM tbSubcomponente order by id LIMIT contador,1);
	SET idUnidadCalculada =  (SELECT  id_Unidad_calculada FROM componentes_detalle
								JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
                                LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado
                                WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

SET Sel_Cantidad_adicional = (SELECT cantidad_adicional FROM componentes_detalle
								JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
                                LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado
                                WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

SET Sel_medida = (SELECT medida FROM componentes_detalle
					JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
                    LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado
                    WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

SET aplicaDecremento = (SELECT aplica_decremento FROM componentes_detalle
						JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
                        LEFT JOIN acabados ON AcabadoPrincipal = acabados.Id_Acabado
                        WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),"")) = pCodigo LIMIT 0,1);

SET Value_Medida = plogitud;

IF  (Sel_medida = 2) THEN
	SET Value_Medida = pAnchura;
END IF;

IF  (aplicaDecremento = 0) THEN
	SET pmedida = Value_Medida + Sel_Cantidad_adicional;
ELSE
	SET pmedida = Value_Medida - Sel_Cantidad_adicional;
END IF;

IF (idUnidadCalculada = 1) THEN 

	INSERT tableResult
    SELECT subcomponentes.id_subcomponente,idUnidadCalculada,CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,subcomponentes.descripcion,
			Cantidad_default AS cantidad,CEILING(pmedida * Cantidad_default) AS medida,pMedidaBase,c.descripcion,m.descripcion
	FROM componentes_detalle 
    JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
    JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
    JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
    LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte
    LEFT JOIN mecanizados m ON m.Id_Mecanizado = componentes_detalle.Mecanizado
    LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado
    WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

ELSEIF (idUnidadCalculada = 2) THEN  

	INSERT tableResult
    SELECT  subcomponentes.id_subcomponente,idUnidadCalculada,CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,subcomponentes.descripcion,
			(pmedida * cantidad_default)/1000 AS cantidad,0 AS medida,0,c.descripcion,m.descripcion
	FROM componentes_detalle 
    JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
    JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
    JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
    LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte
    LEFT JOIN mecanizados m ON m.Id_Mecanizado = componentes_detalle.Mecanizado
    LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado
    WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

ELSEIF (idUnidadCalculada = 3) THEN  

	INSERT tableResult
    SELECT  subcomponentes.id_subcomponente,idUnidadCalculada,CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,subcomponentes.descripcion,
			cantidad_default AS cantidad,0 AS medida,0,c.descripcion,m.descripcion
	FROM componentes_detalle 
    JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
    JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
    JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
    LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte
    LEFT JOIN mecanizados m ON m.Id_Mecanizado = componentes_detalle.Mecanizado
    LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado
    WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

ELSEIF (idUnidadCalculada = 4) THEN 

	CREATE TEMPORARY TABLE tbCantidad (idSubcomponente Int, cantidad int);

	INSERT tbCantidad (idSubcomponente, cantidad)
    SELECT  Id_Subcomponente, cantidad_default
    FROM  componentes_detalle
    JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
    LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado
    WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

	SET cantidad_U = (SELECT sum(cantidad) FROM tbCantidad WHERE tbCantidad.idSubcomponente = idSubComponente);

	INSERT tableResult
    SELECT DISTINCT  subcomponentes.id_subcomponente,idUnidadCalculada,CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,subcomponentes.descripcion,
		cantidad_U AS cantidad,0 AS medida,cantidad_adicional,c.descripcion,m.descripcion
	FROM componentes_detalle 
    JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
    JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
    JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
    LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte
    LEFT JOIN mecanizados m ON m.Id_Mecanizado = componentes_detalle.Mecanizado
    LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado
    WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

	

	DROP TABLE tbCantidad;

ELSEIF (idUnidadCalculada = 5) THEN  

	CREATE TEMPORARY TABLE tbLongitudSinRecopilar (idSubcomponente Int, cantidad int);

	INSERT tbLongitudSinRecopilar (idSubcomponente, cantidad)
    SELECT  Id_Subcomponente, cantidad_default
    FROM  componentes_detalle
    JOIN componentes ON componentes_detalle.Id_componente = componentes.Id_componente
    LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado
    WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

	SET cantidad_U = (SELECT sum(cantidad) FROM tbLongitudSinRecopilar WHERE tbLongitudSinRecopilar.idSubcomponente = idSubComponente);

	INSERT tableResult
    SELECT DISTINCT  subcomponentes.id_subcomponente,idUnidadCalculada,CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,subcomponentes.descripcion,
		cantidad_U,0,pmedida,c.descripcion,m.descripcion
	FROM componentes_detalle 
    JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
    JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
    JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
    LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte
    LEFT JOIN mecanizados m ON m.Id_Mecanizado = componentes_detalle.Mecanizado
    LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado
    WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

	

	DROP TABLE tbLongitudSinRecopilar;

ELSEIF (idUnidadCalculada = 7) THEN  

	INSERT tableResult
	SELECT subcomponentes.id_subcomponente,idUnidadCalculada,CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,subcomponentes.descripcion,
			Cantidad_default AS cantidad,CEILING(pmedida * Cantidad_default) AS medida,0  ,c.descripcion,m.descripcion
	FROM componentes_detalle 
    JOIN subcomponentes ON componentes_detalle.id_subcomponente = subcomponentes.id_subcomponente
    JOIN componentes ON componentes.Id_Componente = componentes_detalle.Id_Componente
    JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado
    LEFT JOIN cortes c on c.Id_Corte = componentes_detalle.idcorte
    LEFT JOIN mecanizados m ON m.Id_Mecanizado = componentes_detalle.Mecanizado
    LEFT JOIN acabados AcabPrincipal ON componentes.AcabadoPrincipal = AcabPrincipal.Id_Acabado
    WHERE componentes_detalle.id = idv AND CONCAT(componentes.Codigo , IFNULL(concat('-',AcabPrincipal.Codigo_Homologacion),"")) = pCodigo;

END IF;

SET contador = contador + 1;

END WHILE;

IF (pSwHerraje = 0) THEN

	SELECT id_subcomponente,id_unidad_Calculada,codigo ,descripcion ,cantidad ,cantidadAdicional AS medidaBase,medida,Corte,Mecanizado
    FROM tableResult where id_unidad_Calculada not in (2,3,7);
    
ELSE

	SELECT id_subcomponente,id_unidad_Calculada,codigo ,descripcion ,cantidad ,cantidadAdicional AS medidaBase,medida,Corte,Mecanizado
    FROM tableResult where id_unidad_Calculada  in (2,3,7);
    
END IF;

DROP TABLE tableResult;
DROP TABLE tbSubcomponente;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteVidrioPanel` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteVidrioPanel`(

pCodigo    nvarchar(50),

pAltura    float

)
BEGIN

DECLARE idv Int;

DECLARE count INT;

DECLARE countColumns INT;

DECLARE selectColumna INT;

DECLARE contador INT default 0;

DECLARE contadorColumns INT default 0;

DECLARE cantidad Int;

DECLARE idUnidadCalculada INT;

DECLARE aplicaDecremento BIT;

DECLARE pmedida float;

DECLARE col,alt INT;

CREATE TEMPORARY TABLE tableResult (id_subcomponente Int,

id_unidad_Calculada Int,

codigo nvarchar(10),

descripcion nvarchar(100),

altura nvarchar(100),

anchura nvarchar(100),

cantidad Int,

medida nvarchar(10),

cantidadAdicional Int);

CREATE TEMPORARY TABLE tbSubcomponente (id Int);

INSERT tbSubcomponente (id)

SELECT componentes_especial_detalle.id FROM  componentes_especial_detalle

JOIN componentes ON componentes_especial_detalle.Id_Componente_especial = componentes.Id_Componente

WHERE componentes.Codigo = pCodigo;

SET count = (SELECT count(*) FROM tbSubcomponente);

WHILE contador < count DO

SET idv = (SELECT * FROM tbSubcomponente order by id LIMIT contador,1);

SET selectColumna =  (SELECT select_Columna FROM componentes_especial_detalle

JOIN componentes ON componentes_especial_detalle.Id_Componente_especial = componentes.Id_Componente

WHERE id = idv AND codigo = pCodigo LIMIT 0,1);

SET countColumns = (SELECT count(*) FROM tbauxanchura);

IF selectColumna = 1  THEN

SET contadorColumns = 0;

WHILE contadorColumns < countColumns DO

SET col = (SELECT Columna1 FROM tbauxanchura  LIMIT contadorColumns,1);

SET alt = (SELECT Altura FROM tbauxanchura  LIMIT contadorColumns,1);

INSERT proyecto_vidriopanel(codigo, Altura,Anchura)VALUES(idv,alt,col);

SET contadorColumns = contadorColumns + 1;

END WHILE;

END IF;

IF selectColumna = 2  THEN

sET contadorColumns = 0;

WHILE contadorColumns < countColumns DO

SET col = (SELECT Columna2 FROM tbauxanchura LIMIT contadorColumns,1);

SET alt = (SELECT Altura FROM tbauxanchura LIMIT contadorColumns,1);

INSERT proyecto_vidriopanel(codigo, Altura,Anchura)VALUES(idv,alt,col);

SET contadorColumns = contadorColumns + 1;

END WHILE;

END IF;

IF selectColumna = 3  THEN

SET contadorColumns = 0;

WHILE contadorColumns < countColumns DO

SET col = (SELECT Columna3 FROM tbauxanchura LIMIT contadorColumns,1);

SET alt = (SELECT Altura FROM tbauxanchura  LIMIT contadorColumns,1);

INSERT proyecto_vidriopanel(codigo, Altura,Anchura)VALUES(idv,alt,col);

SET contadorColumns = contadorColumns + 1;

END WHILE;

END IF;

IF selectColumna = 4  THEN

SET contadorColumns = 0;

WHILE contadorColumns < countColumns DO

SET col = (SELECT Columna4 FROM tbauxanchura LIMIT contadorColumns,1);

SET alt = (SELECT Altura FROM tbauxanchura  LIMIT contadorColumns,1);

INSERT proyecto_vidriopanel(codigo, Altura,Anchura)VALUES(idv,alt,col);

SET contadorColumns = contadorColumns + 1;

END WHILE;

END IF;

IF selectColumna = 5  THEN

SET contadorColumns = 0;

WHILE contadorColumns < countColumns DO

SET col = (SELECT Columna5 FROM tbauxanchura  LIMIT contadorColumns,1);

SET alt = (SELECT Altura FROM tbauxanchura  LIMIT contadorColumns,1);

INSERT proyecto_vidriopanel(codigo, Altura,Anchura)VALUES(idv,alt,col);

SET contadorColumns = contadorColumns + 1;

END WHILE;

END IF;

IF selectColumna = 6  THEN

SET aplicaDecremento = (SELECT aplica_decremento FROM componentes_especial_detalle

JOIN componentes ON componentes_especial_detalle.Id_Componente_especial = componentes.Id_Componente

WHERE componentes_especial_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);

IF  (aplicaDecremento = 0) THEN

SET pmedida = plogitud + (SELECT cantidad_adicional FROM componentes_especial_detalle

JOIN componentes ON componentes_especial_detalle.Id_Componente_especial = componentes.Id_Componente

WHERE componentes_especial_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);

ELSE

SET pmedida = plogitud - (SELECT cantidad_adicional FROM componentes_especial_detalle

JOIN componentes ON componentes_especial_detalle.Id_Componente_especial = componentes.Id_Componente

WHERE componentes_especial_detalle.id = idv AND codigo = pCodigo LIMIT 0,1);

END IF;



END IF;

SET contador = contador + 1;

INSERT tableResult

SELECT Codigo id,

selectColumna,

CONCAT(subcomponentes.codigo_homologacion,"-",acabados.codigo_homologacion) codigo,

subcomponentes.descripcion,

Altura,

Anchura,

count(*) cantidad,

0,

cantidad_adicional

FROM proyecto_vidriopanel JOIN componentes_especial_detalle

ON proyecto_vidriopanel.Codigo = componentes_especial_detalle.id

JOIN subcomponentes ON subcomponentes.Id_Subcomponente = componentes_especial_detalle.Id_Subcomponente

JOIN acabados ON acabados.Id_Acabado = subcomponentes.Id_Acabado

group by proyecto_vidriopanel.Codigo,selectColumna,Altura,Anchura;

END WHILE;

SELECT * FROM tableResult

ORDER BY codigo DESC;

TRUNCATE proyecto_vidriopanel;

DROP TABLE tbSubcomponente;

DROP TABLE tableResult;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteVidrioPanelInsert` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteVidrioPanelInsert`(

pCodigo  nvarchar(50),

pAltura  int,

pAchura  Int

)
BEGIN

INSERT proyecto_vidriopanel (Codigo,

Altura,

Anchura)

VALUES(pCodigo,

pAltura,

pAchura);

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spComponenteVidrioPanelv2` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spComponenteVidrioPanelv2`(

pCodigo    nvarchar(50),

pAltura    float,

pUbicacion    nvarchar(50)

)
BEGIN

select subcomponentes.Id_Subcomponente,p.Altura,p.Anchura,sum(p.cantidad) Cantidad,p.ubicacion from (

SELECT dte.Id,tbauxanchura.altura, tbauxanchura.Columna1 Anchura,tbauxanchura.ubicacion,count(dte.Id) Cantidad FROM tbauxanchura

join componentes on componentes.codigo = tbauxanchura.codigo

join componentes_especial_detalle dte on dte.Id_Componente_especial = componentes.Id_Componente

where select_Columna = dte.select_Columna and tbauxanchura.codigo = pCodigo and tbauxanchura.altura = pAltura and select_Columna = 1

and tbauxanchura.ubicacion like '%M%' and tbauxanchura.ubicacion = pUbicacion

group by dte.Id,tbauxanchura.altura, tbauxanchura.Columna1

union all

SELECT dte.Id,tbauxanchura.altura, tbauxanchura.Columna2 Anchura,tbauxanchura.ubicacion,count(dte.Id) Cantidad FROM tbauxanchura

join componentes on componentes.codigo = tbauxanchura.codigo

join componentes_especial_detalle dte on dte.Id_Componente_especial = componentes.Id_Componente

where select_Columna = dte.select_Columna and tbauxanchura.codigo = pCodigo and tbauxanchura.altura = pAltura and select_Columna = 2

and tbauxanchura.ubicacion like '%M%' and tbauxanchura.ubicacion = pUbicacion

group by dte.Id,tbauxanchura.altura, tbauxanchura.Columna2

union all

SELECT dte.Id,tbauxanchura.altura, tbauxanchura.Columna3 Anchura,tbauxanchura.ubicacion,count(dte.Id) Cantidad FROM tbauxanchura

join componentes on componentes.codigo = tbauxanchura.codigo

join componentes_especial_detalle dte on dte.Id_Componente_especial = componentes.Id_Componente

where select_Columna = dte.select_Columna and tbauxanchura.codigo = pCodigo and tbauxanchura.altura = pAltura and select_Columna = 3

and tbauxanchura.ubicacion like '%M%' and tbauxanchura.ubicacion = pUbicacion

group by dte.Id,tbauxanchura.altura, tbauxanchura.Columna3

union all

SELECT dte.Id,tbauxanchura.altura, tbauxanchura.Columna4 Anchura,tbauxanchura.ubicacion,count(dte.Id) Cantidad FROM tbauxanchura

join componentes on componentes.codigo = tbauxanchura.codigo

join componentes_especial_detalle dte on dte.Id_Componente_especial = componentes.Id_Componente

where select_Columna = dte.select_Columna and tbauxanchura.codigo = pCodigo and tbauxanchura.altura = pAltura and select_Columna = 4

and tbauxanchura.ubicacion like '%M%' and tbauxanchura.ubicacion = pUbicacion

group by dte.Id,tbauxanchura.altura, tbauxanchura.Columna4

union all

SELECT dte.Id,tbauxanchura.altura, tbauxanchura.Columna5 Anchura,tbauxanchura.ubicacion,count(dte.Id) Cantidad FROM tbauxanchura

join componentes on componentes.codigo = tbauxanchura.codigo

join componentes_especial_detalle dte on dte.Id_Componente_especial = componentes.Id_Componente

where select_Columna = dte.select_Columna and tbauxanchura.codigo = pCodigo and tbauxanchura.altura = pAltura and select_Columna = 5

and tbauxanchura.ubicacion like '%M%' and tbauxanchura.ubicacion = pUbicacion

group by dte.Id,tbauxanchura.altura, tbauxanchura.Columna5) p

JOIN componentes_especial_detalle

ON p.Id = componentes_especial_detalle.id

JOIN subcomponentes ON subcomponentes.Id_Subcomponente = componentes_especial_detalle.Id_Subcomponente

group by subcomponentes.Id_Subcomponente,Altura,Anchura,p.ubicacion;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteAgrupar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteAgrupar`(

pDesperdicio float,

pSwHerraje int,
pSwUbicacion int

)
BEGIN

SET SQL_SAFE_UPDATES = 0;
IF (pSwUbicacion = 0) THEN
	IF (pSwHerraje = 0) THEN

		SELECT proy.Id_Subcomponente,concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,
		subcomponente.Descripcion,acabado.Descripcion AcabadoDesc,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and  proyect.medidaAdicional = proy.medidaAdicional)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
		fnUnidadCalculada(proy.Id_Unidad_Medida) unidaMedida,
		proy.corte, proy.Mecanizado
		FROM proyecto proy
		JOIN subcomponentes subcomponente ON proy.Id_Subcomponente = subcomponente.Id_Subcomponente
		JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado        
		GROUP BY proy.Id_Subcomponente, proy.Id_Unidad_Medida, proy.medidaAdicional,proy.corte;

	else

		SELECT proy.Id_Subcomponente,concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,
		subcomponente.Descripcion,acabado.Descripcion AcabadoDesc,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and  proyect.medidaAdicional = proy.medidaAdicional)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
		(select fnUnidadCalculada(min(proyuni.Id_Unidad_Medida)) unidad
		FROM proyecto proyuni
		JOIN subcomponentes subComUni ON proyuni.Id_Subcomponente = subComUni.Id_Subcomponente
		JOIN acabados acabadoUni ON acabadoUni.Id_Acabado = subComUni.Id_Acabado
		where concat(subComUni.codigo_homologacion, "-",acabadoUni.codigo_homologacion ) = concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion )
		group by  concat(subComUni.codigo_homologacion, "-",acabadoUni.codigo_homologacion ))  unidaMedida,
		proy.corte, proy.Mecanizado
		FROM proyecto proy
		JOIN subcomponentes subcomponente ON proy.Id_Subcomponente = subcomponente.Id_Subcomponente
		JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado
		GROUP BY proy.Id_Subcomponente, proy.medidaAdicional,proy.corte;

	END IF;
else
	IF (pSwHerraje = 0) THEN
		SELECT proy.Id_Subcomponente,
		concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,
		subcomponente.Descripcion,
		acabado.Descripcion AcabadoDesc,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and  proyect.medidaAdicional = proy.medidaAdicional and proyect.ubicacion = proy.ubicacion)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and proyect.ubicacion = proy.ubicacion) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and proyect.ubicacion = proy.ubicacion) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and proyect.ubicacion = proy.ubicacion) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
		fnUnidadCalculada(proy.Id_Unidad_Medida) unidaMedida,
		proy.corte, proy.Mecanizado, proy.ubicacion
		FROM proyecto proy       
		JOIN subcomponentes subcomponente ON proy.Id_Subcomponente = subcomponente.Id_Subcomponente
		JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado
		WHERE proy.Id_Unidad_Medida = 5
		GROUP BY proy.Id_Subcomponente, proy.Id_Unidad_Medida, proy.medidaAdicional,proy.corte, proy.ubicacion 
        UNION ALL
        SELECT proy.Id_Subcomponente,
		concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,
		subcomponente.Descripcion,
		acabado.Descripcion AcabadoDesc,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and  proyect.medidaAdicional = proy.medidaAdicional)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
		fnUnidadCalculada(proy.Id_Unidad_Medida) unidaMedida,
		proy.corte, proy.Mecanizado, null AS ubicacion
		FROM proyecto proy
		JOIN subcomponentes subcomponente ON proy.Id_Subcomponente = subcomponente.Id_Subcomponente
		JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado
        WHERE proy.Id_Unidad_Medida != 5
		GROUP BY proy.Id_Subcomponente, proy.Id_Unidad_Medida, proy.medidaAdicional,proy.corte;
	else
		SELECT proy.Id_Subcomponente,
		concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,
		subcomponente.Descripcion,
		acabado.Descripcion AcabadoDesc,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and  proyect.medidaAdicional = proy.medidaAdicional)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
		(select fnUnidadCalculada(min(proyuni.Id_Unidad_Medida)) unidad
		FROM proyecto proyuni
		JOIN subcomponentes subComUni ON proyuni.Id_Subcomponente = subComUni.Id_Subcomponente
		JOIN acabados acabadoUni ON acabadoUni.Id_Acabado = subComUni.Id_Acabado
		where concat(subComUni.codigo_homologacion, "-",acabadoUni.codigo_homologacion ) = concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion )
		group by  concat(subComUni.codigo_homologacion, "-",acabadoUni.codigo_homologacion ))  unidaMedida,
		proy.corte, proy.Mecanizado, proy.ubicacion
		FROM proyecto proy
		JOIN subcomponentes subcomponente ON proy.Id_Subcomponente = subcomponente.Id_Subcomponente
		JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado
        WHERE proy.Id_Unidad_Medida = 5
		GROUP BY proy.Id_Subcomponente, proy.medidaAdicional,proy.corte, proy.ubicacion 
        UNION ALL
		SELECT proy.Id_Subcomponente,
		concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,
		subcomponente.Descripcion,
		acabado.Descripcion AcabadoDesc,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and  proyect.medidaAdicional = proy.medidaAdicional)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
		(select fnUnidadCalculada(min(proyuni.Id_Unidad_Medida)) unidad
		FROM proyecto proyuni
		JOIN subcomponentes subComUni ON proyuni.Id_Subcomponente = subComUni.Id_Subcomponente
		JOIN acabados acabadoUni ON acabadoUni.Id_Acabado = subComUni.Id_Acabado
		where concat(subComUni.codigo_homologacion, "-",acabadoUni.codigo_homologacion ) = concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion )
		group by  concat(subComUni.codigo_homologacion, "-",acabadoUni.codigo_homologacion ))  unidaMedida,
		proy.corte, proy.Mecanizado, NULL ubicacion
		FROM proyecto proy
		JOIN subcomponentes subcomponente ON proy.Id_Subcomponente = subcomponente.Id_Subcomponente
		JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado
        WHERE proy.Id_Unidad_Medida != 5
		GROUP BY proy.Id_Subcomponente, proy.medidaAdicional,proy.corte;
	END IF;
END IF;

DELETE FROM proyecto;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubcomponenteCargar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubcomponenteCargar`(

idSubComponente int

)
BEGIN

SELECT subcomponente.Id_Acabado,

subcomponente.Codigo_Homologacion,

subcomponente.Descripcion,

acabado.Codigo_Homologacion codigo,

Especial

FROM subcomponentes subcomponente

JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado

WHERE Id_Subcomponente = idSubComponente;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteConsultar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteConsultar`(

pCadena nvarchar(300)

)
BEGIN

SELECT Id_subcomponente,

CONCAT(subcomponentes.Codigo_Homologacion , "-" , acabados.Codigo_Homologacion)  codigo,

subcomponentes.Descripcion

FROM subcomponentes JOIN acabados

ON subcomponentes.Id_Acabado = acabados.Id_Acabado

WHERE CONCAT(subcomponentes.Codigo_Homologacion,"-",subcomponentes.Descripcion)

lIKE concat('%',  pCadena , '%');

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteEspecialConsultar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteEspecialConsultar`(

pCadena nvarchar(300)

)
BEGIN

SELECT Id_subcomponente,

CONCAT(subcomponentes.Codigo_Homologacion , "-" , acabados.Codigo_Homologacion)  codigo,

subcomponentes.Descripcion

FROM subcomponentes JOIN acabados

ON subcomponentes.Id_Acabado = acabados.Id_Acabado

WHERE CONCAT(subcomponentes.Codigo_Homologacion,"-",subcomponentes.Descripcion)

lIKE concat('%',  pCadena , '%') AND subcomponentes.Especial = true;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteMamparaAgrupar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteMamparaAgrupar`()
BEGIN

SET SQL_SAFE_UPDATES = 0;

SELECT codigo,

mp.descripcion,

a.Descripcion AcabadoDesc,

SUM(cast(medida as decimal(18,2))) medida,

SUM(puertas) puertas,

SUM(areapuertas) areapuertas

FROM proyecto_mp mp

inner join acabados a on a.Codigo_homologacion = substring(mp.codigo,LOCATE('-',mp.codigo)+1)

GROUP BY codigo;

delete from proyecto_mp;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponentePuertaAgrupar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponentePuertaAgrupar`(

pSwHerraje int,

pSwAP int

)
BEGIN

IF (pSwAP = 0) THEN

IF (pSwHerraje = 0) THEN

select cd.Id_Subcomponente,

concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,

subcomponente.Descripcion,

acabado.Descripcion AcabadoDesc,

case when proy.extra = 1 then max(proy.cantidad)

when  proy.Id_Unidad_Medida in (1,7) then round(sum(proy.medida) / if(Id_Unidad_Medida = 1,max(proy.medida),1000),2)

when  proy.Id_Unidad_Medida in (2,3) then sum(proy.cantidad)  else count(proy.medida) end cantidad,

case when  proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 then proy.medida else

(select sum(proyone.medida) from proyecto_Pt proyone

JOIN componentes_detalle cdone ON cdone.id = proyone.Id_Subcomponente

where cdone.Id_Subcomponente = cd.Id_Subcomponente and proyone.puerta = proy.puerta)

end medidaC,

case when cd.medida = 2 and proy.Id_Unidad_Medida != 6 then

case when proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN proy.medidaAdicional else 0 end

when cd.medida = 1 then 0 else proy.medidaAdicional end medidaCalculada,

fnUnidadCalculada(proy.Id_Unidad_Medida) unidaMedida,

proy.puerta,

ct.descripcion Corte,

m.descripcion Mecanizado,

proy.Codigo CodigoComponente,

cd.extra,

cd.Asignacion_puertas

from proyecto_Pt proy

JOIN componentes_detalle cd ON cd.id = proy.Id_Subcomponente

JOIN subcomponentes subcomponente ON cd.Id_Subcomponente = subcomponente.Id_Subcomponente

JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado

left JOIN cortes ct ON ct.Id_Corte = cd.idCorte

left JOIN mecanizados m ON m.Id_Mecanizado = cd.Mecanizado

group by cd.Id_Subcomponente,cd.medida,proy.puerta,proy.Id_Unidad_Medida,ct.descripcion;

else

select cd.Id_Subcomponente,

concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,

subcomponente.Descripcion,

acabado.Descripcion AcabadoDesc,

case when proy.extra = 1 then max(proy.cantidad)

when  proy.Id_Unidad_Medida in (1,7) then round(sum(proy.medida) / if(Id_Unidad_Medida = 1,max(proy.medida),1000),2)

when  proy.Id_Unidad_Medida in (2,3) then sum(proy.cantidad)  else count(proy.medida) end cantidad,

case when  proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 then proy.medida else

(select sum(proyone.medida) from proyecto_Pt proyone

JOIN componentes_detalle cdone ON cdone.id = proyone.Id_Subcomponente

where cdone.Id_Subcomponente = cd.Id_Subcomponente and proyone.puerta = proy.puerta)

end medidaC,

case when cd.medida = 2 and proy.Id_Unidad_Medida != 6 then

case when proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN proy.medidaAdicional else 0 end

when cd.medida = 1 then 0 else proy.medidaAdicional end medidaCalculada,

fnUnidadCalculada(proy.Id_Unidad_Medida) unidaMedida,

ct.descripcion Corte,

m.descripcion Mecanizado,

proy.Codigo CodigoComponente,

cd.extra,

cd.Asignacion_puertas

from proyecto_Pt proy

JOIN componentes_detalle cd ON cd.id = proy.Id_Subcomponente

JOIN subcomponentes subcomponente ON cd.Id_Subcomponente = subcomponente.Id_Subcomponente

JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado

left JOIN cortes ct ON ct.Id_Corte = cd.idCorte

left JOIN mecanizados m ON m.Id_Mecanizado = cd.Mecanizado

group by cd.Id_Subcomponente,cd.medida,proy.Id_Unidad_Medida,ct.descripcion;

END IF;

ELSE

IF (pSwHerraje = 0) THEN

select cd.Id_Subcomponente,

concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,

subcomponente.Descripcion,

acabado.Descripcion AcabadoDesc,

case when proy.extra = 1 then max(proy.cantidad)

when  proy.Id_Unidad_Medida in (1,7) then round(sum(proy.medida) / if(Id_Unidad_Medida = 1,max(proy.medida),1000),2)

when  proy.Id_Unidad_Medida in (2,3) then sum(proy.cantidad)  else count(proy.medida) end cantidad,

case when  proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 then proy.medida else

(select sum(proyone.medida) from proyecto_Pt proyone

JOIN componentes_detalle cdone ON cdone.id = proyone.Id_Subcomponente

where cdone.Id_Subcomponente = cd.Id_Subcomponente and proyone.puerta = proy.puerta

and cdone.Asignacion_puertas = pSwAP)

end medidaC,

case when cd.medida = 2 and proy.Id_Unidad_Medida != 6 then

case when proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN proy.medidaAdicional else 0 end

when cd.medida = 1 then 0 else proy.medidaAdicional end medidaCalculada,

fnUnidadCalculada(proy.Id_Unidad_Medida) unidaMedida,

proy.puerta,

ct.descripcion Corte,

m.descripcion Mecanizado,

proy.Codigo CodigoComponente,

cd.extra,

cd.Asignacion_puertas

from proyecto_Pt proy

JOIN componentes_detalle cd ON cd.id = proy.Id_Subcomponente

JOIN subcomponentes subcomponente ON cd.Id_Subcomponente = subcomponente.Id_Subcomponente

JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado

left JOIN cortes ct ON ct.Id_Corte = cd.idCorte

left JOIN mecanizados m ON m.Id_Mecanizado = cd.Mecanizado

where cd.Asignacion_puertas = pSwAP

group by cd.Id_Subcomponente,cd.medida,proy.puerta,proy.Id_Unidad_Medida,ct.descripcion;

else

select cd.Id_Subcomponente,

concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,

subcomponente.Descripcion,

acabado.Descripcion AcabadoDesc,

case when proy.extra = 1 then max(proy.cantidad)

when  proy.Id_Unidad_Medida in (1,7) then round(sum(proy.medida) / if(Id_Unidad_Medida = 1,max(proy.medida),1000),2)

when  proy.Id_Unidad_Medida in (2,3) then sum(proy.cantidad)  else count(proy.medida) end cantidad,

case when  proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 then proy.medida else

(select sum(proyone.medida) from proyecto_Pt proyone

JOIN componentes_detalle cdone ON cdone.id = proyone.Id_Subcomponente

where cdone.Id_Subcomponente = cd.Id_Subcomponente and proyone.puerta = proy.puerta

and cdone.Asignacion_puertas = pSwAP)

end medidaC,

case when cd.medida = 2 and proy.Id_Unidad_Medida != 6 then

case when proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN proy.medidaAdicional else 0 end

when cd.medida = 1 then 0 else proy.medidaAdicional end medidaCalculada,

fnUnidadCalculada(proy.Id_Unidad_Medida) unidaMedida,

ct.descripcion Corte,

m.descripcion Mecanizado,

proy.Codigo CodigoComponente,

cd.extra,

cd.Asignacion_puertas

from proyecto_Pt proy

JOIN componentes_detalle cd ON cd.id = proy.Id_Subcomponente

JOIN subcomponentes subcomponente ON cd.Id_Subcomponente = subcomponente.Id_Subcomponente

JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado

left JOIN cortes ct ON ct.Id_Corte = cd.idCorte

left JOIN mecanizados m ON m.Id_Mecanizado = cd.Mecanizado

where cd.Asignacion_puertas = pSwAP

group by cd.Id_Subcomponente,cd.medida,proy.Id_Unidad_Medida,ct.descripcion;

END IF;

END IF;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteRegistrar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteRegistrar`(

pId_Acabado int,

pCodigo nvarchar(10),

pDescripcion nvarchar(100),

pEspecial  boolean,

pIdComponente int

)
BEGIN

INSERT subcomponentes (Id_Acabado,

Codigo_Homologacion,

Descripcion,Especial) VALUES (pId_Acabado,

pCodigo,

pDescripcion,pEspecial

);

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubcomponenteUpdate` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubcomponenteUpdate`(

pIdAcabdo int,

pCodigo nvarchar(50),

pDescripcion  nvarchar(300),

pIdComponente int,

pEspecial boolean

)
BEGIN

UPDATE `subcomponentes`

SET

`Id_Acabado` =pIdAcabdo,

`Codigo_Homologacion` = pCodigo,

`Descripcion` = pDescripcion,

`Especial` = pEspecial

WHERE `Id_Subcomponente` = pIdComponente;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteVidrioPanelAgrupar` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'NO_AUTO_VALUE_ON_ZERO' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSubComponenteVidrioPanelAgrupar`()
BEGIN

SET SQL_SAFE_UPDATES = 0;

SELECT proy.Id_Subcomponente,

concat(subcomponente.codigo_homologacion, "-",acabado.codigo_homologacion ) codigo,

subcomponente.Descripcion,

acabado.Descripcion acabadoDes,

proy.Altura,

proy.Anchura,

sum(proy.cantidad) cantidad,

proy.Ubicacion,

ifnull(proy.medidaAdicional,0) medidaC,

0 medidaCalculada,

fnUnidadCalculadaVidrioPanel(ifnull(proy.Id_Unidad_Medida,0)) unidaMedida

FROM proyecto_vp proy

JOIN subcomponentes subcomponente ON proy.Id_Subcomponente = subcomponente.Id_Subcomponente

JOIN acabados acabado ON acabado.Id_Acabado = subcomponente.Id_Acabado

WHERE Anchura <> 0

GROUP BY proy.Id_Subcomponente,proy.Id_Unidad_Medida, altura, anchura,proy.Ubicacion;

delete FROM proyecto_vp;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-07-06 20:48:12

-- =====================================================================
-- PERMISOS (opcional): el usuario con el que la app beta se conecta
-- (por defecto 'remote') necesita acceso a arquitectdb_beta. Si al abrir
-- la beta da "Access denied", descomenta y ejecuta esto (ajusta el host
-- 'remote'@'%' al que use tu servidor; si el usuario no existe, créalo):
--
-- GRANT ALL PRIVILEGES ON `arquitectdb_beta`.* TO 'remote'@'%';
-- FLUSH PRIVILEGES;
-- =====================================================================
