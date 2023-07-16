using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace AdisG3
{
    public partial class cargarAnuncios : Window
    {
        public int id_profesor { get; set; }

        public int id_cursoSeleccionado { get; set; }

        public string nombreCursoSeleccionado { get; set; }

        public cargarAnuncios(int id_profesor = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "")
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

        }

        private void btnPublicar_Click(object sender, RoutedEventArgs e)
        {
            string titulo = txtTitulo.Text;
            string descripcion = txtDescripcion.Text;

            if (string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(descripcion))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string connString = conn_db.GetConnectionString(); // Reemplaza con tu cadena de conexión

                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    // Crea la consulta SQL para insertar el anuncio en la tabla
                    string query = "INSERT INTO anuncios (id_profesor, id_curso, titulo, descripcion) " +
                                   "VALUES (@id_profesor, @id_curso, @titulo, @descripcion)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Asigna los valores a los parámetros de la consulta
                        command.Parameters.AddWithValue("@id_profesor", id_profesor);
                        command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                        command.Parameters.AddWithValue("@titulo", titulo);
                        command.Parameters.AddWithValue("@descripcion", descripcion);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Anuncio publicado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo publicar el anuncio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al publicar el anuncio: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtTitulo.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
        }

        private void txtTitulo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void txtDescripcion_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
