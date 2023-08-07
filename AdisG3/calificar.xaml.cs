using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AdisG3
{
    public partial class calificar : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public int semanaSeleccionada { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public calificar(int id_profesor = 0, int id_cursoSeleccionado = 0, /*int semanaSeleccionada = 0,*/ string nombreCursoSeleccionado = "")
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.semanaSeleccionada = semanaSeleccionada;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            // Llena el ComboBox con las semanas del 1 al 15
            for (int semana = 1; semana <= 15; semana++)
            {
                cbox_semana.Items.Add(semana);
            }

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
                                "WHERE te.profesor = @id_profesor AND te.curso = @id_curso AND asg.semana = @semana";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);

                    if (cbox_semana.SelectedItem != null)
                    {
                        int semanaSeleccionada = (int)cbox_semana.SelectedItem;
                        command.Parameters.AddWithValue("@semana", semanaSeleccionada);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            List<Tarea> tareasEnviadas = new List<Tarea>();

                            while (reader.Read())
                            {
                                Tarea tarea = new Tarea
                                {
                                    //IdAsignacion = reader.GetInt32("id_asignacion"),
                                    Nombre = reader.GetString("estudiante"),
                                    Titulo = reader.GetString("titulo"),
                                    Descripcion = reader.GetString("descripcion"),
                                    FechaEntrega = reader.GetDateTime("FechaEntrega"),
                                    Valor = reader.GetDouble("valor"),
                                };

                                tareasEnviadas.Add(tarea);
                            }

                            // Assign the list of tasks to the ListView
                            lvTareas.ItemsSource = tareasEnviadas;
                        }
                    }
                }
            }
        }

        public class Tarea
        {
            public string Nombre { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public DateTime FechaEntrega { get; set; }
            public double Valor { get; set; }
            public int Calificacion { get; set; }
            public int IdAsignacion { get; internal set; }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            profesor.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lvTareas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbox_semana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_semana.SelectedItem != null)
            {
                CargarTareasEnviadas();
            }
        }
    }
}
