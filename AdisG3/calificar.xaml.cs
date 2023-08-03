using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using static AdisG3.CursosEstudiantes;
using static AdisG3.Profesor;

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for calificar.xaml
    /// </summary>
    public partial class calificar : Window
    {
        public AsignacionSemana asignacionSeleccionada;
        public int id_profesor;
        public int id_cursoSeleccionado;
        public string nombreCursoSeleccionado;
        public int semanaSeleccionada;

        public calificar(int id_profesor, int id_cursoSeleccionado, string nombreCursoSeleccionado, int semanaSeleccionada)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
            this.semanaSeleccionada = semanaSeleccionada; // Almacenar la semana seleccionada en la variable de clase

            // Load data for the selected course and display it in the ListView
            LoadDataForSelectedCourse();
        }

        private void LoadDataForSelectedCourse()
        {
            CargarTareasEnviadas();
        }
        private void CargarTareasEnviadas()
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT e.nombre AS estudiante, asg.titulo, asg.tipo, asg.descripcion, asg.FechaEntrega, asg.valor " +
                               "FROM asignacionesSemanas asg " +
                               "JOIN TareasEnviadas te ON asg.asignacionesSemanas = te.id " +
                               "JOIN estudiantes e ON te.estudiante = e.id_estudiante " +
                               "WHERE asg.id_profesor = @id_profesor AND asg.id_curso = @id_curso AND asg.semana = @semana";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@semana", semanaSeleccionada); // Asignar el valor de semanaSeleccionada al parámetro @semana

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        List<Estudiante> tareasEnviadas = new List<Estudiante>();

                        while (reader.Read())
                        {
                            Estudiante estudiante = new Estudiante
                            {
                                nombre = reader.GetString("estudiante"),
                                titulo = reader.GetString("titulo"),
                                tipo = reader.GetString("tipo"),
                                descripcion = reader.GetString("descripcion"),
                                FechaEntrega = reader.GetDateTime("FechaEntrega"),
                                valor = reader.GetInt32("valor"),
                            };

                            tareasEnviadas.Add(estudiante);
                        }

                        // Assign the list of tasks to the ListView
                        lvAsignacionesSemana.ItemsSource = tareasEnviadas;
                    }
                }
            }
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor Profesor = new Profesor(id_profesor, id_cursoSeleccionado);
            Profesor.Show();
            this.Close();
        }
    }
}
