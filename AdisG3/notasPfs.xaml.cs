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
using System.Windows.Shapes;
using static AdisG3.CursosEstudiantes;

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for notasPfs.xaml
    /// </summary>
    public partial class notasPfs : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public int id_estudiante { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        private List<AsignacionSemana> tareasEnviadas = new List<AsignacionSemana>();
        public notasPfs(int id_profesor = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "", int id_estudiante = 0)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
            this.id_profesor = id_profesor;

            List<int> semanas = Enumerable.Range(1, 15).ToList();
            cbox_semana.ItemsSource = semanas;

        }


        private void CargarTareasEnviadas(int semana)
        {
            // Limpiar la lista de tareas enviadas
            tareasEnviadas.Clear();

            // Realizar la consulta a la base de datos para obtener las tareas enviadas de la semana seleccionada
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = @"SELECT e.nombre AS estudiante, asg.asignacionesSemanas, asg.titulo, asg.tipo, asg.descripcion, asg.FechaEntrega, asg.valor, te.calificacion  
                                FROM asignacionesSemanas asg 
                                JOIN TareasEnviadas te ON asg.asignacionesSemanas = te.id_asignacionSemana
                                JOIN estudiantes e ON e.id_estudiante = te.estudiante
                                WHERE te.profesor = @idProfesor AND te.curso = @idCurso AND asg.semana = @semana";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idProfesor", id_profesor);
                    command.Parameters.AddWithValue("@idCurso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@semana", semana);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AsignacionSemana tareaEnviada = new AsignacionSemana
                            {
                                estudiante = reader.IsDBNull(reader.GetOrdinal("estudiante")) ? string.Empty : reader.GetString("estudiante"),
                                idAsignacion = reader.GetInt32("asignacionesSemanas"),
                                titulo = reader.IsDBNull(reader.GetOrdinal("titulo")) ? string.Empty : reader.GetString("titulo"),
                                tipo = reader.IsDBNull(reader.GetOrdinal("tipo")) ? string.Empty : reader.GetString("tipo"),
                                descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? string.Empty : reader.GetString("descripcion"),
                                FechaEntrega = reader.GetDateTime("FechaEntrega"),
                                valor = reader.GetInt32("valor"),
                                calificacion = reader.IsDBNull(reader.GetOrdinal("calificacion")) ? 0 : reader.GetInt32("calificacion")
                            };

                            tareasEnviadas.Add(tareaEnviada);
                        }

                    }
                }
            }

            // Asignar la lista de tareas enviadas al ListView
            lvAsignacionesSemana.ItemsSource = tareasEnviadas;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor Profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado, id_estudiante);
            Profesor.Show();
            this.Close();
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // Obtener la semana seleccionada del ComboBox
            int semanaSeleccionada = (int)cbox_semana.SelectedItem;

            // Desvincular el ListView de la lista tareasEnviadas temporalmente
            lvAsignacionesSemana.ItemsSource = null;

            // Llenar el ListView con las tareas enviadas de la semana seleccionada
            CargarTareasEnviadas(semanaSeleccionada);
        }

        private void lvAsignacionesSemana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
