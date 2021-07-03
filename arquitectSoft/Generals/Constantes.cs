using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Generals
{
    class Constantes
    {
        public static String QUERY_EXITS_USUARIO = "SELECT Nombre FROM usuario where usuario = ? and contrasena = ?;";
        
        //Componentes
        public static String QUERY_EXITS_COMPONENTES = "SELECT Id_Componente FROM arquitectdb.componentes where Codigo = ? or Descripcion = ?;";
        public static String QUERY_INSERT_COMPONENTES = "INSERT arquitectdb.componentes(codigo, descripcion, Especial,AcabadoPrincipal) VALUES (?,?,?,?);";
        public static String QUERY_COMPONENTES = "SELECT Id_Componente,Codigo,Descripcion,Especial FROM arquitectdb.componentes ";
        public static String QUERY_UPDATE_COMPONENTES = "UPDATE arquitectdb.componentes SET descripcion = ?, Especial = ?,AcabadoPrincipal = ? WHERE Codigo = ?";
        public static String QUERY_INSERT_COMPONENTE_DETALLE = "INSERT arquitectdb.componentes_detalle (Id_Componente,"
            + " Id_Subcomponente,"
            + " Id_Unidad_Calculada,"
            + " Cantidad_Default,"
            + " Cantidad_Adicional,"
            + " Aplica_Decremento,"
            + " elevado,"
            + " idCorte ) VALUES (?,?,?,?,?,?,?,?);";
        public static String QUERY_INSERT_COMPONENTE_ESPECIAL_DETALLE = "INSERT arquitectdb.componentes_especial_detalle (Id_Componente_especial,"
            + " Id_Subcomponente,"
            + " select_Columna,"
            + " Cantidad_Default,"
            + " Cantidad_Adicional ) VALUES (?,?,?,?,?);";
        public static String QUERY_GET_COMPONENTE_DETALLE = "componenteDetalleCargar";
        public static String QUERY_GET_COMPONENTE_ESPECIAL_DETALLE = "componenteEspecialDetalleCargar";
        public static String QUERY_DELETE_COMPONENTE = "DELETE FROM arquitectdb.componentes WHERE Id_Componente = ?;";
        public static String QUERY_DELETE_COMPONENTE_DETALLE = "DELETE FROM arquitectdb.componentes_detalle WHERE Id_Componente = ?;";
        public static String QUERY_DELETE_COMPONENTE_ESPECIAL_DETALLE = "DELETE FROM arquitectdb.componentes_especial_detalle WHERE Id_Componente_especial = ?;";

        //Sub Componente
        public static String QUERY_EXITS_SUBCOMPONENTES = "SELECT Id_SubComponente FROM arquitectdb.subcomponentes where Codigo_Homologacion = ? or Descripcion = ?;";
        public static String QUERY_SUBCOMPONENTES = "SELECT Id_SubComponente,CONCAT(subcomponentes.Codigo_Homologacion , ' - ' , acabados.Codigo_Homologacion) Codigo_Homologacion" +
                                                    ", CONCAT(subcomponentes.Descripcion , '(' , acabados.Descripcion,') ') Descripcion,subcomponentes.Id_Acabado,Especial " +
                                                    "FROM arquitectdb.subcomponentes JOIN arquitectdb.acabados ON subcomponentes.Id_Acabado = acabados.Id_Acabado ";
        public static String QUERY_INSERT_SUBCOMPONENTES = "CALL spSubComponenteRegistrar(?,?,?,?,?);";
        public static String QUERY_UPDATE_SUBCOMPONENTES = "CALL spSubcomponenteUpdate(?,?,?,?,?);";
        public static String QUERY_DELETE_SUBCOMPONENTES = "DELETE FROM arquitectdb.subcomponentes WHERE Id_subcomponente = ?";

        //Unidad Calculada
        public static String QUERY_UNIDADCALCULADA = "SELECT Id_Unidad_Calculada,Descripcion FROM arquitectdb.unidades_calculadas";

        //Acabado
        public static String QUERY_ACABADO = "SELECT Id_Acabado,Codigo_Homologacion,Descripcion FROM arquitectdb.acabados";
        public static String QUERY_INSERT_ACABADO = "INSERT arquitectdb.acabados (Codigo_Homologacion, descripcion)VALUES(?,?)";
        public static String QUERY_DELETE_ACABADO = "DELETE FROM arquitectdb.acabados WHERE Id_Acabado = ?";
        public static String QUERY_UPDATE_ACABADO = "UPDATE arquitectdb.acabados SET descripcion = ? WHERE Id_Acabado = ?";
        public static String QUERY_EXITS_ACABADO = "SELECT Id_Acabado FROM arquitectdb.acabados where Codigo_Homologacion = ? or Descripcion = ?;";

        //Corte
        public static String QUERY_CORTE = "SELECT Id_Corte,Descripcion,Corte_Derecho,Corte_Izquierdo FROM arquitectdb.cortes";
        public static String QUERY_INSERT_CORTE = "INSERT arquitectdb.cortes (descripcion,Corte_Derecho,Corte_Izquierdo) VALUES(?,?,?)";
        public static String QUERY_DELETE_CORTE = "DELETE FROM arquitectdb.cortes WHERE Id_Corte = ?";
        public static String QUERY_UPDATE_CORTE = "UPDATE arquitectdb.cortes SET descripcion = ?,Corte_Derecho = ?, Corte_Izquierdo = ? WHERE Id_Corte = ?";
        public static String QUERY_EXITS_CORTE = "SELECT Id_Corte FROM arquitectdb.cortes where Descripcion = ?;";
        public static String QUERY_CORTE_MAX = "SELECT Max(Id_Corte) + 1 Id_Corte FROM arquitectdb.cortes";

        //Unidad de Medida
        public static String QUERY_UNIDADMEDIDA = "SELECT Id_Unidad_Medida,Descripcion,Convencion FROM arquitectdb.unidades_medidas";
        public static String QUERY_INSERT_UNIDADMEDIDA = "INSERT arquitectdb.unidades_medidas (Descripcion,Convencion) VALUES(?,?)";
        public static String QUERY_DELETE_UNIDADMEDIDA = "DELETE FROM arquitectdb.unidades_medidas WHERE Id_Unidad_Medida = ?";
        public static String QUERY_UPDATE_UNIDADMEDIDA = "UPDATE arquitectdb.unidades_medidas SET descripcion = ?,Convencion = ? WHERE Id_Unidad_Medida = ?";
        public static String QUERY_EXITS_UNIDADMEDIDA = "SELECT Id_Corte FROM arquitectdb.cortes where Descripcion = ?;";
        public static String QUERY_UNIDADMEDIDA_MAX = "SELECT Max(Id_Unidad_Medida) + 1 Id_Corte FROM arquitectdb.unidades_medidas";

        //Analisis de Datos
        public static String QUERY_GET_CALCULATE_VIDRIOPANEL = "spComponenteVidrioPanel";
        public static String QUERY_INSERT_AUXANCHURA = "INSERT tbauxanchura(Codigo, Altura, Columna1,"
                        + "Columna2,"
                        + "Columna3,"
                        + "Columna4,"
                        + "Columna5)VALUES(?,?,?,?,?,?,?);";

        public static String QUERY_GET_AUXANCHURA = "SELECT componentes_especial.Codigo,componentes_especial_detalle.id,select_Columna, " +
                                "componentes_especial.descripcion, concat('Columna #',select_Columna) Columns FROM  componentes_especial_detalle " +
                        "JOIN componentes_especial ON componentes_especial_detalle.Id_Componente_especial = componentes_especial.Id_Componente_especial " +
                        "WHERE componentes_especial.Codigo = ? ";
    }
}
