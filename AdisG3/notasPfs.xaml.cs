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
using static AdisG3.calificar;
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


        private void CargarTareasEnviadas(int idEstudiante, int semanaSeleccionada)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                string query = @"SELECT te.id,
                                        CONCAT(e.nombre,' ',e.apellido1, ' ', e.apellido2) AS estudiante,
                                        e.nombre AS estudiante,
                                        asg.asignacionesSemanas AS idAsignacion,
                                        asg.titulo,
                                        asg.tipo,
                                        asg.descripcion,
                                        asg.FechaEntrega,
                                        asg.valor,
                                        te.calificacion,
                                        (SELECT SUM(te2.calificacion)
                                         FROM TareasEnviadas te2
                                         WHERE te2.estudiante = e.id_estudiante) AS sumaCalificaciones
                                    FROM asignacionesSemanas asg 
                                    JOIN TareasEnviadas te ON asg.asignacionesSemanas = te.id_asignacionSemana
                                    JOIN estudiantes e ON e.id_estudiante = te.estudiante 
                                    WHERE te.profesor = @id_profesor AND te.curso = @id_curso AND asg.semana = @semana
                                    ";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@semana", semanaSeleccionada);
                    command.Parameters.AddWithValue("@idEstudiante", idEstudiante);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        List<Tarea> tareasEnviadas = new List<Tarea>();

                        while (reader.Read())
                        {
                            Tarea tarea = new Tarea
                            {
                                id = reader.IsDBNull(reader.GetOrdinal("id")) ? -1 : reader.GetInt32("id"),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("estudiante")) ? string.Empty : reader.GetString("estudiante"),
                                Titulo = reader.IsDBNull(reader.GetOrdinal("titulo")) ? string.Empty : reader.GetString("titulo"),
                                Tipo = reader.IsDBNull(reader.GetOrdinal("tipo")) ? string.Empty : reader.GetString("tipo"),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? string.Empty : reader.GetString("descripcion"),
                                FechaEntrega = reader.IsDBNull(reader.GetOrdinal("FechaEntrega")) ? DateTime.MinValue : reader.GetDateTime("FechaEntrega"),
                                Valor = reader.IsDBNull(reader.GetOrdinal("valor")) ? 0.0 : reader.GetDouble("valor"),
                                Calificacion = reader.IsDBNull(reader.GetOrdinal("calificacion")) ? -1 : reader.GetInt32("calificacion"),
                                IdAsignacion = reader.IsDBNull(reader.GetOrdinal("idAsignacion")) ? -1 : reader.GetInt32("idAsignacion"),
                                SumaCalificaciones = reader.IsDBNull(reader.GetOrdinal("sumaCalificaciones")) ? 0 : reader.GetInt32("sumaCalificaciones")
                            };

                            tareasEnviadas.Add(tarea);
                        }

                        lvTareas.ItemsSource = tareasEnviadas;
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor Profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado, id_estudiante);
            Profesor.Show();
            this.Close();
        }

        private void cbox_semana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_semana.SelectedItem != null)
            {
                int selectedWeek = (int)cbox_semana.SelectedItem;
                CargarTareasEnviadas(id_estudiante, selectedWeek);
            }
        }

        private void lvTareas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
