using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using static AdisG3.cargarEstudiantes;

namespace AdisG3
{
    public partial class asistencia : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public int id_estudiante;

        public int id_curso;

        public ObservableCollection<Estudiante> Estudiantes { get; set; }

        public asistencia(int id_profesor, int id_cursoSeleccionado, string nombreCursoSeleccionado)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            Estudiantes = new ObservableCollection<Estudiante>();

            // Llena el ComboBox con las semanas del 1 al 15
            for (int semana = 1; semana <= 15; semana++)
            {
                cbox_semana.Items.Add(semana);
            }

            // Lógica para cargar los datos desde la base de datos
            LoadEstudiantesFromDatabase();        
        }

        private void LoadEstudiantesFromDatabase()
        {
            string connString = conn_db.GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                // Comentario: Para seleccionar un estudiante específico en función del id_profesor, necesitas ajustar esta consulta.
                string studentQuery = "SELECT id_estudiante FROM estudiantesMatriculados WHERE id_profesor = @id_profesor";
                using (MySqlCommand studentCommand = new MySqlCommand(studentQuery, connection))
                {
                    studentCommand.Parameters.AddWithValue("@id_profesor", id_profesor);
                    using (MySqlDataReader studentReader = studentCommand.ExecuteReader())
                    {
                        if (studentReader.Read())
                        {
                            id_estudiante = studentReader.GetInt32(0);
                        }
                        else
                        {
                            // El estudiante no fue encontrado en la base de datos
                            MessageBox.Show("Estudiante no encontrado en la base de datos.");
                        }
                    }
                }


                string query = "SELECT e.id_estudiante, e.nombre, e.apellido1, e.apellido2 " +
                               "FROM estudiantesMatriculados em " +
                               "JOIN estudiantes e ON em.id_estudiante = e.id_estudiante " +
                               "WHERE em.id_curso = @id_curso AND em.id_profesor = @id_profesor";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Estudiante estudiante = new Estudiante
                            {
                                id_estudiante = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                ApellidoPaterno = reader.GetString(2),
                                ApellidoMaterno = reader.GetString(3),
                            };

                            Estudiantes.Add(estudiante);
                        }
                    }
                }
            }
            StudentListView.DataContext = this; // Asignar el DataContext
        }

        private void cbox_semana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Aquí puedes manejar la lógica para registrar la asistencia en la semana seleccionada
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            profesor.Show();
            this.Close();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
