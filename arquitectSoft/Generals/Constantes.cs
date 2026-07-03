using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Generals
{
    class Constantes
    {
        
        public static String QUERY_EXITS_USUARIO = "SELECT Nombre,usuario FROM usuario where usuario = ? and contrasena = ?;";
        
        //Componentes
        public static String QUERY_EXITS_COMPONENTES = "SELECT Id_Componente FROM componentes where (Codigo = ? or Descripcion = ?) and AcabadoPrincipal = ? LIMIT 1;";
        public static String QUERY_INSERT_COMPONENTES = "INSERT componentes(codigo, descripcion, Especial,AcabadoPrincipal) VALUES (?,?,?,?);";
        public static String QUERY_COMPONENTES = "SELECT Id_Componente,CONCAT(Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),'')) Codigo,componentes.Descripcion,Especial,AcabadoPrincipal" +
                                                    " FROM componentes " +
                                                            "LEFT JOIN acabados ON acabados.Id_Acabado = AcabadoPrincipal ";
        public static String QUERY_UPDATE_COMPONENTES = "UPDATE componentes SET descripcion = ?, Especial = ?,AcabadoPrincipal = ? WHERE Codigo = ?";
        public static String QUERY_INSERT_COMPONENTE_DETALLE = "INSERT componentes_detalle (Id_Componente,"
            + " Id_Subcomponente,"
            + " Id_Unidad_Calculada,"
            + " Cantidad_Default,"
            + " Cantidad_Adicional,"
            + " Aplica_Decremento,"
            + " elevado,"
            + " idCorte, extra,Medida,Cantidad_Adicional_Anch,Aplica_Decremento_Anch,Mecanizado,Asignacion_puertas ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?);";
        public static String QUERY_INSERT_COMPONENTE_ESPECIAL_DETALLE = "INSERT componentes_especial_detalle (Id_Componente_especial,"
            + " Id_Subcomponente,"
            + " select_Columna,"
            + " Cantidad_Default,"
            + " Cantidad_Adicional ) VALUES (?,?,?,?,?);";
        public static String QUERY_GET_COMPONENTE_DETALLE = "componenteDetalleCargar";
        public static String QUERY_GET_COMPONENTE_ESPECIAL_DETALLE = "componenteEspecialDetalleCargar";
        public static String QUERY_DELETE_COMPONENTE = "DELETE FROM componentes WHERE Id_Componente = ?;";
        public static String QUERY_DELETE_COMPONENTE_DETALLE = "DELETE FROM componentes_detalle WHERE Id_Componente = ?;";
        public static String QUERY_DELETE_COMPONENTE_ESPECIAL_DETALLE = "DELETE FROM componentes_especial_detalle WHERE Id_Componente_especial = ?;";

        //Sub Componente
        public static String QUERY_EXITS_SUBCOMPONENTES = "SELECT Id_SubComponente FROM subcomponentes where (Codigo_Homologacion = ? or Descripcion = ?)";
        public static String QUERY_SUBCOMPONENTES = "SELECT Id_SubComponente,CONCAT(subcomponentes.Codigo_Homologacion , '-' , acabados.Codigo_Homologacion) Codigo_Homologacion" +
                                                    ", CONCAT(subcomponentes.Descripcion , '(' , acabados.Descripcion,') ') Descripcion,subcomponentes.Id_Acabado,Especial,acabados.Descripcion as DescripcionAcabado " +
                                                    "FROM subcomponentes JOIN acabados ON subcomponentes.Id_Acabado = acabados.Id_Acabado ";
        public static String QUERY_INSERT_SUBCOMPONENTES = "CALL spSubComponenteRegistrar(?,?,?,?,?);";
        public static String QUERY_UPDATE_SUBCOMPONENTES = "CALL spSubcomponenteUpdate(?,?,?,?,?);";
        public static String QUERY_DELETE_SUBCOMPONENTES = "DELETE FROM subcomponentes WHERE Id_subcomponente = ?";
        public static String QUERY_GET_SUBCOMPONENTE_RELACION = "componenteRelationSub";
        public static String QUERY_CHANGE_SUBCOMPONENTES = "UPDATE componentes_detalle SET Id_Subcomponente = ? WHERE Id_Subcomponente = ?";

        //Unidad Calculada
        public static String QUERY_UNIDADCALCULADA = "SELECT Id_Unidad_Calculada,Concat(Id_Unidad_Calculada, ' - ', Descripcion) Descripcion FROM unidades_calculadas";
        
        //Mecanizado
        public static String QUERY_MECANIZADO = "SELECT Id_mecanizado,Codigo_Homologacion,Descripcion FROM mecanizados";
        public static String QUERY_INSERT_MECANIZADO = "INSERT mecanizados (Codigo_Homologacion, descripcion)VALUES(?,?)";
        public static String QUERY_DELETE_MECANIZADO = "DELETE FROM mecanizados WHERE Id_mecanizado = ?";
        public static String QUERY_UPDATE_MECANIZADO = "UPDATE mecanizados SET descripcion = ? WHERE Id_mecanizado = ?";
        public static String QUERY_EXITS_MECANIZADO = "SELECT Id_mecanizado FROM mecanizados where Id_mecanizado = ? ;";
        public static String QUERY_MECANIZADO_MAX = "SELECT Max(Id_mecanizado) + 1 Id_Corte FROM mecanizados";

        //Acabado
        public static String QUERY_ACABADO = "SELECT Id_Acabado,Codigo_Homologacion,CONCAT(Codigo_Homologacion , ' - ' ,Descripcion) Descripcion FROM acabados";

        public static String QUERY_INSERT_ACABADO = "INSERT acabados (Codigo_Homologacion, descripcion)VALUES(?,?)";
        public static String QUERY_DELETE_ACABADO = "DELETE FROM acabados WHERE Id_Acabado = ?";
        public static String QUERY_UPDATE_ACABADO = "UPDATE acabados SET Codigo_Homologacion = ?, descripcion = ? WHERE Id_Acabado = ?";
        public static String QUERY_EXITS_ACABADO = "SELECT Id_Acabado FROM acabados where Codigo_Homologacion = ? or Descripcion = ?;";
        // ¿OTRO acabado (distinto del actual) ya usa este código? (para validar la edición de código)
        public static String QUERY_ACABADO_COD_OTRO = "SELECT Id_Acabado FROM acabados WHERE Codigo_Homologacion = ? AND Id_Acabado <> ? LIMIT 1;";

        // ===== Dependencias de acabado (BETA) =====
        // Placeholders MOD… (acabados "variables" que dependen de la perfilería).
        public static String QUERY_ACABADOS_MOD = "SELECT Codigo_Homologacion, Descripcion FROM acabados WHERE Codigo_Homologacion LIKE 'MOD%' ORDER BY Codigo_Homologacion";
        // Acabados REALES (sin los MOD…) para elegir el resultado y para los selectores de análisis/export.
        public static String QUERY_ACABADOS_REALES = "SELECT Id_Acabado, Codigo_Homologacion, CONCAT(Codigo_Homologacion,' - ',Descripcion) Descripcion FROM acabados WHERE Codigo_Homologacion NOT LIKE 'MOD%' ORDER BY Codigo_Homologacion";
        // Mapa código→descripción de TODOS los acabados (para resolver el resultado).
        public static String QUERY_ACABADOS_DESC = "SELECT Codigo_Homologacion, Descripcion FROM acabados";
        // Reglas de dependencia: (placeholder, valor de perfilería) → acabado resultante.
        public static String QUERY_DEPENDENCIAS = "SELECT Id, Cod_Placeholder, Cod_Perfileria, Cod_Resultado FROM beta_dependencias_acabado";
        // Reglas con descripciones (para la pantalla de configuración).
        public static String QUERY_DEPENDENCIAS_DETALLE =
            "SELECT d.Cod_Placeholder, IFNULL(ph.Descripcion, d.Cod_Placeholder) Placeholder, " +
            "d.Cod_Perfileria, IFNULL(pf.Descripcion, d.Cod_Perfileria) Perfileria, " +
            "d.Cod_Resultado, IFNULL(rs.Descripcion, d.Cod_Resultado) Resultado " +
            "FROM beta_dependencias_acabado d " +
            "LEFT JOIN acabados ph ON ph.Codigo_Homologacion = d.Cod_Placeholder " +
            "LEFT JOIN acabados pf ON pf.Codigo_Homologacion = d.Cod_Perfileria " +
            "LEFT JOIN acabados rs ON rs.Codigo_Homologacion = d.Cod_Resultado " +
            "ORDER BY d.Cod_Placeholder, d.Cod_Perfileria";
        public static String QUERY_UPSERT_DEPENDENCIA = "INSERT beta_dependencias_acabado (Cod_Placeholder, Cod_Perfileria, Cod_Resultado) VALUES (?,?,?) ON DUPLICATE KEY UPDATE Cod_Resultado = VALUES(Cod_Resultado)";
        // Al renombrar el código de un acabado, propaga el cambio a las reglas que lo usen
        // (para que la dependencia no se rompa). Uno por columna.
        public static String QUERY_REN_DEP_PLACEHOLDER = "UPDATE beta_dependencias_acabado SET Cod_Placeholder = ? WHERE Cod_Placeholder = ?";
        public static String QUERY_REN_DEP_PERFILERIA  = "UPDATE beta_dependencias_acabado SET Cod_Perfileria = ? WHERE Cod_Perfileria = ?";
        public static String QUERY_REN_DEP_RESULTADO   = "UPDATE beta_dependencias_acabado SET Cod_Resultado = ? WHERE Cod_Resultado = ?";
        public static String QUERY_DELETE_DEPENDENCIA = "DELETE FROM beta_dependencias_acabado WHERE Cod_Placeholder = ? AND Cod_Perfileria = ?";

        //Corte
        public static String QUERY_CORTE = "SELECT Id_Corte,Descripcion,Corte_Derecho,Corte_Izquierdo FROM cortes";
        public static String QUERY_INSERT_CORTE = "INSERT cortes (descripcion,Corte_Derecho,Corte_Izquierdo) VALUES(?,?,?)";
        public static String QUERY_DELETE_CORTE = "DELETE FROM cortes WHERE Id_Corte = ?";
        public static String QUERY_UPDATE_CORTE = "UPDATE cortes SET descripcion = ?,Corte_Derecho = ?, Corte_Izquierdo = ? WHERE Id_Corte = ?";
        public static String QUERY_EXITS_CORTE = "SELECT Id_Corte FROM cortes where Descripcion = ?;";
        public static String QUERY_CORTE_MAX = "SELECT Max(Id_Corte) + 1 Id_Corte FROM cortes";

        //Unidad de Medida
        public static String QUERY_UNIDADMEDIDA = "SELECT Id_Unidad_Medida, Descripcion,Convencion FROM unidades_medidas";
        public static String QUERY_INSERT_UNIDADMEDIDA = "INSERT unidades_medidas (Descripcion,Convencion) VALUES(?,?)";
        public static String QUERY_DELETE_UNIDADMEDIDA = "DELETE FROM unidades_medidas WHERE Id_Unidad_Medida = ?";
        public static String QUERY_UPDATE_UNIDADMEDIDA = "UPDATE unidades_medidas SET descripcion = ?,Convencion = ? WHERE Id_Unidad_Medida = ?";
        public static String QUERY_EXITS_UNIDADMEDIDA = "SELECT Id_Unidad_Medida FROM unidades_medidas where Id_Unidad_Medida = ?;";
        public static String QUERY_UNIDADMEDIDA_MAX = "SELECT Max(Id_Unidad_Medida) + 1 Id_Corte FROM unidades_medidas";

        //Analisis de Datos
        public static String QUERY_GET_CALCULATE_VIDRIOPANEL = "spComponenteVidrioPanelv2";
        public static String QUERY_INSERT_AUXANCHURA = "INSERT tbauxanchura(Codigo, Altura, Columna1,"
                        + "Columna2,"
                        + "Columna3,"
                        + "Columna4,"
                        + "Columna5,ubicacion)VALUES(?,?,?,?,?,?,?,?);";

        public static String QUERY_GET_AUXANCHURA = "select distinct Codigo,Altura,ubicacion FROM tbauxanchura";

        public static String QUERY_INSERT_PROYECTO = "INSERT INTO proyecto "
                        + "(Id_Subcomponente, "
                        + " Id_Unidad_Medida, "
                        + " cantidad, "
                        + " medidaAdicional,"
                        + " medida,Corte,ubicacion,Mecanizado) "
                        + "VALUES (?,?,?,?,?,?,?,?)";        

        public static String QUERY_INSERT_PROYECTO_VIDRIO_PANEL = "INSERT INTO proyecto_vp "
                        + "(Id_Subcomponente, "
                        + " Altura, "
                        + " Anchura,"
                        + " Cantidad,Ubicacion) "
                        + "VALUES (?,?,?,?,?)";

        public static String QUERY_INSERT_PROYECTO_MAMPARAS = "INSERT INTO proyecto_mp "
                       + "(codigo, "
                       + " descripcion, "
                       + " medida,puertas,areapuertas)"
                       + "VALUES (?,?,?,?,?)";

        public static String QUERY_GET_PROYECTO = "spSubComponenteAgrupar";

        public static String QUERY_GET_PROYECTO_VIDRIO_PANEL = "spSubComponenteVidrioPanelAgrupar";

        public static String QUERY_GET_CALCULATE_PERFILES = "spComponentePerfilesCargar";       

        public static String QUERY_GET_CALCULATE_MAMPARAS = "spSubComponenteMamparaAgrupar";

        public static String QUERY_GET_CALCULATE_PUERTAS = "componentePuertaDetalleCargar";

        public static String QUERY_GET_CALCULATE_PUERTAS_GENERAL = "componentePuertaCargar";

        public static String QUERY_GET_CALCULATE_PUERTAS_AGRUPAR = "spSubComponentePuertaAgrupar";

        public static String QUERY_GET_COMPONENTE_CODIGOACABADO = "spComponenteGetCodigoAcabado";

        //Managments data base
        public static String QUERY_INSERT_dbmanagmet = "INSERT dbmanagments (filename) VALUES (?)";
    }
}
