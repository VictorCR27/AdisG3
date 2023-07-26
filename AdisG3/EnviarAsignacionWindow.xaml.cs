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
        private AsignacionSemana asignacionSemana;
        private int id_estudiante;
        private int id_cursoSeleccionado;

        public EnviarAsignacionWindow(AsignacionSemana asignacionSemana, int id_estudiante, int id_cursoSeleccionado)
        {
            InitializeComponent();
            this.asignacionSemana = asignacionSemana;
            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;

            // Set the DataContext to the current instance of EnviarAsignacionWindow (this)
            DataContext = asignacionSemana;

            MessageBox.Show($"id_estudiante: {id_estudiante}");
            MessageBox.Show($"Id_cursoSeleccionado {id_cursoSeleccionado}");
        }

        // Property to bind to the DataContext in XAML
        public AsignacionSemana Asignacion
        {
            get { return asignacionSemana; }
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            string tareaTexto = txtTareaTexto.Text;
            string tareaArchivo = txtArchivoSeleccionado.Text; 

            string connString = conn_db.GetConnectionString();

            try
            {
                int id_curso = 0;
                int id_profesor = 0;

                // Fetch the id_curso and id_profesor from estudiantesMatriculados
                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    string query = "SELECT id_curso, id_profesor FROM estudiantesMatriculados " +
                                   "WHERE id_estudiante = @id_estudiante";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_estudiante", id_estudiante);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                id_curso = reader.GetInt32("id_curso");
                                id_profesor = reader.GetInt32("id_profesor");
                            }
                        }
                    }
                }

                // Insert the assignment into the TareasEnviadas table
                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    string query = "INSERT INTO TareasEnviadas (profesor, curso, estudiante, tareaTXT, tareaArchivo) " +
                                   "VALUES (@profesor, @curso, @estudiante, @tareaTXT, @tareaArchivo)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@profesor", id_profesor);
                        command.Parameters.AddWithValue("@curso", id_curso);
                        command.Parameters.AddWithValue("@estudiante", id_estudiante);
                        command.Parameters.AddWithValue("@tareaTXT", tareaTexto);
                        command.Parameters.AddWithValue("@tareaArchivo", tareaArchivo);

                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Tarea enviada exitosamente.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar la tarea: " + ex.Message);
            }
        }

        private void BtnAdjuntarArchivo_Click(object sender, RoutedEventArgs e)
        {
            // Show a file dialog to select the file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos PDF e Imágenes|*.pdf;*.jpg;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file path and display it (you can also save the file to a temporary location if needed)
                string filePath = openFileDialog.FileName;
                txtArchivoSeleccionado.Text = filePath;
            }
        }
    }
}
