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
using static AdisG3.cargarEstudiantes;

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for calificar.xaml
    /// </summary>
    public partial class calificar : Window
    {
        public int id_profesor { get; set; }

        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }
        public calificar(int id_profesor = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "")
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            // Llenar el ComboBox con las 15 opciones de semanas
            for (int semana = 1; semana <= 15; semana++)
            {
                cbox_semana.Items.Add($"Semana {semana}");
            }

            CargarEstudiantesSemana(1);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor Profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            Profesor.Show();
            this.Close();
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            // Obtener el número de semana seleccionado del ComboBox
            if (cbox_semana.SelectedIndex != -1)
            {
                string selectedItem = cbox_semana.SelectedItem.ToString();
                int numeroSemana = int.Parse(selectedItem.Replace("Semana ", ""));

                // Cargar los estudiantes correspondientes a la semana seleccionada en el ListView
                CargarEstudiantesSemana(numeroSemana);
            }
        }

        private void CargarEstudiantesSemana(int numeroSemana)
        {
            // Crear una lista de estudiantes de la semana seleccionada
            List<Estudiante> estudiantesSemana = new List<Estudiante>
            {
            new Estudiante { nombre = "Estudiante 1", titulo = "Título 1", tipo = "Tipo 1", descripcion = "Descripción 1", FechaEntrega = DateTime.Now, valor = 10, nota = 8 },
            new Estudiante { nombre = "Estudiante 2", titulo = "Título 2", tipo = "Tipo 2", descripcion = "Descripción 2", FechaEntrega = DateTime.Now, valor = 8, nota = 6 },
            new Estudiante { nombre = "Estudiante 3", titulo = "Título 3", tipo = "Tipo 3", descripcion = "Descripción 3", FechaEntrega = DateTime.Now, valor = 6, nota = 4 }
            };

            // Asignar la lista de estudiantes al ListView
            lvAsignacionesSemana.ItemsSource = estudiantesSemana;
        }

        public class Estudiante
        {
            public string nombre { get; set; }
            public string titulo { get; set; }
            public string tipo { get; set; }
            public string descripcion { get; set; }
            public DateTime FechaEntrega { get; set; }
            public int valor { get; set; }
            public int nota { get; set; }
        }

        private void lvAsignacionesSemana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
