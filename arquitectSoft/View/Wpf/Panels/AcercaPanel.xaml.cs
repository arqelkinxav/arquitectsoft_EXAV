using System;
using System.Reflection;
using System.Windows.Controls;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>Panel "Acerca de" para el escritorio (piloto del MDI). Solo lectura.</summary>
    public partial class AcercaPanel : UserControl
    {
        public AcercaPanel()
        {
            InitializeComponent();
            LblProducto.Text = Attr<AssemblyProductAttribute>(a => a.Product) != ""
                ? Attr<AssemblyProductAttribute>(a => a.Product)
                : System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            LblVersion.Text = "Versión " + Assembly.GetExecutingAssembly().GetName().Version;
            LblEmpresa.Text = Attr<AssemblyCompanyAttribute>(a => a.Company);
            LblCopyright.Text = Attr<AssemblyCopyrightAttribute>(a => a.Copyright);
            LblDescripcion.Text = Attr<AssemblyDescriptionAttribute>(a => a.Description);
        }

        private static string Attr<T>(Func<T, string> pick) where T : Attribute
        {
            object[] at = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            return at.Length == 0 ? "" : pick((T)at[0]);
        }
    }
}
