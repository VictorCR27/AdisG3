using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using Neodynamic.SDK.ImageDraw;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static AdisG3.cargarEstudiantes;

namespace AdisG3
{
    public partial class calificar : Window
    {
        public int idEstudiante;
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
            // Obtiene una lista de id_estudiante desde la tabla TareasEnviadas
            List<int> idEstudiantes = GetIdEstudiantesFromTareasEnviadas();

            // Carga las tareas enviadas para cada estudiante
            foreach (int idEstudiante in idEstudiantes)
            {
                CargarTareasEnviadas(idEstudiante);
            }
        }

        private List<int> GetIdEstudiantesFromTareasEnviadas()
        {
            List<int> idEstudiantes = new List<int>();

            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT DISTINCT estudiante FROM TareasEnviadas"; // Obtener estudiantes únicos

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idEstudiante = reader.GetInt32(0);
                            idEstudiantes.Add(idEstudiante);
                        }
                    }
                }
            }

            return idEstudiantes;
        }

        private void CargarTareasEnviadas(int idEstudiante)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                string query = "SELECT e.nombre AS estudiante, asg.titulo, asg.tipo, asg.descripcion, asg.FechaEntrega, asg.valor " +
                                "FROM asignacionesSemanas asg " +
                                "JOIN TareasEnviadas te ON asg.asignacionesSemanas = te.id " +
                                "JOIN estudiantes e ON e.id_estudiante = te.estudiante " + // Utiliza la columna estudiante de TareasEnviadas
                                "WHERE te.profesor = @id_profesor AND te.curso = @id_curso AND asg.semana = @semana";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@estudiante", idEstudiante); // Utiliza idEstudiante

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
                                    Nombre = reader.GetString("estudiante"),
                                    Titulo = reader.GetString("titulo"),
                                    Descripcion = reader.GetString("descripcion"),
                                    FechaEntrega = reader.GetDateTime("FechaEntrega"),
                                    Valor = reader.GetDouble("valor"),
                                };

                                tareasEnviadas.Add(tarea);
                            }

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
            string connString = conn_db.GetConnectionString();

            foreach (Tarea tarea in lvTareas.Items)
            {
                //int idAsignacion = ObtenerIdAsignacion(tarea); // Obtener el id de la asignación de la tarea
                //int idEstudiante = ObtenerIdEstudiante(tarea); // Obtener el id del estudiante de la tarea
                double calificacion = tarea.Calificacion; // Obtiene la calificación desde la propiedad de la tarea

                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    string query = "UPDATE TareasEnviadas " +
                                   "SET calificacion = @calificacion " +
                                   "WHERE id = 2 AND estudiante = 3";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        //command.Parameters.AddWithValue("@idAsignacion", idAsignacion);
                        //command.Parameters.AddWithValue("@idEstudiante", idEstudiante);
                        command.Parameters.AddWithValue("@calificacion", calificacion);

                        command.ExecuteNonQuery();
                    }
                }
            }

            // Después de la inserción, puedes actualizar la lista de tareas enviadas si es necesario
            CargarTareasEnviadas(idEstudiante);
        }


        private void lvTareas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbox_semana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_semana.SelectedItem != null)
            {
                foreach (int idEstudiante in GetIdEstudiantesFromTareasEnviadas())
                {
                    CargarTareasEnviadas(idEstudiante);
                }
            }
        }

    }
}
