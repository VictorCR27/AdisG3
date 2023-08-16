using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace AdisG3
{
    public partial class gruposStd : Window
    {
        public int id_estudiante { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public int idProfesorSeleccionado { get; set; }

        private ObservableCollection<string> estudiantes = new ObservableCollection<string>();

        public gruposStd(int id_estudiante, int id_cursoSeleccionado, string nombreCursoSeleccionado, int idProfesorSeleccionado)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
            this.idProfesorSeleccionado = idProfesorSeleccionado;

            CargarEstudiantes();
        }

        private void CargarEstudiantes()
        {
            string connString = conn_db.GetConnectionString();
            string query = @"SELECT e.id_estudiante, 
                                   e.nombre AS nombre_estudiante, 
                                   e.apellido1 AS apellido1_estudiante, 
                                   e.apellido2 AS apellido2_estudiante,
                                   g.grupo AS nombre_grupo
                            FROM estudiantes e
                            JOIN estudiantesMatriculados em ON e.id_estudiante = em.id_estudiante
                            JOIN grupos g ON em.id_curso = g.id_curso AND e.id_estudiante = g.id_estudiante
                            WHERE em.id_curso = @id_cursoSeleccionado AND g.grupo = (SELECT grupo FROM grupos WHERE id_curso = @id_cursoSeleccionado AND id_estudiante = @id_estudiante);
                            ";

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id_cursoSeleccionado", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@id_estudiante", id_estudiante);

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string nombreEstudiante = $"{reader["nombre_estudiante"]} {reader["apellido1_estudiante"]} {reader["apellido2_estudiante"]}";
                        string nombreGrupo = $"{reader["nombre_grupo"]}";

                        estudiantes.Add($"{nombreEstudiante} - {nombreGrupo}");
                    }

                    reader.Close();

                    lstIntegrantes.ItemsSource = estudiantes;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CursosEstudiantes CursosEstudiantes = new CursosEstudiantes(id_estudiante, id_cursoSeleccionado, nombreCursoSeleccionado, idProfesorSeleccionado);
            CursosEstudiantes.Show();
            this.Close();
        }
    }
}
