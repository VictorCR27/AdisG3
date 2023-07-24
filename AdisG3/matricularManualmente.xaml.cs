using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for matricularManualmente.xaml
    /// </summary>
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
            string cedula = txt_cedula.Text;
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
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(cedula))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            // Verificar si el estudiante ya existe en la base de datos
            if (EstudianteExists(correo))
            {
                MessageBox.Show($"El estudiante con el correo '{correo}' ya existe en la base de datos.");
                return;
            }

            // Realizar la inserción en la tabla estudiantes
            try
            {

                // Verificar que la cédula tenga exactamente 9 caracteres
                if (cedula.Length != 9)
                {
                    MessageBox.Show("La cédula debe tener exactamente 9 caracteres.");
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    string query = "INSERT INTO estudiantes (cedula,nombre, apellido1, apellido2, id_curso, id_profesor, correo, password) " +
                           "VALUES (@cedula,@nombre, @apellido1, @apellido2, @id_curso, @id_profesor, @correo, @password)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@cedula", cedula);
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@apellido1", apellido1);
                        command.Parameters.AddWithValue("@apellido2", apellido2);
                        command.Parameters.AddWithValue("@id_curso", id_curso);
                        command.Parameters.AddWithValue("@id_profesor", id_Profesor);
                        command.Parameters.AddWithValue("@correo", correo);
                        command.Parameters.AddWithValue("@password", password);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Estudiante matriculado exitosamente.");

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

                    // Vaciar los campos de texto
                    txt_cedula.Text = string.Empty;
                    txt_nombre.Text = string.Empty;
                    txt_apellido1.Text = string.Empty;
                    txt_apellido2.Text = string.Empty;
                    txt_correo.Text = string.Empty;
                    txt_password.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al matricular al estudiante: " + ex.Message);
            }

        }

        private bool EstudianteExists(string correo)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM estudiantes " +
                               "WHERE id_curso = @id_curso AND correo = @correo";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@correo", correo);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        private void txt_nombre_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_apellido1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_apellido2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_correo_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_password_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txt_cedula_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
