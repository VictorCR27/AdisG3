using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using MySql.Data.MySqlClient;
using System.IO;
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
        public int idTarea { get; set; }

        public revTareas(int idAsignacion, int idTarea)
        {
            InitializeComponent();

            // Lógica para obtener y mostrar los datos en la interfaz
            ObtenerYMostrarDatos(idAsignacion);
            ObtenerYMostrarDatos(idTarea);
        }


        private void ObtenerYMostrarDatos(int idTarea)
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
                        command.Parameters.AddWithValue("@idTarea", idTarea);

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
                                //MessageBox.Show($"{idAsignacion}");
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

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tareaArchivo))
            {
                try
                {
                    WebClient webClient = new WebClient();
                    byte[] fileBytes = webClient.DownloadData(tareaArchivo);

                    // Replace "filename.ext" with the actual filename from the URL or path
                    string fileName = "filename.ext";

                    // Save the file to the user's Downloads folder
                    string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\" + fileName;
                    File.WriteAllBytes(downloadsPath, fileBytes);

                    // Open the downloaded file using the default associated program
                    Process.Start(downloadsPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al descargar el archivo: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("No hay archivo adjunto para descargar.");
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}