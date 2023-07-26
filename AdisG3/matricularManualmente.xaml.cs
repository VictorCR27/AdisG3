using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace AdisG3
{
    public partial class matricularManualmente : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }
        public cargarEstudiantes ParentWindow { get; set; }

        public matricularManualmente(int id_profesor = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "")
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            cargarEstudiantes cargarEstudiantes = new cargarEstudiantes(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            cargarEstudiantes.Show();
            this.Close();
        }

        private void btn_matricular_Click(object sender, RoutedEventArgs e)
        {
            string connString = conn_db.GetConnectionString();

            // Obtener los valores de los campos de texto
            string nombre = txt_nombre.Text;
            string apellido1 = txt_apellido1.Text;
            string apellido2 = txt_apellido2.Text;
            string correo = txt_correo.Text;
            string password = txt_password.Text;
            int id_Profesor = id_profesor;
            int id_curso = id_cursoSeleccionado;

            // Validar que todos los campos estén llenos
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido1) ||
                string.IsNullOrWhiteSpace(apellido2) || string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            // Verificar si el estudiante ya existe en la base de datos
            int id_estudiante = GetExistingEstudianteId(correo);

            // Si el estudiante no existe, realizar la inserción en la tabla estudiantes
            if (id_estudiante == -1)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connString))
                    {
                        connection.Open();

                        string insertEstudianteQuery = "INSERT INTO estudiantes (nombre, apellido1, apellido2, correo, password) " +
                                                       "VALUES (@nombre, @apellido1, @apellido2, @correo, @password); " +
                                                       "SELECT LAST_INSERT_ID();";

                        using (MySqlCommand command = new MySqlCommand(insertEstudianteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@nombre", nombre);
                            command.Parameters.AddWithValue("@apellido1", apellido1);
                            command.Parameters.AddWithValue("@apellido2", apellido2);
                            command.Parameters.AddWithValue("@correo", correo);
                            command.Parameters.AddWithValue("@password", password);

                            // Ejecutar la consulta y obtener el ID generado
                            id_estudiante = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al matricular al estudiante: " + ex.Message);
                    return;
                }
            }

            // Verificar si el estudiante ya está matriculado en el curso
            if (IsEstudianteMatriculado(id_estudiante, id_cursoSeleccionado))
            {
                MessageBox.Show($"El estudiante ya está matriculado en el curso seleccionado.");
                return;
            }

            // Insertar en estudiantesMatriculados con el ID del estudiante obtenido
            string insertMatriculadosQuery = "INSERT INTO estudiantesMatriculados (id_estudiante, id_profesor, id_curso) " +
                                             "VALUES (@id_estudiante, @id_profesor, @id_curso)";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(insertMatriculadosQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id_estudiante", id_estudiante);
                        command.Parameters.AddWithValue("@id_profesor", id_profesor);
                        command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                        command.ExecuteNonQuery();
                    }
                }

                // Agregar el estudiante a la colección en la clase cargarEstudiantes
                if (ParentWindow != null)
                {
                    cargarEstudiantes.Estudiante estudiante = new cargarEstudiantes.Estudiante
                    {
                        Nombre = nombre,
                        ApellidoPaterno = apellido1,
                        ApellidoMaterno = apellido2,
                        Correo = correo
                    };
                    ParentWindow.Estudiantes.Add(estudiante);
                }

                MessageBox.Show("Estudiante matriculado exitosamente.");

                // Vaciar los campos de texto
                txt_nombre.Text = string.Empty;
                txt_apellido1.Text = string.Empty;
                txt_apellido2.Text = string.Empty;
                txt_correo.Text = string.Empty;
                txt_password.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al matricular al estudiante: " + ex.Message);
            }
        }


        private bool IsEstudianteMatriculado(int id_estudiante, int id_curso)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM estudiantesMatriculados " +
                               "WHERE id_estudiante = @id_estudiante AND id_curso = @id_curso";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_estudiante", id_estudiante);
                    command.Parameters.AddWithValue("@id_curso", id_curso);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        private int GetExistingEstudianteId(string correo)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT id_estudiante FROM estudiantes WHERE correo = @correo";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@correo", correo);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        // El estudiante ya existe, obtener su ID
                        return Convert.ToInt32(result);
                    }

                    // El estudiante no existe, return -1
                    return -1;
                }
            }
        }


        private void txt_nombre_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void txt_apellido1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void txt_apellido2_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void txt_correo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void txt_password_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
