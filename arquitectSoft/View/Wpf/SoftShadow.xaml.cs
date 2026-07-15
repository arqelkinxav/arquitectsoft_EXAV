using System.Windows.Controls;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Sombra pre-renderizada (9-slice) que reemplaza a DropShadowEffect en superficies grandes.
    /// El blur de WPF cuesta por area de pixeles cada frame (lento al maximizar); esto es una
    /// imagen estatica estirada, coste ~0. Uso: colocar detras de la tarjeta con Margin negativo
    /// (para que el halo sobresalga) y Opacity a gusto. IsHitTestVisible=false por defecto.
    /// </summary>
    public partial class SoftShadow : UserControl
    {
        public SoftShadow()
        {
            InitializeComponent();
        }
    }
}
