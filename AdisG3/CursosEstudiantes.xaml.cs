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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for CursosEstudiantes.xaml
    /// </summary>
    public partial class CursosEstudiantes : Window
    {

        public int id_estudiante { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string correo { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public CursosEstudiantes(string correo = "",int id_estudiante = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "")
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
            this.correo = correo;

            curso.Content = nombreCursoSeleccionado;
            MessageBox.Show($"Id del estudiante:{id_estudiante}");

        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Semanas_Frame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de inicioEstudiante
            inicioEstudiante inicioEstudiante = new inicioEstudiante(id_estudiante, correo, id_cursoSeleccionado);
            inicioEstudiante.Show();
            this.Close();
        }

        private void Button_Anuncios(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Nota(object sender, RoutedEventArgs e)
        {

        }
    }
}
