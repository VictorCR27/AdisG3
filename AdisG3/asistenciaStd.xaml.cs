using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using static AdisG3.cargarEstudiantes;

namespace AdisG3
{
    public partial class asistenciaStd : Window
    {
        public int id_estudiante { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public int idProfesorSeleccionado { get; set; }

        public int selectedWeek;

        public ObservableCollection<Estudiante> Estudiantes { get; set; }


        public asistenciaStd(int id_estudiante, int id_cursoSeleccionado, string nombreCursoSeleccionado, int idProfesorSeleccionado)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
            this.idProfesorSeleccionado = idProfesorSeleccionado;
            // Llena el ComboBox con las semanas del 1 al 15
            for (int semana = 1; semana <= 15; semana++)
            {
                cbox_semana.Items.Add($"Semana {semana}");
            }

            // Maneja el evento SelectionChanged del ComboBox
            cbox_semana.SelectionChanged += Cbox_semana_SelectionChanged;
          
        }

        private void LoadEstudiantesFromDatabase()
        {
            string connString = conn_db.GetConnectionString();
            selectedWeek = (int)cbox_semana.SelectedIndex + 1; // Update selected week

            string query = "SELECT a.semana, a.estado_estudiante " +
                           "FROM estudiantes e " +
                           "JOIN asistencia a ON e.id_estudiante = a.id_estudiante " +
                           "WHERE e.id_estudiante = @id_estudiante AND a.semana = @semana AND a.id_curso = @id_cursoSeleccionado";

            Estudiantes = new ObservableCollection<Estudiante>(); // Initialize the collection

            using (MySqlConnection studentsConnection = new MySqlConnection(connString))
            {
                studentsConnection.Open();

                using (MySqlCommand command = new MySqlCommand(query, studentsConnection))
                {
                    command.Parameters.AddWithValue("@id_estudiante", id_estudiante);
                    command.Parameters.AddWithValue("@semana", selectedWeek);
                    command.Parameters.AddWithValue("@id_cursoSeleccionado", id_cursoSeleccionado);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // Loop through all the records
                        {
                            int semana = reader.GetInt32(0);
                            string estado = $"{reader.GetString(1)}";

                            // Add the student to the collection
                            Estudiantes.Add(new Estudiante
                            {
                                semana = semana,
                                estado_estudiante = estado
                            });
                        }

                        // Assign the collection of students to the ListView
                        StudentListView.ItemsSource = Estudiantes;
                    }
                }
            }
        }

        private void Cbox_semana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_semana.SelectedItem != null)
            {
                selectedWeek = cbox_semana.SelectedIndex + 1; // Update selected week
                LoadEstudiantesFromDatabase();
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
