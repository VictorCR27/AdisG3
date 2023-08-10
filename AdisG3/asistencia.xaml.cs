using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static AdisG3.cargarEstudiantes;

namespace AdisG3
{

        public partial class asistencia : Window
        {

        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public int id_estudiante;

        public int id_curso;

        public int selectedWeek;

        public ObservableCollection<Estudiante> Estudiantes { get; set; }

        public asistencia(int id_profesor, int id_cursoSeleccionado, string nombreCursoSeleccionado)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            Estudiantes = new ObservableCollection<Estudiante>();

            // Llena el ComboBox con las semanas del 1 al 15
            for (int semana = 1; semana <= 15; semana++)
            {
                cbox_semana.Items.Add(semana);
            }

            // Lógica para cargar los datos desde la base de datos
            LoadEstudiantesFromDatabase();
        }

        private void LoadEstudiantesFromDatabase()
        {
            string connString = conn_db.GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                // Comentario: Para seleccionar un estudiante específico en función del id_profesor, necesitas ajustar esta consulta.
                string studentQuery = "SELECT id_estudiante FROM estudiantesMatriculados WHERE id_profesor = @id_profesor";
                using (MySqlCommand studentCommand = new MySqlCommand(studentQuery, connection))
                {
                    studentCommand.Parameters.AddWithValue("@id_profesor", id_profesor);
                    using (MySqlDataReader studentReader = studentCommand.ExecuteReader())
                    {
                        if (studentReader.Read())
                        {
                            id_estudiante = studentReader.GetInt32(0);
                        }
                        else
                        {
                            // El estudiante no fue encontrado en la base de datos
                            MessageBox.Show("Estudiante no encontrado en la base de datos.");
                        }
                    }
                }

                Estudiantes = new ObservableCollection<Estudiante>();

                string query = "SELECT e.id_estudiante, e.nombre, e.apellido1, e.apellido2 " +
                               "FROM estudiantesMatriculados em " +
                               "JOIN estudiantes e ON em.id_estudiante = e.id_estudiante " +
                               "WHERE em.id_curso = @id_curso AND em.id_profesor = @id_profesor";

                using (MySqlConnection studentsConnection = new MySqlConnection(connString))
                {
                    studentsConnection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, studentsConnection))
                    {
                        command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                        command.Parameters.AddWithValue("@id_profesor", id_profesor);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Estudiante estudiante = new Estudiante
                                {
                                    id_estudiante = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    ApellidoPaterno = reader.GetString(2),
                                    ApellidoMaterno = reader.GetString(3),
                                };

                                Estudiantes.Add(estudiante);
                            }
                        }
                    }
                }
                StudentListView.DataContext = this; // Asignar el DataContext
            }
        }



        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor profesor = new Profesor(id_estudiante, id_cursoSeleccionado, nombreCursoSeleccionado);
            profesor.Show();
            this.Close();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {

            if (cbox_semana.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecciona una semana válida.");
                return;
            }

            string connString = conn_db.GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                int selectedWeek = (int)cbox_semana.SelectedItem; // Obtener la semana seleccionada

                foreach (Estudiante estudiante in Estudiantes)
                {
                    // Buscar el ListViewItem correspondiente al estudiante
                    ListViewItem listViewItem = StudentListView
                        .ItemContainerGenerator
                        .ContainerFromItem(estudiante) as ListViewItem;

                    if (listViewItem != null)
                    {
                        // Busca el ComboBox dentro del ListViewItem
                        ComboBox comboBox = FindVisualChild<ComboBox>(listViewItem);
                        if (comboBox != null)
                        {
                            // Obtén el valor de asistencia del ComboBox para cada estudiante
                            string asistencia1 = comboBox.SelectedItem.ToString();
                            string asistencia = asistencia1.Substring(38).Trim();

                            // Verificar si ya existe un registro para la semana seleccionada
                            string existingQuery = "SELECT COUNT(*) FROM asistencia " +
                                                   "WHERE id_profesor = @id_profesor " +
                                                   "AND id_curso = @id_curso " +
                                                   "AND id_estudiante = @id_estudiante " +
                                                   "AND semana = @semana";

                            using (MySqlCommand existingCommand = new MySqlCommand(existingQuery, connection))
                            {
                                existingCommand.Parameters.AddWithValue("@id_profesor", id_profesor);
                                existingCommand.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                                existingCommand.Parameters.AddWithValue("@id_estudiante", estudiante.id_estudiante);
                                existingCommand.Parameters.AddWithValue("@semana", selectedWeek);

                                int existingCount = Convert.ToInt32(existingCommand.ExecuteScalar());

                                if (existingCount > 0)
                                {
                                    // Si ya existe un registro, realizar una actualización en lugar de inserción
                                    string updateQuery = "UPDATE asistencia " +
                                                         "SET estado_estudiante = @estado_estudiante " +
                                                         "WHERE id_profesor = @id_profesor " +
                                                         "AND id_curso = @id_curso " +
                                                         "AND id_estudiante = @id_estudiante " +
                                                         "AND semana = @semana";

                                    using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                                    {
                                        updateCommand.Parameters.AddWithValue("@estado_estudiante", asistencia);
                                        updateCommand.Parameters.AddWithValue("@id_profesor", id_profesor);
                                        updateCommand.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                                        updateCommand.Parameters.AddWithValue("@id_estudiante", estudiante.id_estudiante);
                                        updateCommand.Parameters.AddWithValue("@semana", selectedWeek);
                                        updateCommand.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    //asistencia = asistencia.Substring(39);
                                    // Si no existe un registro, realizar la inserción
                                    string insertQuery = "INSERT INTO asistencia (id_profesor, id_curso, id_estudiante, nombre, estado_estudiante, semana) " +
                                                         "VALUES (@id_profesor, @id_curso, @id_estudiante, @nombre, @estado_estudiante, @semana)";
                                    
                                    using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                                    {
                                        insertCommand.Parameters.AddWithValue("@id_profesor", id_profesor);
                                        insertCommand.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                                        insertCommand.Parameters.AddWithValue("@id_estudiante", estudiante.id_estudiante);
                                        insertCommand.Parameters.AddWithValue("@nombre", estudiante.Nombre);
                                        insertCommand.Parameters.AddWithValue("@estado_estudiante", asistencia);
                                        insertCommand.Parameters.AddWithValue("@semana", selectedWeek);
                                        insertCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("Asistencia guardada correctamente.");
            }
        }


        private ListViewItem FindListViewItemFromEstudiante(ListView listView, Estudiante estudiante)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item.DataContext == estudiante)
                {
                    return item;
                }
            }
            return null;
        }

        private ComboBox FindComboBoxFromListViewItem(ListView listView, int idEstudiante)
        {
            foreach (ListViewItem item in listView.Items)
            {
                Estudiante estudiante = item.DataContext as Estudiante;
                if (estudiante != null && estudiante.id_estudiante == idEstudiante)
                {
                    // Buscar el ComboBox dentro del ListViewItem
                    ComboBox comboBox = FindVisualChild<ComboBox>(item);
                    return comboBox;
                }
            }
            return null;
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T visualChild)
                {
                    return visualChild;
                }
                T childItem = FindVisualChild<T>(child);
                if (childItem != null)
                {
                    return childItem;
                }
            }
            return null;
        }

        private void cbox_semana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
