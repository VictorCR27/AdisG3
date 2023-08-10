using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using Neodynamic.SDK.ImageDraw;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AdisG3
{
    public partial class calificar : Window
    {
        public int idEstudiante;

        public int IdAsignacion;
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public int semanaSeleccionada { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public calificar(int id_profesor = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "")
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
                CargarTareasEnviadas(idEstudiante, semanaSeleccionada); // Pasa el valor de semanaSeleccionada
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

        private void CargarTareasEnviadas(int idEstudiante, int semanaSeleccionada)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                string query = @"SELECT e.nombre AS estudiante, asg.asignacionesSemanas AS idAsignacion, asg.titulo, asg.tipo, asg.descripcion, asg.FechaEntrega, asg.valor, te.calificacion 
                        FROM asignacionesSemanas asg 
                        JOIN TareasEnviadas te ON asg.asignacionesSemanas = te.id_asignacionSemana
                        JOIN estudiantes e ON e.id_estudiante = te.estudiante 
                        WHERE te.profesor = @id_profesor AND te.curso = @id_curso AND asg.semana = @semana";

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
                                Nombre = reader.GetString("estudiante"),
                                Titulo = reader.GetString("titulo"),
                                Tipo = reader.GetString("tipo"),
                                Descripcion = reader.GetString("descripcion"),
                                FechaEntrega = reader.GetDateTime("FechaEntrega"),
                                Valor = reader.GetDouble("valor"),
                                Calificacion = reader.GetInt32("calificacion"),
                                IdAsignacion = reader.GetInt32("idAsignacion")
                            };

                            tareasEnviadas.Add(tarea);
                        }

                        lvTareas.ItemsSource = tareasEnviadas;
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
            public int IdAsignacion { get; set; }
            public string Tipo { get; internal set; }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            profesor.Show();
            this.Close();
        }

        private int GetIdEstudiante(string nombreEstudiante)
        {
            int idEstudiante = -1; // Valor por defecto en caso de que no se encuentre el estudiante

            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT id_estudiante FROM estudiantes WHERE nombre = @nombreEstudiante";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombreEstudiante", nombreEstudiante);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        idEstudiante = Convert.ToInt32(result);
                    }
                }
            }
            //MessageBox.Show($"{nombreEstudiante}");
            return idEstudiante;
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connString = conn_db.GetConnectionString();
                bool calificacionesAgregadas = false;

                foreach (Tarea tarea in lvTareas.Items)
                {
                    int idAsignacion = tarea.IdAsignacion;
                    int idEstudiante = GetIdEstudiante(tarea.Nombre);

                    // Convierte la calificación de double a int
                    int calificacion = (int)tarea.Calificacion;

                    if (calificacion >= 0) // Verifica si se agregó una calificación válida
                    {
                        using (MySqlConnection connection = new MySqlConnection(connString))
                        {
                            connection.Open();

                            string query = "UPDATE TareasEnviadas " +
                                           "SET calificacion = @calificacion " +
                                           "WHERE id_asignacionSemana = @idAsignacion AND estudiante = @idEstudiante";

                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@idAsignacion", idAsignacion);
                                command.Parameters.AddWithValue("@idEstudiante", idEstudiante);
                                command.Parameters.AddWithValue("@calificacion", calificacion);

                                command.ExecuteNonQuery();
                            }

                            calificacionesAgregadas = true;
                        }

                        // Actualiza la calificación en la lista de tareas
                        tarea.Calificacion = calificacion;
                    }
                }

                if (calificacionesAgregadas)
                {
                    // Mostrar la notificación
                    MessageBox.Show("Estudiantes calificados");

                    // Actualiza la lista de tareas enviadas
                    int semanaSeleccionada = (int)cbox_semana.SelectedItem;
                    foreach (int idEstudiante in GetIdEstudiantesFromTareasEnviadas())
                    {
                        CargarTareasEnviadas(idEstudiante, semanaSeleccionada);
                    }
                }
                else
                {
                    MessageBox.Show("No se agregaron calificaciones.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al calificar estudiantes: " + ex.Message);
            }
        }


        private void lvTareas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvTareas.SelectedItem != null)
            {
                Tarea tareaSeleccionada = (Tarea)lvTareas.SelectedItem;
                int idAsignacion = tareaSeleccionada.IdAsignacion;
                string nombreEstudiante = tareaSeleccionada.Nombre;
                string tituloTarea = tareaSeleccionada.Titulo;

                // Realizar una consulta a la base de datos para obtener tareaTXT y tareaArchivo
                string tareaTXT = ObtenerTareaTXTDesdeBD(idAsignacion);
                string tareaArchivo = ObtenerTareaArchivoDesdeBD(idAsignacion);

                // Crear e mostrar la ventana revTareas con los datos
                revTareas ventanaRevTareas = new revTareas(nombreEstudiante, tituloTarea, tareaTXT, tareaArchivo);
                ventanaRevTareas.ShowDialog();
            }
        }


        private string ObtenerTareaTXTDesdeBD(int idAsignacion)
        {
            string connString = conn_db.GetConnectionString();
            string tareaTXT = string.Empty;

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT tareaTXT FROM TareasEnviadas WHERE id_asignacionSemana = @idAsignacion AND estudiante = @idEstudiante"; 

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idAsignacion", idAsignacion);
                    command.Parameters.AddWithValue("@idEstudiante", idEstudiante);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        tareaTXT = result.ToString();
                    }
                }
            }

            return tareaTXT;
        }

        private string ObtenerTareaArchivoDesdeBD(int idAsignacion)
        {
            string connString = conn_db.GetConnectionString();
            string tareaArchivo = string.Empty;

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT tareaArchivo FROM TareasEnviadas WHERE id_asignacionSemana = @idAsignacion AND estudiante = @idEstudiante"; 

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idAsignacion", idAsignacion);
                    command.Parameters.AddWithValue("@idEstudiante", idEstudiante);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        tareaArchivo = result.ToString();
                    }
                }
            }

            return tareaArchivo;
        }



        private void lvTareas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbox_semana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_semana.SelectedItem != null)
            {
                int semanaSeleccionada = (int)cbox_semana.SelectedItem;

                foreach (int idEstudiante in GetIdEstudiantesFromTareasEnviadas())
                {
                    CargarTareasEnviadas(idEstudiante, semanaSeleccionada); // Pasa el valor de semanaSeleccionada
                }
            }
        }



    }
}
