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
        public static String QUERY_INSERT_COMPONENTES = "INSERT arquitectdb.componentes(codigo, descripcion, NoSubcomponente) VALUES (?,?,?);";
        public static String QUERY_COMPONENTES = "SELECT Id_Componente,Codigo,Descripcion,NoSubcomponente FROM arquitectdb.componentes ";
        public static String QUERY_UPDATE_COMPONENTES = "UPDATE arquitectdb.componentes SET descripcion = ?, NoSubcomponente = ? WHERE Codigo = ?";
        public static String QUERY_INSERT_COMPONENTE_DETALLE = "INSERT arquitectdb.componentes_detalle (Id_Componente,"
            + " Id_Subcomponente,"
            + " Id_Unidad_Calculada,"
            + " Cantidad_Default,"
            + " Cantidad_Adicional,"
            + " Aplica_Decremento) VALUES (?,?,?,?,?,?);";
        public static String QUERY_GET_COMPONENTE_DETALLE = "componenteDetalleCargar";

        public static String QUERY_DELETE_COMPONENTE_DETALLE = "DELETE FROM arquitectdb.componentes_detalle WHERE Id_Componente = ?;";

        //Sub Componente
        public static String QUERY_SUBCOMPONENTES = "SELECT Id_SubComponente,Codigo_Homologacion,Descripcion FROM arquitectdb.subcomponentes ";

        //Unidad Calculada
        public static String QUERY_UNIDADCALCULADA = "SELECT Id_Unidad_Calculada,Descripcion FROM arquitectdb.unidades_calculadas ";


    }
}
