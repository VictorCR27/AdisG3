using MySql.Data.MySqlClient;
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
        public string nombreCursoSeleccionado { get; set; }
        public int idProfesorSeleccionado { get; set; }

        private List<AsignacionSemana> asignacionesSemana;
        

        public CursosEstudiantes(int id_estudiante = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "",int idProfesorSeleccionado = 0)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
            this.idProfesorSeleccionado = idProfesorSeleccionado;


            curso.Content = nombreCursoSeleccionado;

            MessageBox.Show($"Id del curso seleccionado {id_cursoSeleccionado}");
            MessageBox.Show($"Id del curso id_estudiante {id_estudiante}");
            MessageBox.Show($"Id del id_Profe {idProfesorSeleccionado}");


            // Inicializar la lista de asignaciones de la semana
            asignacionesSemana = new List<AsignacionSemana>();

            // Llenar el ComboBox con las semanas del 1 al 15
            List<int> semanas = Enumerable.Range(1, 15).ToList();
            cbox_semana.ItemsSource = semanas;

            // Agregar el evento de clic al ListView
            lvAsignacionesSemana.MouseDoubleClick += LvAsignacionesSemana_MouseDoubleClick;
        }

        private void LvAsignacionesSemana_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Verificar si se ha seleccionado un elemento
            if (lvAsignacionesSemana.SelectedItem != null)
            {
                // Obtener la asignación seleccionada
                AsignacionSemana asignacionSeleccionada = (AsignacionSemana)lvAsignacionesSemana.SelectedItem;

                // Abrir la ventana para enviar la asignación y pasar la asignación seleccionada y el idProfesorSeleccionado a esa ventana
                EnviarAsignacionWindow enviarAsignacionWindow = new EnviarAsignacionWindow(asignacionSeleccionada, id_estudiante, id_cursoSeleccionado, idProfesorSeleccionado);
                enviarAsignacionWindow.ShowDialog();

                // Refresh the assignments after closing the EnviarAsignacionWindow to show any updates
                CargarAsignacionesSemana((int)cbox_semana.SelectedItem);
            }
        }




        public class AsignacionSemana
        {
            internal object asignacionesSemanas;
            internal object semana;

            public string titulo { get; set; }
            public object Titulo { get; internal set; }
            public string tipo { get; set; }
            public string descripcion { get; set; }
            public DateTime FechaEntrega { get; set; }
            public int valor { get; set; }
        }


        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // Obtener la semana seleccionada del ComboBox
            int semanaSeleccionada = (int)cbox_semana.SelectedItem;

            // Desvincular el ListView de la lista asignacionesSemana temporalmente
            lvAsignacionesSemana.ItemsSource = null;

            // Llenar el ListView con las asignaciones de la semana seleccionada
            CargarAsignacionesSemana(semanaSeleccionada);

            // Vincular nuevamente la lista asignacionesSemana al ListView
            lvAsignacionesSemana.ItemsSource = asignacionesSemana;
        }


        private void CargarAsignacionesSemana(int semana)
        {
            // Limpiar la lista de asignaciones de la semana
            asignacionesSemana.Clear();

            // Realizar la consulta a la base de datos para obtener las asignaciones de la semana seleccionada
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT titulo, tipo, descripcion, FechaEntrega, valor FROM asignacionesSemanas WHERE id_curso = @id_curso AND semana = @semana";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@semana", semana);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AsignacionSemana asignacion = new AsignacionSemana
                            {
                                titulo = reader.GetString("titulo"),
                                tipo = reader.GetString("tipo"),
                                descripcion = reader.GetString("descripcion"),
                                FechaEntrega = reader.GetDateTime("FechaEntrega"),
                                valor = reader.GetInt32("valor")
                            };

                            asignacionesSemana.Add(asignacion);
                        }
                    }
                }
            }

            // Asignar la lista de asignaciones de la semana al ListView
            lvAsignacionesSemana.ItemsSource = asignacionesSemana;
        }

        private void Semanas_Frame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de inicioEstudiante
            inicioEstudiante inicioEstudiante = new inicioEstudiante(id_estudiante, id_cursoSeleccionado);
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
