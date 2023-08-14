using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for crearGrupos.xaml
    /// </summary>
    public partial class crearGrupos : Window
    {

        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public crearGrupos(int id_profesor, int id_cursoSeleccionado, string nombreCursoSeleccionado)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            string connString = conn_db.GetConnectionString();
            string query = @"SELECT nombre, apellido1, apellido2 FROM estudiantes
                            JOIN estudiantesMatriculados ON estudiantes.id_estudiante = estudiantesMatriculados.id_estudiante
                            WHERE id_curso = @id_cursoSeleccionado AND id_profesor = @id_profesor";

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id_cursoSeleccionado", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string nombreEstudiante = $"{reader["nombre"]} {reader["apellido1"]} {reader["apellido2"]}";
                        cmbEstudiantes.Items.Add(nombreEstudiante);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void cmbEstudiantes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string selectedEstudiante = cmbEstudiantes.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedEstudiante))
            {
                string grupo = ObtenerGrupoDelEstudiante(selectedEstudiante);

                if (!string.IsNullOrEmpty(grupo))
                {
                    InsertarEstudianteEnBaseDeDatos(selectedEstudiante, grupo);
                }
            }
        }

        private void InsertarEstudianteEnBaseDeDatos(string estudiante, string grupo)
        {
            string connString = conn_db.GetConnectionString();

            string insertQuery = "INSERT INTO grupos (nombre, grupo, id_curso, id_profesor) VALUES (@nombre, @grupo, @id_curso, @id_profesor)";

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@nombre", estudiante);
                    command.Parameters.AddWithValue("@grupo", grupo);
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Estudiante {estudiante} insertado en la base de datos con el grupo {grupo}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private string ObtenerGrupoDelEstudiante(string estudiante)
        {
            // Debes implementar la lógica para obtener el grupo del estudiante seleccionado
            // Puede ser desde una lista, base de datos, etc.
            return "Grupo A"; // Cambia esto según tu lógica
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            gruposPfs gruposPfs = new gruposPfs(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            gruposPfs.Show();
            this.Close();
        }

        private void Crear_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
