using System;
using System.Windows;
using MySql.Data.MySqlClient;
using static AdisG3.calificar;

namespace AdisG3
{
    public partial class revTareas : Window
    {
        private string nombreEstudiante;
        private string tituloAsignacion;
        private string tareaTXT;
        private string tareaArchivo;

        public int idAsignacion { get; set; }

        public revTareas(int idAsignacion)
        {
            InitializeComponent();

            // Lógica para obtener y mostrar los datos en la interfaz
            ObtenerYMostrarDatos(idAsignacion);
        }


        private void ObtenerYMostrarDatos(int idAsignacion)
        {
            try
            {
                string connString = conn_db.GetConnectionString();

                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    string query = @"SELECT e.nombre AS NombreEstudiante, asg.titulo, te.TareaTXT, te.TareaArchivo 
                                    FROM TareasEnviadas te 
                                    INNER JOIN estudiantes e ON te.estudiante = e.id_estudiante
                                    INNER JOIN asignacionesSemanas asg ON te.id_asignacionSemana = asg.asignacionesSemanas
                                    WHERE te.id = @idTarea";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idTarea", idAsignacion);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nombreEstudiante = reader.GetString("NombreEstudiante");
                                tituloAsignacion = reader.GetString("titulo");
                                tareaTXT = reader.GetString("TareaTXT");
                                tareaArchivo = reader.GetString("TareaArchivo");

                                // Asignar los valores a los elementos de la interfaz (por ejemplo, TextBoxes)
                                txtNombreEstudiante.Text = nombreEstudiante;
                                txtTituloTarea.Text = tituloAsignacion;
                                txtTareaTXT.Text = tareaTXT;
                                txtTareaArchivo.Text = tareaArchivo;
                                MessageBox.Show($"{idAsignacion}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los datos: " + ex.Message);
            }
        }
    }
}
