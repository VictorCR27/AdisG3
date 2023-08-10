using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static AdisG3.CursosEstudiantes;

namespace AdisG3
{
    public partial class notasStd : Window
    {
        public int id_estudiante { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }
        public int idProfesorSeleccionado { get; set; }
        public int idAsignacionSeleccionada { get; set; }

        private List<AsignacionSemana> tareasEnviadas = new List<AsignacionSemana>();

        public notasStd(int id_estudiante = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "", int idProfesorSeleccionado = 0, int idAsignacionSeleccionada = 0)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
            this.idProfesorSeleccionado = idProfesorSeleccionado;
            this.idAsignacionSeleccionada = idAsignacionSeleccionada;

            List<int> semanas = Enumerable.Range(1, 15).ToList();
            cbox_semana.ItemsSource = semanas;

            // Agregar el evento de clic al ListView
            lvAsignacionesSemana.MouseDoubleClick += LvAsignacionesSemana_MouseDoubleClick;
        }

        private void LvAsignacionesSemana_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Verificar si se ha seleccionado un elemento
            if (lvAsignacionesSemana.SelectedItem != null)
            {
                // Obtener la tarea enviada seleccionada
                AsignacionSemana tareaEnviadaSeleccionada = (AsignacionSemana)lvAsignacionesSemana.SelectedItem;

                // Abrir la ventana para ver la tarea enviada
                reclamosStd reclamosStd = new reclamosStd(tareaEnviadaSeleccionada, id_estudiante, id_cursoSeleccionado, nombreCursoSeleccionado, idProfesorSeleccionado, idAsignacionSeleccionada);
                reclamosStd.ShowDialog();
            }
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // Obtener la semana seleccionada del ComboBox
            int semanaSeleccionada = (int)cbox_semana.SelectedItem;

            // Desvincular el ListView de la lista tareasEnviadas temporalmente
            lvAsignacionesSemana.ItemsSource = null;

            // Llenar el ListView con las tareas enviadas de la semana seleccionada
            CargarTareasEnviadas(semanaSeleccionada);

            // Vincular nuevamente la lista tareasEnviadas al ListView
            lvAsignacionesSemana.ItemsSource = tareasEnviadas;
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
                                WHERE te.profesor = @idProfesor AND te.curso = @idCurso AND asg.semana = @semana AND te.estudiante = @id_estudiante";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idProfesor", idProfesorSeleccionado);
                    command.Parameters.AddWithValue("@idCurso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@semana", semana);
                    command.Parameters.AddWithValue("@id_estudiante", id_estudiante);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AsignacionSemana tareaEnviada = new AsignacionSemana
                            {
                                estudiante = reader.GetString("estudiante"),
                                idAsignacion = reader.GetInt32("asignacionesSemanas"),
                                titulo = reader.GetString("titulo"),
                                tipo = reader.GetString("tipo"),
                                descripcion = reader.GetString("descripcion"),
                                FechaEntrega = reader.GetDateTime("FechaEntrega"),
                                valor = reader.GetInt32("valor"),
                                calificacion = reader.GetInt32("calificacion")
                            };

                            tareasEnviadas.Add(tareaEnviada);
                        }
                    }
                }
            }

            // Asignar la lista de tareas enviadas al ListView
            lvAsignacionesSemana.ItemsSource = tareasEnviadas;
        }

        private void lvAsignacionesSemana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CursosEstudiantes CursosEstudiantes = new CursosEstudiantes(id_estudiante, id_cursoSeleccionado, nombreCursoSeleccionado, idProfesorSeleccionado);
            CursosEstudiantes.Show();
            this.Close();
        }
    }
}
