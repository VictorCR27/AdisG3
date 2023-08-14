using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using AdisG3;

namespace AdisG3
{
    public partial class reclamosStd : Window
    {
        public int id_estudiante { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }
        public int idProfesorSeleccionado { get; set; }
        public int idAsignacionSeleccionada { get; set; }

        private CursosEstudiantes.AsignacionSemana tareaEnviadaSeleccionada;

        public reclamosStd(CursosEstudiantes.AsignacionSemana tareaEnviadaSeleccionada, int id_estudiante = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "", int idProfesorSeleccionado = 0, int idAsignacionSeleccionada = 0)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
            this.idProfesorSeleccionado = idProfesorSeleccionado;
            this.idAsignacionSeleccionada = idAsignacionSeleccionada;

            if (tareaEnviadaSeleccionada != null)
            {
                this.tareaEnviadaSeleccionada = tareaEnviadaSeleccionada;

            }
        

        }

        private void EnviarReclamo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connString = conn_db.GetConnectionString();

                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    string query = "INSERT INTO reclamos (id_estudiante, id_curso, id_asignacion, reclamo) " +
                                   "VALUES (@id_estudiante, @id_curso, @id_asignacion, @reclamo)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_estudiante", id_estudiante);
                        command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                        command.Parameters.AddWithValue("@id_asignacion", tareaEnviadaSeleccionada.idAsignacion);
                        command.Parameters.AddWithValue("@reclamo", txtReclamo.Text);

                        command.ExecuteNonQuery();

                        MessageBox.Show("Reclamo enviado correctamente.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar el reclamo: " + ex.Message);
            }
        }

        private void txtEstudiante_TextChanged(object sender, TextChangedEventArgs e)
        {
            // No es necesario implementar este método si no realiza ninguna acción.
        }

        private void txtTituloTarea_TextChanged(object sender, TextChangedEventArgs e)
        {
            // No es necesario implementar este método si no realiza ninguna acción.
        }

        private void txtDescripcionTarea_TextChanged(object sender, TextChangedEventArgs e)
        {
            // No es necesario implementar este método si no realiza ninguna acción.
        }

        private void txtReclamo_TextChanged(object sender, TextChangedEventArgs e)
        {
            // No es necesario implementar este método si no realiza ninguna acción.
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
