using System;
using System.Collections.Generic;
using System.Data;

namespace arquitectSoft.Engine
{
    /// <summary>Un sistema configurado (prefijo del código del componente) y su tipo de vidrio estándar.</summary>
    public class SistemaVidrio
    {
        public string Prefijo { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoEstandar { get; set; }
        public string TipoEstandar { get; set; }
        /// <summary>False cuando el sistema aparece en el proyecto pero no está configurado.</summary>
        public bool Configurado { get; set; }
    }

    /// <summary>
    /// Sustituye piezas según el TIPO DE VIDRIO elegido para cada sistema. En la base, los
    /// componentes están cargados con el vidrio ESTÁNDAR de su sistema (6+6, 5+5/c/6+6…);
    /// cuando un proyecto pide otro tipo hay que cambiar el vidrio y todo lo que arrastra
    /// (calces, gomas, cintas, perfiles) SOLO dentro de ese sistema.
    ///
    /// Por eso la sustitución NO puede hacerse sobre las tablas finales como en
    /// <see cref="DependenciaResolver"/>: allí las filas ya vienen agrupadas de todo el
    /// proyecto y una misma línea puede sumar cantidades de dos sistemas distintos. Se aplica
    /// dentro del cálculo, componente a componente, mientras todavía se sabe de dónde viene
    /// cada pieza (ver los enganches en <c>Dto.AnalisisDatosDto</c>).
    ///
    /// El sistema de una fila se averigua por su UBICACIÓN (una ubicación = una mampara, y el
    /// código de la mampara lleva el prefijo del sistema) y, si no hay ubicación o no se cargó
    /// el TXT de mamparas, por el prefijo del código del propio componente.
    /// </summary>
    public class VidrioResolver
    {
        // Sistemas configurados, de prefijo más largo a más corto (gana la coincidencia más específica).
        private readonly List<SistemaVidrio> _sistemas = new List<SistemaVidrio>();
        // "PREFIJO|idTipo|idSubOrigen" → idSubDestino
        private readonly Dictionary<string, int> _reglas =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // idSubcomponente → código y descripción tal como los escribe el SP de cálculo.
        private readonly Dictionary<int, string[]> _piezas = new Dictionary<int, string[]>();
        // Nombre de cada tipo (para la pantalla del análisis).
        private readonly Dictionary<int, string> _tipos = new Dictionary<int, string>();

        // Ubicación → código de la mampara de esa ubicación (del TXT 5-).
        private Dictionary<string, string> _ubicaciones =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        // Prefijo → tipo elegido por el usuario para este análisis.
        private Dictionary<string, int> _seleccion =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        /// <summary>True si hay algún sistema dado de alta.</summary>
        public bool HayConfiguracion { get { return _sistemas.Count > 0; } }

        /// <summary>True si el usuario pidió algún tipo distinto del estándar.</summary>
        public bool HaySeleccion
        {
            get
            {
                foreach (var par in _seleccion)
                {
                    SistemaVidrio s = SistemaDePrefijo(par.Key);
                    if (s != null && par.Value > 0 && par.Value != s.IdTipoEstandar) return true;
                }
                return false;
            }
        }

        public IList<SistemaVidrio> Sistemas { get { return _sistemas; } }

        /// <summary>Tipos de vidrio del catálogo (id → nombre).</summary>
        public IDictionary<int, string> Tipos { get { return _tipos; } }

        /// <summary>
        /// Lee configuración y reglas de la base. Si las tablas no existen todavía o falla la
        /// conexión, queda vacío y actúa como no-op (el análisis corre igual que siempre).
        /// </summary>
        public static VidrioResolver Cargar()
        {
            var r = new VidrioResolver();
            try
            {
                var dto = new Dto.VidrioDto();

                DataTable tipos = dto.GetTipos();
                if (tipos != null)
                    foreach (DataRow row in tipos.Rows)
                        r._tipos[Convert.ToInt32(row["Id"])] = Convert.ToString(row["Nombre"]);

                DataTable sistemas = dto.GetSistemasMotor();
                if (sistemas != null)
                {
                    foreach (DataRow row in sistemas.Rows)
                    {
                        int idEstandar = Convert.ToInt32(row["Id_Tipo_Estandar"]);
                        r._sistemas.Add(new SistemaVidrio
                        {
                            Prefijo = Convert.ToString(row["Prefijo"]).Trim(),
                            Descripcion = Convert.ToString(row["Descripcion"]).Trim(),
                            IdTipoEstandar = idEstandar,
                            TipoEstandar = r.NombreTipo(idEstandar),
                            Configurado = true
                        });
                    }
                }

                DataTable reglas = dto.GetReglasMotor();
                if (reglas != null)
                    foreach (DataRow row in reglas.Rows)
                        r._reglas[Clave(Convert.ToString(row["Prefijo"]),
                                        Convert.ToInt32(row["Id_Tipo"]),
                                        Convert.ToInt32(row["Id_Sub_Origen"]))] =
                            Convert.ToInt32(row["Id_Sub_Destino"]);

                DataTable piezas = dto.GetMapaSubcomponentes();
                if (piezas != null)
                    foreach (DataRow row in piezas.Rows)
                        r._piezas[Convert.ToInt32(row["Id_SubComponente"])] = new[]
                        {
                            Convert.ToString(row["Codigo"]),
                            Convert.ToString(row["Descripcion"])
                        };
            }
            catch { /* sin tablas/BD → resolver vacío */ }
            return r;
        }

