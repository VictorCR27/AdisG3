using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Windows;

namespace AdisG3
{
    public partial class gruposPfs : Window
    {
        public class Grupo : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string nombreGrupo;
            public string NombreGrupo
            {
                get { return nombreGrupo; }
                set
                {
                    nombreGrupo = value;
                    OnPropertyChanged("NombreGrupo");
                }
            }

            private string integrantes;
            public string Integrantes
            {
                get { return integrantes; }
                set
                {
                    integrantes = value;
                    OnPropertyChanged("Integrantes");
                }
            }

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
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
            CargarEstudiantesGrupos();

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

        private void CargarEstudiantesGrupos()
        {
            string connString = conn_db.GetConnectionString();
            string query = @"SELECT nombre, grupo FROM grupos WHERE id_curso = @id_cursoSeleccionado AND id_profesor = @id_profesor";

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
                        string nombreGrupo = reader["grupo"].ToString();
                        string nombreEstudiante = reader["nombre"].ToString();

                        // Find the existing group or create a new one
                        Grupo grupo = grupos.FirstOrDefault(g => g.NombreGrupo == nombreGrupo);
                        if (grupo == null)
                        {
                            grupo = new Grupo { NombreGrupo = nombreGrupo, Integrantes = nombreEstudiante };
                            grupos.Add(grupo);
                        }
                        else
                        {
                            grupo.Integrantes += $", {nombreEstudiante}";
                        }
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
                    Grupo existingGrupo = grupos.FirstOrDefault(g => g.NombreGrupo == grupo);
                    if (existingGrupo != null)
                    {
                        existingGrupo.Integrantes += $", {selectedEstudiante}";
                    }
                }
            }
        }



        private void InsertarGrupoEnBaseDeDatos(string nombreGrupo, List<string> integrantes)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    foreach (string integrante in integrantes)
                    {
                        string insertGrupoQuery = "INSERT INTO grupos (nombre, grupo, id_curso, id_profesor) VALUES (@nombre, @grupo, @id_curso, @id_profesor)";

                        MySqlCommand command = new MySqlCommand(insertGrupoQuery, connection);
                        command.Parameters.AddWithValue("@nombre", integrante);
                        command.Parameters.AddWithValue("@grupo", nombreGrupo);
                        command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                        command.Parameters.AddWithValue("@id_profesor", id_profesor);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Estudiante {integrante} insertado en la base de datos con el grupo {nombreGrupo}");
                        }
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

        private void ActualizarGrupoEstudiante(string estudiante, string nuevoGrupo)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    string updateQuery = "UPDATE grupos SET grupo = @nuevoGrupo WHERE nombre = @estudiante";

                    MySqlCommand command = new MySqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@nuevoGrupo", nuevoGrupo);
                    command.Parameters.AddWithValue("@estudiante", estudiante);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Estudiante {estudiante} actualizado en la base de datos con el nuevo grupo {nuevoGrupo}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        private void Crear_Click(object sender, RoutedEventArgs e)
        {
            string nombreGrupo = txtNombreGrupo.Text;
            string selectedEstudiante = cmbEstudiantes.SelectedItem as string;

            if (!string.IsNullOrEmpty(nombreGrupo) && !string.IsNullOrEmpty(selectedEstudiante))
            {
                // Check if the student is already in any group
                if (estudiantesGrupos.ContainsKey(selectedEstudiante))
                {
                    string existingGroup = estudiantesGrupos[selectedEstudiante];

                    if (existingGroup != nombreGrupo)
                    {
                        // Remove the student from their existing group
                        Grupo grupoToUpdate = grupos.FirstOrDefault(g => g.NombreGrupo == existingGroup);
                        if (grupoToUpdate != null)
                        {
                            grupoToUpdate.Integrantes = grupoToUpdate.Integrantes.Replace(selectedEstudiante, "");
                        }

                        // Update the student's group in the database and dictionary
                        ActualizarGrupoEstudiante(selectedEstudiante, nombreGrupo);
                        estudiantesGrupos[selectedEstudiante] = nombreGrupo;

                        // Add the student to the new group in the ObservableCollection
                        Grupo newGroup = grupos.FirstOrDefault(g => g.NombreGrupo == nombreGrupo);
                        if (newGroup != null)
                        {
                            newGroup.Integrantes += $", {selectedEstudiante}";
                        }
                    }
                    else
                    {
                        MessageBox.Show($"El estudiante {selectedEstudiante} ya está en el grupo {nombreGrupo}");
                    }
                }
                else
                {
                    Grupo grupoExistente = grupos.FirstOrDefault(g => g.NombreGrupo == nombreGrupo);

                    if (grupoExistente != null)
                    {
                        grupoExistente.Integrantes += $", {selectedEstudiante}";
                    }
                    else
                    {
                        grupos.Add(new Grupo
                        {
                            NombreGrupo = nombreGrupo,
                            Integrantes = selectedEstudiante
                        });
                    }

                    // Insert the group data into the database
                    List<string> selectedEstudiantes = new List<string> { selectedEstudiante };
                    InsertarGrupoEnBaseDeDatos(nombreGrupo, selectedEstudiantes);

                    // Update the estudiantesGrupos dictionary
                    estudiantesGrupos[selectedEstudiante] = nombreGrupo;
                }
            }
        }



        private void CrearAutomatico_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    public class Grupo
    {
        public string NombreGrupo { get; set; }
        public string Integrantes { get; set; }
    }
}