using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using arquitectSoft.Dto;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// Motor de análisis SIN interfaz de usuario. Es el equivalente exacto de la
    /// lógica de <c>FrmAnalisisDatos.SetDataAll</c>/<c>SetDataView</c> de WinForms,
    /// pero en vez de escribir en DataGridView llena un <see cref="ResultadoAnalisis"/>.
    /// Reutiliza tal cual <see cref="AnalisisDatosDto.CalculateTab"/> (el cálculo no se toca).
    ///
    /// Uso: <see cref="Cargar"/> una vez (clasifica los TXT, como BtnCargar_Click) y
    /// luego <see cref="Ejecutar"/> cuantas veces haga falta al cambiar Medida Base /
    /// % Desperdicio. <see cref="Ejecutar"/> no toca UI ni hilos, así que se puede
    /// llamar en segundo plano (Task.Run).
    /// </summary>
    public class AnalisisEngine
    {
        // --- Estado compartido entre archivos durante una corrida (campos del Form) ---
        private DataTable dtPuertas = new DataTable();
        private DataTable dtVidrio = new DataTable();
        private DataTable dtTubos = new DataTable();
        private DataTable dtPerfil = new DataTable();
        private DataTable dtPerfilCalculate = new DataTable();
        private DataTable dtPerfilHRCalculate = new DataTable();
        private DataTable dtPerfilOfVidrioPanel = new DataTable();
        private DataTable dtPerfilOfTubos = new DataTable();
        private int swPMVertical = 0;

        // --- Archivos cargados ---
        private List<string> _file124;
        private List<string> _file35;

        // Id del despiece de TECHOS (tabla "6- ..."). Sus lamas NO son un documento nuevo:
        // el TXT es una copia del de perfiles metalicos, asi que se tratan como filas mas
        // de ese despiece. Ver 'Cargar' para por que no puede entrar en 'wantedFiles'.
        private const int ID_TECHOS = 6;
        private List<string> _fileTechos;
        private int _idPerfilTechos = -1;

        /// <summary>
        /// Se cargo una tabla de techos pero no habia ningun despiece de perfiles (ni 1- ni
        /// 0-) donde meter sus lamas, asi que se quedaron fuera. Caso raro: un proyecto con
        /// techos y sin un solo perfil de mampara.
        /// </summary>
        public bool TechosIgnorados { get; private set; }
        private int _wantedFiles;
        private string _swSegmentadoUbiInicial = "1";

        public bool DatosCargados { get; private set; }
        public string DirectorioActual { get; private set; }

        // --- Tipo de vidrio por sistema (ver VidrioResolver) ---
        // Ubicación → TIPO (nombre de familia) de la mampara que hay en ella; una ubicación =
        // una mampara. Es lo que permite saber a qué sistema pertenece un perfil o una goma,
        // que por su propio código no lo dicen. Se lee del TXT de mamparas ANTES de calcular,
        // porque en el orden normal ese archivo se procesa el último.
        //
        // OJO: se usa el TIPO y NO el código de la mampara. El código NO distingue monovidrio
        // de doble vidrio: ambos comparten prefijo (ITA01 = mono, ITA05 = doble, los dos "IT").
        // El que sí lo distingue es el nombre de familia, que empieza por MV o DV.
        private readonly Dictionary<string, string> _sistemaDeUbicacion =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        // Tipos de mampara distintos del proyecto (para listar sus sistemas).
        private readonly List<string> _tiposMampara = new List<string>();

        /// <summary>Tipo de vidrio elegido para cada sistema (prefijo → id de tipo).</summary>
        public IDictionary<string, int> SeleccionVidrio { get; set; }

        // --- Información del proyecto leída de un TXT de info (ver Cargar / LeerInfoProyecto) ---
        public string ProyectoCodigo { get; private set; }
        public string ProyectoNombre { get; private set; }
        // Referencia del cliente (etiqueta "referencia:" del TXT, la escribe EXAV_Tools
        // desde el campo "REFERENCIA CLIENTE" de la Información del Proyecto de Revit).
        public string ProyectoReferencia { get; private set; }

        /// <summary>
        /// Formato único "código nombre referencia" usado tanto para el título de la
        /// ventana de análisis como para el nombre del archivo Excel, para que siempre
        /// coincidan. Las partes vacías se omiten; devuelve "" si no hay ninguna.
        /// </summary>
        public static string NombreProyecto(string codigo, string nombre, string referencia = null)
        {
            var partes = new List<string>();
            if (!string.IsNullOrWhiteSpace(codigo)) partes.Add(codigo.Trim());
            if (!string.IsNullOrWhiteSpace(nombre)) partes.Add(nombre.Trim());
            if (!string.IsNullOrWhiteSpace(referencia)) partes.Add(referencia.Trim());
            return string.Join(" ", partes);
        }

        /// <summary>
        /// Clasifica los archivos TXT seleccionados (equivalente a BtnCargar_Click).
        /// No calcula nada todavía.
        /// </summary>
        /// <param name="archivos">Rutas de los .txt elegidos.</param>
        /// <param name="segmentarPorUbicacion">
        /// Respuesta a "¿Segmentar perfiles metálicos por ubicación?" (Sí = true).
        /// </param>
        public void Cargar(IEnumerable<string> archivos, bool segmentarPorUbicacion)
        {
            _swSegmentadoUbiInicial = segmentarPorUbicacion ? "1" : "0";
            swPMVertical = 0;
            ProyectoCodigo = null;
            ProyectoNombre = null;
            ProyectoReferencia = null;
            _sistemaDeUbicacion.Clear();
            _tiposMampara.Clear();

            int wantedFiles = 0;
            var file124 = new List<string>();
            var file35 = new List<string>();
            var fileTechos = new List<string>();
            var idsVistos = new HashSet<int>();
            string directorio = "";
            string fileMamparas = null, fileVidrios = null;

            foreach (string file in archivos)
            {
                FileInfo archivo = new FileInfo(file);
                directorio = archivo.DirectoryName;

                // Los despieces se nombran "N-...txt" (N = id de documento). Un TXT cuyo
                // prefijo NO sea numérico se trata como TABLA DE INFORMACIÓN DEL PROYECTO
                // (contiene el código y el nombre del proyecto), no como un despiece.
                int idDocumento;
                if (!int.TryParse(archivo.Name.Split('-')[0].Trim(), out idDocumento))
                {
                    LeerInfoProyecto(file);
                    continue;
                }

                // TECHOS (6-): no es un documento nuevo, son FILAS MAS del despiece de
                // perfiles (mismo formato de columnas). Se apartan aqui y se inyectan al
                // procesar el 1-.
                //
                // OJO, y por esto no se puede hacer de la forma obvia: 'wantedFiles' NO es
                // un contador, es una SUMA que el resto del codigo lee como CODIGO DE
                // COMBINACION (1+2=3, 1+4=5, 2+4=6, 1+2+4=7) para saber que archivos
                // cargaste. Meter aqui un id nuevo -o repetir uno existente- corrompe esos
                // totales y el programa se va por ramas equivocadas SIN dar ningun error.
                if (idDocumento == ID_TECHOS) { fileTechos.Add(file); continue; }

                idsVistos.Add(idDocumento);

                if (idDocumento == 0) { swPMVertical = 1; }
                if (idDocumento == 5) fileMamparas = file;
                if (idDocumento == 2) fileVidrios = file;

                if (idDocumento == 0 || idDocumento == 1 || idDocumento == 2 || idDocumento == 4)
                {
                    wantedFiles += idDocumento;
                    file124.Add(file);
                }
                else
                {
                    file35.Add(file);
                }
            }

            _file124 = file124.OrderByDescending(x => x).ToList();
            _file35 = file35;
            _wantedFiles = wantedFiles;

            // Las lamas se enganchan al despiece de perfiles: al 1- y, si esa tabla no se
            // cargo, al 0-. Si no hay ninguno de los dos no hay donde meterlas y se quedan
            // fuera; 'TechosIgnorados' lo deja dicho por si se quiere avisar por pantalla.
            _fileTechos = fileTechos;
            _idPerfilTechos = idsVistos.Contains(1) ? 1 : (idsVistos.Contains(0) ? 0 : -1);
            TechosIgnorados = fileTechos.Count > 0 && _idPerfilTechos < 0;
            DirectorioActual = directorio;
            DatosCargados = true;

            // Mapa ubicación → sistema. El TXT de mamparas (5-) trae las columnas
            // Codigo(0) | Tipo(1) | Area(2) | Ubicacion(3): el TIPO es el nombre de familia y es
            // el ÚNICO que distingue monovidrio de doble vidrio (el código no: ITA01 e ITA05
            // comparten prefijo).
            //
            // Respaldo: el TXT de vidrios (2-) es Codigo(0) … Ubicación(7) y NO trae el tipo, así
            // que si falta el 5- se cae al código de la pieza y la distinción MV/DV se pierde.
            if (fileMamparas != null) LeerUbicaciones(fileMamparas, 1, 3);
            else if (fileVidrios != null) LeerUbicaciones(fileVidrios, 0, 7);
        }

        /// <summary>
        /// Lee de un TXT la pareja (sistema, ubicación) de cada línea y arma el mapa que dice qué
        /// mampara —y por tanto qué sistema— hay en cada ubicación. En el TXT de mamparas el
        /// sistema es el TIPO (nombre de familia, columna 1), no el código.
        /// </summary>
        private void LeerUbicaciones(string ruta, int colSistema, int colUbicacion)
        {
            try
            {
                var dto = new AnalisisDatosDto();
                foreach (object[] fila in dto.readFileTxt(ruta, dto.ValidationSplit(ruta)))
                {
                    if (fila.Length <= colUbicacion || fila.Length <= colSistema) continue;
                    string sis = Limpiar(fila[colSistema]);
                    string ubi = Limpiar(fila[colUbicacion]);
                    if (sis.Length == 0 || ubi.Length == 0) continue;

                    if (!_sistemaDeUbicacion.ContainsKey(ubi)) _sistemaDeUbicacion[ubi] = sis;
                    if (!_tiposMampara.Contains(sis)) _tiposMampara.Add(sis);
                }
            }
            catch { /* TXT ilegible: se sigue sin mapa (se usará el prefijo del propio código) */ }
        }

        private static string Limpiar(object valor)
        {
            return Convert.ToString(valor).Replace("\"", "").Trim();
        }

        /// <summary>
        /// Sistemas presentes en el proyecto, para el diálogo "Tipo de vidrio". Los que no estén
        /// dados de alta salen igual, marcados como NO configurados, para que se note el olvido
        /// en vez de pasar en silencio.
        /// </summary>
        public List<SistemaVidrio> SistemasDelProyecto(VidrioResolver resolver)
        {
            var lista = new List<SistemaVidrio>();
            if (resolver == null) return lista;

            foreach (string tipo in _tiposMampara)
            {
                SistemaVidrio s = resolver.SistemaDeCodigo(tipo);
                string prefijo = s != null ? s.Prefijo : PrefijoLibre(tipo);
                if (prefijo.Length == 0) continue;
                if (lista.Exists(x => string.Equals(x.Prefijo, prefijo, StringComparison.OrdinalIgnoreCase)))
                    continue;

                lista.Add(s ?? new SistemaVidrio
                {
                    Prefijo = prefijo,
                    Descripcion = "",
                    IdTipoEstandar = 0,
                    TipoEstandar = "",
                    Configurado = false
                });
            }

            lista.Sort((a, b) => string.Compare(a.Prefijo, b.Prefijo, StringComparison.OrdinalIgnoreCase));
            return lista;
        }

        // Sistema no configurado: se muestra por las 2 primeras letras del tipo (MV, DV, IT…).
        private static string PrefijoLibre(string texto)
        {
            texto = (texto ?? "").Trim();
            return texto.Length >= 2 ? texto.Substring(0, 2).ToUpperInvariant() : texto.ToUpperInvariant();
        }

        /// <summary>
        /// Lee un TXT de información del proyecto y rellena <see cref="ProyectoCodigo"/> /
        /// <see cref="ProyectoNombre"/>. Acepta varios formatos para tolerar lo que la
        /// empresa decida usar (el formato aún no está cerrado):
        ///   • Una línea "CODIGO;NOMBRE" (separadores admitidos: ; , | o tabulador).
        ///   • Dos líneas: la 1ª = código, la 2ª = nombre.
        ///   • Una sola línea sin separador: se toma como nombre.
        /// También reconoce etiquetas "codigo:" / "nombre:" / "proyecto:" si aparecen.
        /// </summary>
        private void LeerInfoProyecto(string ruta)
        {
            try
            {
                var lineas = File.ReadAllLines(ruta)
                    .Select(l => l.Trim())
                    .Where(l => l.Length > 0)
                    .ToList();
                if (lineas.Count == 0) return;

                // 1) Etiquetas explícitas en cualquier línea (codigo:/nombre:/proyecto:).
                foreach (string l in lineas)
                {
                    int idx = l.IndexOf(':');
                    if (idx <= 0) continue;
                    string etq = l.Substring(0, idx).Trim().ToLowerInvariant();
                    string val = l.Substring(idx + 1).Trim();
                    if (val.Length == 0) continue;
                    if (etq.StartsWith("cod")) ProyectoCodigo = val;
                    else if (etq.StartsWith("nom") || etq.StartsWith("proy")) ProyectoNombre = val;
                    else if (etq.StartsWith("ref")) ProyectoReferencia = val;
                }
                if (!string.IsNullOrEmpty(ProyectoCodigo) || !string.IsNullOrEmpty(ProyectoNombre))
                    return;

                // 2) Una línea con separador → "CODIGO<sep>NOMBRE".
                char[] seps = { ';', '\t', '|', ',' };
                string[] partes = lineas[0].Split(seps, StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length >= 2)
                {
                    ProyectoCodigo = partes[0].Trim();
                    ProyectoNombre = string.Join(" ", partes.Skip(1)).Trim();
                }
                else if (lineas.Count >= 2)
                {
                    // 3) Dos líneas: código y nombre.
                    ProyectoCodigo = lineas[0];
                    ProyectoNombre = lineas[1];
                }
                else
                {
                    // 4) Una sola palabra: la tomamos como nombre.
                    ProyectoNombre = lineas[0];
                }
            }
            catch { /* TXT ilegible: se ignora sin romper la carga */ }
        }

        /// <summary>
        /// Corre el análisis sobre los archivos ya cargados con los valores actuales
        /// de Medida Base y % Desperdicio. Devuelve los resultados por pestaña.
        /// </summary>
        public ResultadoAnalisis Ejecutar(int medidaBase, int desperdicioPct)
        {
            var r = new ResultadoAnalisis();
            if (!DatosCargados) return r;

            Reset();
            // Determinista: cada corrida parte del valor inicial elegido al cargar
            // (en WinForms este global no se reinicia entre recálculos).
            Generals.Global.SwSegmentadoUbi = _swSegmentadoUbiInicial;
            Generals.Global.AnalisisType = "1";

            decimal desperdicio = ((decimal)desperdicioPct / 100m) + 1m;
            var dto = new AnalisisDatosDto();

            // Tipo de vidrio por sistema. Se recargan las reglas en cada corrida (así entra lo
            // que se acabe de configurar) y solo se engancha si hay algo distinto del estándar.
            if (SeleccionVidrio != null && SeleccionVidrio.Count > 0)
            {
                VidrioResolver vidrio = VidrioResolver.Cargar();
                vidrio.FijarUbicaciones(_sistemaDeUbicacion);
                vidrio.FijarSeleccion(SeleccionVidrio);
                if (vidrio.HaySeleccion) dto.Vidrio = vidrio;
            }

            if (_file124 != null && _file124.Count > 0)
                SetDataAll(dto, _wantedFiles, _file124, medidaBase, desperdicio, r);
            if (_file35 != null && _file35.Count > 0)
                SetDataAll(dto, _wantedFiles, _file35, medidaBase, desperdicio, r);

            // Estado final de la segmentación (lo lee el export, igual que WinForms).
            r.SwSegmentadoUbiFinal = Generals.Global.SwSegmentadoUbi;

            // Mejora: Perfilería y Herrajes SIEMPRE en orden alfabético por descripción
            // (en WinForms el orden era no determinista por el GroupBy).
            r.PerfilMetalico = OrdenarPorDescripcion(r.PerfilMetalico);
            r.PerfilMetalicoHerraje = OrdenarPorDescripcion(r.PerfilMetalicoHerraje);
            return r;
        }

        /// <summary>Devuelve una copia de la tabla ordenada A→Z por su columna de descripción.</summary>
        internal static DataTable OrdenarPorDescripcion(DataTable t)
        {
            if (t == null || t.Rows.Count == 0) return t;
            DataColumn col = t.Columns.Cast<DataColumn>()
                .FirstOrDefault(c => c.ColumnName.ToLowerInvariant().Contains("descrip"));
            if (col == null) return t;
            DataView dv = new DataView(t) { Sort = "[" + col.ColumnName + "] ASC" };
            return dv.ToTable();
        }

        /// <summary>Reinicia el estado de tablas entre corridas (equivale a Cancelar(), solo datos).</summary>
        private void Reset()
        {
            dtPuertas = new DataTable();
            dtVidrio = new DataTable();
            dtTubos = new DataTable();
            dtPerfil = new DataTable();
            dtPerfilCalculate = new DataTable();
            dtPerfilHRCalculate = new DataTable();
            dtPerfilOfVidrioPanel = new DataTable();
            dtPerfilOfTubos = new DataTable();
        }

        // Port fiel de FrmAnalisisDatos.SetDataAll (sin las líneas de UI/columnas).
        private void SetDataAll(AnalisisDatosDto dto, int wantedFiles, List<string> Filenames,
                                int medidabase, decimal Desperdicio, ResultadoAnalisis r)
        {
            int pageinitial = 0;
            bool perfilandvidrios = false;
            List<int> UseTab = new List<int>();

            foreach (String file in Filenames)
            {
                bool swmergePM = true;
                FileInfo Archivo = new FileInfo(file);

                int idDocumento = int.Parse(Archivo.Name.ToString().Split('-')[0].Trim());

                UseTab.Add(idDocumento);
                pageinitial = idDocumento < pageinitial ? idDocumento : pageinitial;

                List<Object[]> listData = new List<Object[]>();
                List<String> listColumns = new List<String>();

                listColumns = dto.setCreateColumns(idDocumento);
                listData = dto.readFileTxt(file, dto.ValidationSplit(file));

                // Las lamas de techo se suman aqui, como filas mas de este despiece: mismo
                // esquema de columnas y misma tuberia de calculo. Salen en Perfileria
                // distinguidas por su ubicacion (T1, T2...), sin duplicar el documento y sin
                // tocar 'wantedFiles'. Un 6- vacio no aporta filas y no molesta.
                if (idDocumento == _idPerfilTechos && _fileTechos != null)
                    foreach (string fileTecho in _fileTechos)
                        listData.AddRange(dto.readFileTxt(fileTecho, dto.ValidationSplit(fileTecho)));

                DataTable dtResul = new DataTable();
                DataTable dtcalculate = new DataTable();

                dtResul = dto.showTab(idDocumento, listColumns, listData);
                dtPuertas = idDocumento == 3 ? dtResul : dtPuertas;

                if (idDocumento == 4)
                {
                    if (wantedFiles != 4) { swmergePM = false; dtTubos = dtResul; }

                    DataTable dtresulVP = new DataTable();
                    dtPerfilOfTubos = dto.CalculateTab(1, dtResul, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                    dtresulVP = dtPerfilOfTubos.Copy();
                    dtresulVP.Merge(dtPerfilCalculate);

                    r.PerfilMetalico = dtresulVP;

                    if (swmergePM)
                    {
                        DataTable dtresulPMHerraje = new DataTable();
                        Generals.Global.SwSegmentadoUbi = "0";
                        dtresulPMHerraje = dto.CalculateTab(8, dtResul, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);

                        r.PerfilMetalicoHerraje = dtresulPMHerraje;
                    }
                }

                if (idDocumento == 2)
                {
                    if ((wantedFiles != 2 && wantedFiles != 6) || swPMVertical == 1) { swmergePM = false; dtVidrio = dtResul; }

                    DataTable dtresulVP = new DataTable();
                    dtPerfilOfVidrioPanel = dto.CalculateTab(1, dtResul, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                    dtresulVP = dtPerfilOfVidrioPanel.Copy();
                    dtresulVP.Merge(dtPerfilCalculate);

                    r.PerfilMetalico = dtresulVP;

                    if (swmergePM)
                    {
                        DataTable dtresulPMHerraje = new DataTable();
                        Generals.Global.SwSegmentadoUbi = "0";
                        dtresulPMHerraje = dto.CalculateTab(8, dtResul, dtPuertas, true, medidabase, Desperdicio, wantedFiles == 6 ? false : true, 0);
                        if (wantedFiles == 6)
                            dtresulPMHerraje.Merge(dto.CalculateTab(8, dtTubos, dtPuertas, true, medidabase, Desperdicio, true, 0));

                        r.PerfilMetalicoHerraje = dtresulPMHerraje;
                    }
                }

                if (idDocumento != 4)
                {
                    if (idDocumento == 0) { Generals.Global.SwSegmentadoUbi = "0"; };
                    dtcalculate = dto.CalculateTab(idDocumento, dtResul, dtPuertas, perfilandvidrios, medidabase, Desperdicio, swmergePM, 0);
                    if (idDocumento == 0 && dtPerfilCalculate.Rows.Count > 0) { Generals.Global.SwSegmentadoUbi = "1"; };
                }

                if (idDocumento == 1)
                {
                    dtPerfil = dtResul;
                    dtPerfilCalculate = dtcalculate;

                    if (wantedFiles != 1) { swmergePM = false; }

                    if (swPMVertical != 1)
                    {
                        DataTable dtresulPMHerraje = new DataTable();
                        Generals.Global.SwSegmentadoUbi = (Generals.Global.SwSegmentadoUbi == "1") ? "-1" : "0";
                        dtresulPMHerraje = dto.CalculateTab(8, dtResul, dtPuertas, perfilandvidrios, medidabase, Desperdicio, swmergePM, 0);

                        if (wantedFiles == 7)
                        {
                            swmergePM = (swPMVertical == 1) ? false : true;
                            dtresulPMHerraje = dto.CalculateTab(8, dtVidrio, dtPuertas, true, medidabase, Desperdicio, false, 0);
                            dtresulPMHerraje = dto.CalculateTab(8, dtTubos, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                        }

                        if (wantedFiles == 3)
                        {
                            swmergePM = (swPMVertical == 1) ? false : true;
                            dtresulPMHerraje = dto.CalculateTab(8, dtVidrio, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                        }

                        if (wantedFiles == 5)
                        {
                            swmergePM = (swPMVertical == 1) ? false : true;
                            dtresulPMHerraje = dto.CalculateTab(8, dtTubos, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                        }

                        dtPerfilHRCalculate = dtresulPMHerraje;
                        r.PerfilMetalicoHerraje = dtresulPMHerraje;

                        Generals.Global.SwSegmentadoUbi = (Generals.Global.SwSegmentadoUbi == "-1") ? "1" : "0";
                    }
                }

                if (idDocumento == 3)
                {
                    DataTable dtresulPC = dto.CalculateTab(6, dtResul, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                    r.PuertasCantidad = dtresulPC;

                    DataTable dtresulPHerraje = dto.CalculateTab(7, dtResul, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                    r.PuertasHerraje = dtresulPHerraje;
                }

                if (idDocumento == 0)
                {
                    if (wantedFiles == 2 && swPMVertical == 1)
                        swmergePM = false;

                    DataTable dtresulPMHerraje = new DataTable();
                    Generals.Global.SwSegmentadoUbi = (Generals.Global.SwSegmentadoUbi == "1") ? "-1" : "0";

                    dtresulPMHerraje = dto.CalculateTab(8, dtPerfil, dtPuertas, perfilandvidrios, medidabase, Desperdicio, false, 0);
                    dtresulPMHerraje = dto.CalculateTab(8, dtResul, dtPuertas, perfilandvidrios, medidabase, Desperdicio, false, 0);
                    dtresulPMHerraje = dto.CalculateTab(8, dtTubos, dtPuertas, true, medidabase, Desperdicio, false, 0);
                    dtresulPMHerraje = dto.CalculateTab(8, dtVidrio, dtPuertas, true, medidabase, Desperdicio, true, 0);

                    dtPerfil.Merge(dtResul);
                    dtPerfilCalculate.Merge(dtcalculate);

                    r.PMVHerraje = dtresulPMHerraje;
                    r.PerfilMetalicoHerraje = dtresulPMHerraje;

                    Generals.Global.SwSegmentadoUbi = (Generals.Global.SwSegmentadoUbi == "-1") ? "1" : "0";
                }

                SetDataView(dtResul, dtcalculate, idDocumento, r);
            }

            switch (UseTab.First())
            {
                case 1: r.PestanaSugerida = 0; break;  // Perfil Metálico
                case 2: r.PestanaSugerida = 2; break;  // Vidrio y Paneles
                case 3: r.PestanaSugerida = 3; break;  // Puertas
                case 4: r.PestanaSugerida = 6; break;  // Tubos Metálicos
                case 5: r.PestanaSugerida = 7; break;  // Mamparas
            }
        }

        // Port fiel de SetDataView (qué tabla calculada va a cada pestaña).
        private void SetDataView(DataTable dt, DataTable dtcalculate, int index, ResultadoAnalisis r)
        {
            switch (index)
            {
                case 0:
                    // dataFinishPM(dtPerfil, dtPerfilCalculate, ...)
                    r.PerfilMetalico = dtPerfilCalculate;
                    r.PerfilMetalicoRaw = dtPerfil;
                    break;
                case 1:
                    // dataFinishPM(dt, dtcalculate, ...)
                    r.PerfilMetalico = dtcalculate;
                    r.PerfilMetalicoRaw = dt;
                    break;
                case 2:
                    r.VidrioPaneles = dtcalculate;
                    r.VidrioRaw = dt;
                    break;
                case 3:
                    r.Puertas = dtcalculate;
                    r.PuertasRaw = dt;
                    break;
                case 4:
                    // En WinForms el grid de cálculo de Tubos está comentado: no se asigna.
                    r.TubosRaw = dt;
                    break;
                case 5:
                    r.Mamparas = dtcalculate;
                    r.MamparasRaw = dt;
                    break;
            }
        }
    }
}