        /// <summary>Mapa ubicación → código de mampara, leído del TXT de mamparas.</summary>
        public void FijarUbicaciones(IDictionary<string, string> ubicaciones)
        {
            _ubicaciones = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (ubicaciones == null) return;
            foreach (var par in ubicaciones)
                _ubicaciones[(par.Key ?? "").Trim()] = (par.Value ?? "").Trim();
        }

        /// <summary>Tipo elegido por el usuario para cada sistema (prefijo → id de tipo).</summary>
        public void FijarSeleccion(IDictionary<string, int> seleccion)
        {
            _seleccion = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (seleccion == null) return;
            foreach (var par in seleccion)
                _seleccion[(par.Key ?? "").Trim()] = par.Value;
        }

        public IDictionary<string, int> Seleccion { get { return _seleccion; } }

        /// <summary>Nombre del tipo, o "" si no está en el catálogo.</summary>
        public string NombreTipo(int idTipo)
        {
            string n;
            return _tipos.TryGetValue(idTipo, out n) ? n : "";
        }

        /// <summary>
        /// Sistema al que pertenece una pieza: primero por la ubicación (su mampara) y, si no
        /// se puede, por el prefijo del código del propio componente. Devuelve null si no casa
        /// con ningún sistema configurado.
        /// </summary>
        public SistemaVidrio SistemaDe(string codigoComponente, string ubicacion)
        {
            string codMampara;
            if (!string.IsNullOrEmpty(ubicacion) &&
                _ubicaciones.TryGetValue(ubicacion.Trim(), out codMampara))
            {
                SistemaVidrio porUbicacion = SistemaDeCodigo(codMampara);
                if (porUbicacion != null) return porUbicacion;
            }
            return SistemaDeCodigo(codigoComponente);
        }

        /// <summary>Sistema configurado cuyo prefijo encaja con el código (gana el más largo).</summary>
        public SistemaVidrio SistemaDeCodigo(string codigo)
        {
            codigo = (codigo ?? "").Trim();
            if (codigo.Length == 0) return null;
            foreach (SistemaVidrio s in _sistemas)   // ya vienen de prefijo más largo a más corto
                if (s.Prefijo.Length > 0 &&
                    codigo.StartsWith(s.Prefijo, StringComparison.OrdinalIgnoreCase)) return s;
            return null;
        }

        /// <summary>
        /// Si esta pieza debe cambiar por el tipo de vidrio elegido, devuelve true y entrega el
        /// subcomponente que la sustituye (id, código y descripción como los escribiría el SP).
        /// </summary>
        public bool TrySustituir(string codigoComponente, string ubicacion, int idSub,
                                 out int idNuevo, out string codigoNuevo, out string descripcionNueva)
        {
            idNuevo = idSub; codigoNuevo = null; descripcionNueva = null;
            if (_reglas.Count == 0 || idSub <= 0) return false;

            SistemaVidrio s = SistemaDe(codigoComponente, ubicacion);
            if (s == null) return false;

            int tipo;
            if (!_seleccion.TryGetValue(s.Prefijo, out tipo)) return false;
            if (tipo <= 0 || tipo == s.IdTipoEstandar) return false;   // se queda como está en la base

            int destino;
            if (!_reglas.TryGetValue(Clave(s.Prefijo, tipo, idSub), out destino)) return false;
            if (destino <= 0 || destino == idSub) return false;

            idNuevo = destino;
            string[] pieza;
            if (_piezas.TryGetValue(destino, out pieza)) { codigoNuevo = pieza[0]; descripcionNueva = pieza[1]; }
            return true;
        }

        private SistemaVidrio SistemaDePrefijo(string prefijo)
        {
            foreach (SistemaVidrio s in _sistemas)
                if (string.Equals(s.Prefijo, prefijo, StringComparison.OrdinalIgnoreCase)) return s;
            return null;
        }

        private static string Clave(string prefijo, int idTipo, int idSubOrigen)
        {
            return (prefijo ?? "").Trim() + "|" + idTipo + "|" + idSubOrigen;
        }
    }
}
