using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Generals
{
    class Constantes
    {
        
        public static String QUERY_EXITS_USUARIO = "SELECT id,Nombre,usuario,rol FROM usuario where usuario = ? and contrasena = ?;";

        //Usuarios (gestión de cuentas y permisos)
        public static String QUERY_USUARIOS = "SELECT id,usuario,Nombre,contrasena,rol FROM usuario ORDER BY rol,usuario;";
        // Existe otro usuario con el mismo login (excluyendo el id indicado; para alta usar id = 0).
        public static String QUERY_USUARIO_LOGIN_OTRO = "SELECT id FROM usuario WHERE usuario = ? AND id <> ? LIMIT 1;";
        public static String QUERY_INSERT_USUARIO = "INSERT INTO usuario(usuario,contrasena,Nombre,rol,estado) VALUES (?,?,?,?,1);";
        public static String QUERY_UPDATE_USUARIO = "UPDATE usuario SET usuario = ?, contrasena = ?, Nombre = ?, rol = ? WHERE id = ?;";
        public static String QUERY_DELETE_USUARIO = "DELETE FROM usuario WHERE id = ?;";
        // Cambio de la propia clave: verifica la actual y actualiza por id.
        public static String QUERY_VERIFICAR_CLAVE = "SELECT id FROM usuario WHERE id = ? AND contrasena = ? LIMIT 1;";
        public static String QUERY_CAMBIAR_CLAVE = "UPDATE usuario SET contrasena = ? WHERE id = ?;";
        // Cambio del propio nombre legible (no del login) por id.
        public static String QUERY_CAMBIAR_NOMBRE = "UPDATE usuario SET Nombre = ? WHERE id = ?;";

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

        // ===== Dependencias de vidrio (BETA) =====
        // Cada sistema (prefijo del código del componente: DV, IT, AV…) tiene en la base sus
        // subcomponentes cargados como su tipo de vidrio ESTÁNDAR. Al pedir otro tipo, las
        // reglas dicen qué subcomponente pasa a ser cuál dentro de ese sistema.
        // Ver db/migrations/005_dependencias_vidrio.sql.
        public static String QUERY_VIDRIO_TIPOS = "SELECT Id, Nombre FROM beta_vidrio_tipo ORDER BY Orden, Nombre";
        public static String QUERY_VIDRIO_SISTEMAS =
            "SELECT s.Id, s.Prefijo, IFNULL(s.Descripcion,'') Descripcion, IFNULL(s.Id_Tipo_Estandar,0) Id_Tipo_Estandar, " +
            "IFNULL(t.Nombre,'(sin estándar)') Estandar " +
            "FROM beta_vidrio_sistema s LEFT JOIN beta_vidrio_tipo t ON t.Id = s.Id_Tipo_Estandar " +
            "ORDER BY s.Prefijo";
        public static String QUERY_INSERT_VIDRIO_SISTEMA =
            "INSERT beta_vidrio_sistema (Prefijo, Descripcion, Id_Tipo_Estandar) VALUES (?,?,?) " +
            "ON DUPLICATE KEY UPDATE Descripcion = VALUES(Descripcion), Id_Tipo_Estandar = VALUES(Id_Tipo_Estandar)";
        public static String QUERY_UPDATE_VIDRIO_SISTEMA =
            "UPDATE beta_vidrio_sistema SET Prefijo = ?, Descripcion = ?, Id_Tipo_Estandar = ? WHERE Id = ?";
        public static String QUERY_DELETE_VIDRIO_SISTEMA = "DELETE FROM beta_vidrio_sistema WHERE Id = ?";
        public static String QUERY_DELETE_VIDRIO_REGLAS_SISTEMA = "DELETE FROM beta_vidrio_regla WHERE Id_Sistema = ?";
        // Reglas de un (sistema, tipo) con el código y la descripción de cada subcomponente.
        // El filtro se concatena en el Dto (Conexion.ExecuteDataSet no admite parámetros).
        public static String QUERY_VIDRIO_REGLAS_DETALLE =
            "SELECT r.Id, r.Id_Sub_Origen, r.Id_Sub_Destino, " +
            "CONCAT(IFNULL(so.Codigo_Homologacion,'?'),'-',IFNULL(ao.Codigo_Homologacion,'?')) CodOrigen, " +
            "IFNULL(so.Descripcion,'(subcomponente borrado)') DescOrigen, " +
            "CONCAT(IFNULL(sd.Codigo_Homologacion,'?'),'-',IFNULL(ad.Codigo_Homologacion,'?')) CodDestino, " +
            "IFNULL(sd.Descripcion,'(subcomponente borrado)') DescDestino " +
            "FROM beta_vidrio_regla r " +
            "LEFT JOIN subcomponentes so ON so.Id_SubComponente = r.Id_Sub_Origen " +
            "LEFT JOIN acabados ao ON ao.Id_Acabado = so.Id_Acabado " +
            "LEFT JOIN subcomponentes sd ON sd.Id_SubComponente = r.Id_Sub_Destino " +
            "LEFT JOIN acabados ad ON ad.Id_Acabado = sd.Id_Acabado ";
        public static String QUERY_UPSERT_VIDRIO_REGLA =
            "INSERT beta_vidrio_regla (Id_Sistema, Id_Tipo, Id_Sub_Origen, Id_Sub_Destino) VALUES (?,?,?,?) " +
            "ON DUPLICATE KEY UPDATE Id_Sub_Destino = VALUES(Id_Sub_Destino)";
        public static String QUERY_DELETE_VIDRIO_REGLA = "DELETE FROM beta_vidrio_regla WHERE Id = ?";
        // Sistemas en plano (aunque no tengan reglas), para el motor y el diálogo del análisis.
        public static String QUERY_VIDRIO_SISTEMAS_MOTOR =
            "SELECT Prefijo, IFNULL(Descripcion,'') Descripcion, IFNULL(Id_Tipo_Estandar,0) Id_Tipo_Estandar " +
            "FROM beta_vidrio_sistema ORDER BY LENGTH(Prefijo) DESC, Prefijo";
        // Código y descripción de cada subcomponente, con el mismo formato que devuelven los SP
        // de cálculo (código + '-' + acabado, descripción cruda): al sustituir una pieza hay que
        // dejar esos dos campos como los habría escrito el SP.
        public static String QUERY_VIDRIO_SUBCOMPONENTES_MAPA =
            "SELECT s.Id_SubComponente, CONCAT(s.Codigo_Homologacion,'-',a.Codigo_Homologacion) Codigo, s.Descripcion " +
            "FROM subcomponentes s JOIN acabados a ON a.Id_Acabado = s.Id_Acabado";
        // Todas las reglas en plano (prefijo + tipo), para el motor.
        public static String QUERY_VIDRIO_REGLAS_MOTOR =
            "SELECT s.Prefijo, IFNULL(s.Id_Tipo_Estandar,0) Id_Tipo_Estandar, r.Id_Tipo, r.Id_Sub_Origen, r.Id_Sub_Destino " +
            "FROM beta_vidrio_regla r JOIN beta_vidrio_sistema s ON s.Id = r.Id_Sistema";

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
