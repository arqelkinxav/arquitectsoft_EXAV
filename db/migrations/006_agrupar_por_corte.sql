-- =====================================================================
-- 006_agrupar_por_corte.sql
--
-- BUG: una pieza con el mismo codigo, descripcion, acabado, cantidad
-- extra y tipo de medida pero con DOS cortes distintos (90/45...) salia
-- duplicada y cada fila con el TOTAL de la pieza (todas las piezas con
-- un corte y otra vez todas con el otro) -> el material se contaba doble.
--
-- Causa: spSubComponenteAgrupar agrupa por corte (GROUP BY ... corte),
-- pero las subconsultas que suman cantidad y metros no filtraban por
-- corte. Igual en spSubComponentePuertaAgrupar (medidaC de las piezas
-- lineales, agrupadas por ct.descripcion). Se anade el filtro de corte
-- (<=> para que los cortes vacios/NULL casen entre si). No cambia nada
-- mas: una fila por corte, cada una con SUS piezas.
--
-- Generado desde las rutinas vigentes de Olimpo (17-sep-2026).
-- Idempotente.
-- =====================================================================
SET SESSION sql_mode = 'NO_AUTO_VALUE_ON_ZERO';

/*!50003 DROP PROCEDURE IF EXISTS `spSubComponenteAgrupar` */;
DELIMITER ;;
CREATE PROCEDURE `spSubComponenteAgrupar`(

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
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and  proyect.medidaAdicional = proy.medidaAdicional)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
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
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and  proyect.medidaAdicional = proy.medidaAdicional)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
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
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and  proyect.medidaAdicional = proy.medidaAdicional and proyect.ubicacion = proy.ubicacion)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and proyect.ubicacion = proy.ubicacion) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and proyect.ubicacion = proy.ubicacion) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and proyect.ubicacion = proy.ubicacion) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
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
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida and  proyect.medidaAdicional = proy.medidaAdicional)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
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
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and  proyect.medidaAdicional = proy.medidaAdicional)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
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
		THEN (SELECT SUM(proyect.cantidad) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and  proyect.medidaAdicional = proy.medidaAdicional)
		WHEN mod((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000),1) < 0.1 then
		FLOOR((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000) ) * pDesperdicio
		else
		ceiling(((SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte
		and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio) / if(Id_Unidad_Medida = 1,(proy.medidaAdicional),1000)) END cantidad,
		proy.medidaAdicional medidaC,
		CASE WHEN proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 THEN 0
		ELSE (SELECT SUM(proyect.medida) FROM proyecto proyect WHERE proyect.Id_Subcomponente = proy.Id_Subcomponente and proyect.corte <=> proy.corte and proyect.Id_Unidad_Medida = proy.Id_Unidad_Medida) * pDesperdicio END medidaCalculada,
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

/*!50003 DROP PROCEDURE IF EXISTS `spSubComponentePuertaAgrupar` */;
DELIMITER ;;
CREATE PROCEDURE `spSubComponentePuertaAgrupar`(

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

when  proy.Id_Unidad_Medida in (2,3,8) then sum(proy.cantidad)  else count(proy.medida) end cantidad,

case when  proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 then proy.medida else

(select sum(proyone.medida) from proyecto_Pt proyone

JOIN componentes_detalle cdone ON cdone.id = proyone.Id_Subcomponente

where cdone.Id_Subcomponente = cd.Id_Subcomponente and proyone.puerta = proy.puerta and cdone.idCorte <=> cd.idCorte)

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

when  proy.Id_Unidad_Medida in (2,3,8) then sum(proy.cantidad)  else count(proy.medida) end cantidad,

case when  proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 then proy.medida else

(select sum(proyone.medida) from proyecto_Pt proyone

JOIN componentes_detalle cdone ON cdone.id = proyone.Id_Subcomponente

where cdone.Id_Subcomponente = cd.Id_Subcomponente and proyone.puerta = proy.puerta and cdone.idCorte <=> cd.idCorte)

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

when  proy.Id_Unidad_Medida in (2,3,8) then sum(proy.cantidad)  else count(proy.medida) end cantidad,

case when  proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 then proy.medida else

(select sum(proyone.medida) from proyecto_Pt proyone

JOIN componentes_detalle cdone ON cdone.id = proyone.Id_Subcomponente

where cdone.Id_Subcomponente = cd.Id_Subcomponente and proyone.puerta = proy.puerta and cdone.idCorte <=> cd.idCorte

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

when  proy.Id_Unidad_Medida in (2,3,8) then sum(proy.cantidad)  else count(proy.medida) end cantidad,

case when  proy.Id_Unidad_Medida != 1 and proy.Id_Unidad_Medida != 7 then proy.medida else

(select sum(proyone.medida) from proyecto_Pt proyone

JOIN componentes_detalle cdone ON cdone.id = proyone.Id_Subcomponente

where cdone.Id_Subcomponente = cd.Id_Subcomponente and proyone.puerta = proy.puerta and cdone.idCorte <=> cd.idCorte

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
