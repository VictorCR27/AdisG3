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
                    if (nombreGrupo != value)
                    {
                        nombreGrupo = value;
                        OnPropertyChanged("NombreGrupo");
                    }
                }
            }

            private string integrantes;
            public string Integrantes
            {
                get { return integrantes; }
                set
                {
                    if (integrantes != value)
                    {
                        integrantes = value;
                        OnPropertyChanged("Integrantes");
                    }
                }
            }

            public virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }


        }


        public int id_profesor { get; set; }

        private int id_std;
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
            string query = @"SELECT nombre, apellido1, apellido2, estudiantes.id_estudiante FROM estudiantes
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

                        string id_estudiante = $"{reader["id_estudiante"]}";

                        id_std = Convert.ToInt32(id_estudiante);  // Asignar a la variable id_std

                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void CargarEstudiantes()
        {
            cmbEstudiantes.Items.Clear();

            string connString = conn_db.GetConnectionString();
            string query = @"SELECT nombre, apellido1, apellido2, estudiantes.id_estudiante FROM estudiantes
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
                        string id_estudiante = $"{reader["id_estudiante"]}";
                        cmbEstudiantes.Items.Add(nombreEstudiante);

                        //int id_std = Convert.ToInt32(id_estudiante);

                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error: " + ex.Message);
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
                    //MessageBox.Show("Error: " + ex.Message);
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
                        // Check if the student is already part of the group
                        if (!existingGrupo.Integrantes.Contains(selectedEstudiante))
                        {
                            existingGrupo.Integrantes += $", {selectedEstudiante}";
                        }
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
                        string insertGrupoQuery = "INSERT INTO grupos (nombre, grupo, id_curso, id_profesor, id_estudiante) VALUES (@nombre, @grupo, @id_curso, @id_profesor, @id_estudiante)";

                        MySqlCommand command = new MySqlCommand(insertGrupoQuery, connection);
                        command.Parameters.AddWithValue("@nombre", integrante);
                        command.Parameters.AddWithValue("@grupo", nombreGrupo);
                        command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                        command.Parameters.AddWithValue("@id_profesor", id_profesor);

                        // Obtener el id_estudiante correspondiente al estudiante actual
                        int id_estudiante = ObtenerIdEstudiante(integrante);
                        command.Parameters.AddWithValue("@id_estudiante", id_estudiante);

                        //MessageBox.Show($"estudiante {id_estudiante}");
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            //MessageBox.Show($"Estudiante {integrante} insertado en la base de datos con el grupo {nombreGrupo}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private int ObtenerIdEstudiante(string nombreEstudiante)
        {
            string connString = conn_db.GetConnectionString();
            string query = "SELECT id_estudiante FROM estudiantes WHERE CONCAT(nombre, ' ', apellido1, ' ', apellido2) = @nombreEstudiante";

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nombreEstudiante", nombreEstudiante);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        //MessageBox.Show($"No se encontró el id_estudiante para {nombreEstudiante}");
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error: " + ex.Message);
                    return -1;
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


        private void ActualizarGrupoEstudiante(string estudiante, string nuevoGrupo)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT grupo FROM grupos WHERE nombre = @estudiante";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@estudiante", estudiante);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        string existingGroup = result.ToString();
                        if (existingGroup != nuevoGrupo)
                        {
                            string updateQuery = "UPDATE grupos SET grupo = @nuevoGrupo WHERE nombre = @estudiante";

                            command = new MySqlCommand(updateQuery, connection);
                            command.Parameters.AddWithValue("@nuevoGrupo", nuevoGrupo);
                            command.Parameters.AddWithValue("@estudiante", estudiante);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"Estudiante {estudiante} actualizado en la base de datos con el nuevo grupo {nuevoGrupo}");

                            }
                        }
                        else
                        {
                            MessageBox.Show($"El estudiante {estudiante} ya está en el grupo {nuevoGrupo}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró el grupo del estudiante {estudiante}");
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error: " + ex.Message);
                }
            }
            ActualizarDatosListView();
        }

        private void ActualizarDatosListView()
        {
            grupos.Clear(); // Limpia la colección actual

            // Vuelve a cargar los datos de la base de datos y actualiza la colección
            CargarEstudiantesGrupos();

            // Notifica a la interfaz de usuario que los datos han cambiado
            lstIntegrantes.ItemsSource = null;
            lstIntegrantes.ItemsSource = grupos;

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
                    MessageBox.Show($"El estudiante {selectedEstudiante} ya está en el grupo {estudiantesGrupos[selectedEstudiante]}");

                }
                else
                {
                    Grupo grupoExistente = grupos.FirstOrDefault(g => g.NombreGrupo == nombreGrupo);

                    if (grupoExistente != null)
                    {
                        if (!grupoExistente.Integrantes.Contains(selectedEstudiante) || !estudiantesGrupos.ContainsKey(selectedEstudiante))
                        {
                            // Insert the group data into the database
                            List<string> selectedEstudiantes = new List<string> { selectedEstudiante };

                            InsertarGrupoEnBaseDeDatos(nombreGrupo, selectedEstudiantes);

                            // Update the estudiantesGrupos dictionary
                            estudiantesGrupos[selectedEstudiante] = nombreGrupo;

                            // Update the group in the database
                            ActualizarGrupoEstudiante(selectedEstudiante, nombreGrupo);

                            grupoExistente.Integrantes += $", {selectedEstudiante}";

                            // Notificar a la interfaz de usuario sobre el cambio en los datos
                            grupoExistente.OnPropertyChanged("Integrantes");

                        }
                        else
                        {
                            MessageBox.Show($"El estudiante {selectedEstudiante} ya está en el grupo {estudiantesGrupos[selectedEstudiante]}");

                        }

                    }
                    else
                    {
                        grupos.Add(new Grupo
                        {
                            NombreGrupo = nombreGrupo,
                            Integrantes = selectedEstudiante
                        });

                        // Insert the group data into the database
                        List<string> selectedEstudiantes = new List<string> { selectedEstudiante };
                        InsertarGrupoEnBaseDeDatos(nombreGrupo, selectedEstudiantes);

                        // Update the estudiantesGrupos dictionary
                        estudiantesGrupos[selectedEstudiante] = nombreGrupo;


                        // Notificar a la interfaz de usuario sobre el cambio en los datos
                        //OnPropertyChanged("grupos");
                    }
                }
            }
            ActualizarDatosListView();

        }





        private void CrearAutomatico_Click(object sender, RoutedEventArgs e)
        {
            // Clear any existing groups and student assignments
            grupos.Clear();
            estudiantesGrupos.Clear();

            // Retrieve the list of students
            List<string> studentNames = new List<string>(cmbEstudiantes.Items.Cast<string>());

            // Set the maximum number of students per group
            int maxStudentsPerGroup = 3;

            // Create and populate the groups
            int groupNumber = 1;
            while (studentNames.Count > 0)
            {
                // Create a new group
                List<string> groupStudents = new List<string>();

                for (int i = 0; i < maxStudentsPerGroup && studentNames.Count > 0; i++)
                {
                    string student = studentNames[0];

                    // Check if the student is not already assigned to a group
                    if (!estudiantesGrupos.ContainsKey(student))
                    {
                        groupStudents.Add(student);
                        studentNames.RemoveAt(0);

                        // Assign the student to the current group
                        estudiantesGrupos[student] = $"Grupo{groupNumber}";
                    }
                    else
                    {
                        // Move to the next student
                        studentNames.RemoveAt(0);
                        i--; // Decrement i to ensure we fill the group with the required number of students
                    }
                }

                string groupName = $"Grupo{groupNumber}";

                // Add the group to the ObservableCollection
                grupos.Add(new Grupo { NombreGrupo = groupName, Integrantes = string.Join(", ", groupStudents) });

                // Insert the group into the database
                InsertarGrupoEnBaseDeDatos(groupName, groupStudents);

                groupNumber++;
            }
        }

        private void EliminarGr_Click(object sender, RoutedEventArgs e)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    string truncateQuery = "TRUNCATE TABLE grupos";

                    MySqlCommand command = new MySqlCommand(truncateQuery, connection);
                    command.ExecuteNonQuery();

                    // Clear any existing groups and student assignments
                    grupos.Clear();
                    estudiantesGrupos.Clear();

                    MessageBox.Show("Todos los grupos han sido eliminados.");

                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

    }
    public class Grupo
    {
        public string NombreGrupo { get; set; }
        public string Integrantes { get; set; }
    }
}