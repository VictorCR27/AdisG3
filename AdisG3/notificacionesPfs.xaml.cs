using System;
using System.Collections.Generic;
using System.Windows;
using MySql.Data.MySqlClient;

namespace AdisG3
{
    public partial class notificacionesPfs : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public notificacionesPfs(int id_profesor, int id_cursoSeleccionado, string nombreCursoSeleccionado)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            // Cargar los reclamos de los estudiantes al ListView
            CargarReclamos();
        }

        private void CargarReclamos()
        {
            try
            {
                string connString = conn_db.GetConnectionString();
                List<ReclamoEstudiante> reclamos = new List<ReclamoEstudiante>();

                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    string query = "SELECT e.nombre AS Estudiante, c.nombre_curso AS Curso, a.titulo AS Asignacion, r.reclamo " +
                                   "FROM reclamos r " +
                                   "INNER JOIN estudiantes e ON r.id_estudiante = e.id_estudiante " +
                                   "INNER JOIN cursos c ON r.id_curso = c.id_curso " +
                                   "INNER JOIN asignacionesSemanas a ON r.id_asignacion = a.asignacionesSemanas";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string estudiante = reader.GetString("Estudiante");
                            string curso = reader.GetString("Curso");
                            string asignacion = reader.GetString("Asignacion");
                            string reclamo = reader.GetString("reclamo");

                            reclamos.Add(new ReclamoEstudiante
                            {
                                Estudiante = estudiante,
                                Curso = curso,
                                Asignacion = asignacion,
                                Reclamo = reclamo
                            });
                        }
                    }
                }

                // Asignar la lista de reclamos al ListView
                reclamosListView.ItemsSource = reclamos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los reclamos: " + ex.Message);
            }
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class ReclamoEstudiante
    {
        public string Estudiante { get; set; }
        public string Curso { get; set; }
        public string Asignacion { get; set; }
        public string Reclamo { get; set; }
    }
}
