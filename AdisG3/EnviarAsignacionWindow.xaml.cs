using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using static AdisG3.CursosEstudiantes;

namespace AdisG3
{
    public partial class EnviarAsignacionWindow : Window
    {
        private AsignacionSemana asignacion;

        private bool esArchivo;
        public string correo { get; set; }
        public int id_cursoSeleccionado { get; set; }
        

        public EnviarAsignacionWindow(AsignacionSemana asignacion, string correo = "", int id_cursoSeleccionado = 0)
        {
            InitializeComponent();
            this.asignacion = asignacion;
            this.correo = correo;
            this.id_cursoSeleccionado= id_cursoSeleccionado;
            DataContext = this.asignacion;
            MessageBox.Show($"Correo {correo}");
            MessageBox.Show($"Id_cursoSeleccionado {id_cursoSeleccionado}");
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si se ha seleccionado un archivo o texto
            if (!string.IsNullOrWhiteSpace(txtArchivoSeleccionado.Text) || !string.IsNullOrWhiteSpace(txtTextoTarea.Text))
            {
                try
                {
                    // Leer el contenido del archivo seleccionado o el texto ingresado
                    string archivoOTexto = esArchivo ? File.ReadAllText(txtArchivoSeleccionado.Text) : txtTextoTarea.Text;

                    // Obtener las claves primarias del profesor, curso y estudiante
                    int idCurso = 1;
                    string estudianteCorreo = "v@gmail.com";

                    // Insertar los datos en la tabla TareasEnviadas
                    string connString = conn_db.GetConnectionString();
                    using (MySqlConnection connection = new MySqlConnection(connString))
                    {
                        connection.Open();

                        string query = "INSERT INTO TareasEnviadas (profesor, curso, estudiante, tarea) " +
                                       "VALUES (@profesor, @curso, @estudiante, @tarea)";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            //command.Parameters.AddWithValue("@profesor", idProfesor);
                            command.Parameters.AddWithValue("@curso", idCurso);
                            command.Parameters.AddWithValue("@estudiante", estudianteCorreo);
                            command.Parameters.AddWithValue("@tarea", archivoOTexto);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Tarea enviada correctamente.");
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo enviar la tarea.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un archivo o ingrese un texto para la tarea.");
            }
        }



        private void BtnCargarArchivo_Click(object sender, RoutedEventArgs e)
        {
            // Abrir el cuadro de diálogo para seleccionar un archivo
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // Obtener el nombre del archivo seleccionado y mostrarlo en el TextBlock
                string archivoSeleccionado = openFileDialog.FileName;
                txtArchivoSeleccionado.Text = Path.GetFileName(archivoSeleccionado);

               
            }
        }
    }
}
