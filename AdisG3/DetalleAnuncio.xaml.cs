using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdisG3
{
    /// <summary>
    /// Lógica de interacción para DetalleAnuncio.xaml
    /// </summary>
    public partial class DetalleAnuncio : Window
    {
        public DetalleAnuncio(string titulo, string descripcion, string nombreProfesor)
        {
            InitializeComponent();

            // Asignar los datos del anuncio a los TextBox
            txtTitulo.Text = titulo;
            txtDescripcion.Text = descripcion;

            // Asignar el nombre del profesor al TextBox correspondiente
            txtNombreProfesor.Text = nombreProfesor;
        }


        private void CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
