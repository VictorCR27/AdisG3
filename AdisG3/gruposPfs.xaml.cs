using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace AdisG3
{
    public partial class gruposPfs : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        private ObservableCollection<Grupo> grupos = new ObservableCollection<Grupo>();

        private Dictionary<string, string> estudiantesGrupos = new Dictionary<string, string>();


        public gruposPfs(int id_profesor, int id_cursoSeleccionado, string nombreCursoSeleccionado)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            lstIntegrantes.ItemsSource = grupos;

            CargarEstudiantes();

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

        private void CargarEstudiantes()
        {
            cmbEstudiantes.Items.Clear();

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
                    List<string> integrantes = new List<string> { selectedEstudiante };
                    grupos.Add(new Grupo { NombreGrupo = grupo, Integrantes = string.Join(", ", integrantes) });
                }
            }
        }


        private void InsertarGrupoEnBaseDeDatos(string nombreGrupo)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    foreach (Grupo grupo in grupos)
                    {
                        string insertGrupoQuery = "INSERT INTO grupos (nombre, grupo, id_curso, id_profesor) VALUES (@nombre, @grupo, @id_curso, @id_profesor)";

                        MySqlCommand command = new MySqlCommand(insertGrupoQuery, connection);
                        command.Parameters.AddWithValue("@nombre", grupo.Integrantes);
                        command.Parameters.AddWithValue("@grupo", nombreGrupo);
                        command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                        command.Parameters.AddWithValue("@id_profesor", id_profesor);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Grupo {nombreGrupo} insertado en la base de datos con {grupo.Integrantes}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void ObtenerGrupoDesdeTextBox()
        {
            string nombreGrupo = ObtenerGrupoDelEstudiante(txtNombreGrupo.Text);
            if (!string.IsNullOrEmpty(nombreGrupo))
            {
                List<string> integrantes = new List<string>();

                string selectedEstudiante = cmbEstudiantes.SelectedItem as string;
                if (!string.IsNullOrEmpty(selectedEstudiante))
                {
                    integrantes.Add(selectedEstudiante);
                }

                if (!string.IsNullOrEmpty(nombreGrupo) && integrantes.Count > 0)
                {
                    grupos.Add(new Grupo { NombreGrupo = nombreGrupo, Integrantes = string.Join(", ", integrantes) });
                }
            }
        }

        private string ObtenerGrupoDelEstudiante(string estudiante)
        {
            if (estudiantesGrupos.ContainsKey(estudiante))
            {
                return estudiantesGrupos[estudiante];
            }
            return string.Empty;
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor Profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            Profesor.Show();
            this.Close();
        }

        private void CrearGrupo_Click(object sender, RoutedEventArgs e)
        {
            crearGrupos crearGrupos = new crearGrupos(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            crearGrupos.Show();
            this.Close();
        }


        private void Crear_Click(object sender, RoutedEventArgs e)
        {
            string nombreGrupo = txtNombreGrupo.Text;
            List<string> integrantes = new List<string>();

            // Aquí debes obtener la lista de integrantes seleccionados desde tu lógica
            // Por ejemplo, desde cmbEstudiantes_SelectionChanged
            string selectedEstudiante = cmbEstudiantes.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedEstudiante))
            {
                integrantes.Add(selectedEstudiante);
            }

            if (!string.IsNullOrEmpty(nombreGrupo) && integrantes.Count > 0)
            {
                string grupoNombreCompleto = $"{selectedEstudiante} - Profesor: {id_profesor} - Curso: {nombreCursoSeleccionado}";
                grupos.Add(new Grupo { NombreGrupo = grupoNombreCompleto, Integrantes = string.Join(", ", integrantes) });
            }
        }


    }
    public class Grupo
    {
        public string NombreGrupo { get; set; }
        public string Integrantes { get; set; }
    }
}
