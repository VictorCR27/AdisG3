using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace AdisG3
{
    public partial class cargarEstudiantes : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public class Estudiante
        {
            public string Nombre { get; set; }
            public string ApellidoPaterno { get; set; }
            public string ApellidoMaterno { get; set; }
            public string Correo { get; set; }
        }
        public ObservableCollection<Estudiante> Estudiantes { get; set; }

        public cargarEstudiantes(int id_profesor = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "")
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            // Inicializa la colección de estudiantes
            Estudiantes = new ObservableCollection<Estudiante>();

            // Lógica para cargar los datos desde la base de datos
            LoadEstudiantesFromDatabase();

            // Asigna la colección de estudiantes al ListView
            estudiantesListView.ItemsSource = Estudiantes;
        }

        private void LoadEstudiantesFromDatabase()
        {
            string connString = conn_db.GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT e.nombre, e.apellido1, e.apellido2, e.correo " +
                               "FROM estudiantesMatriculados em " +
                               "JOIN estudiantes e ON em.id_estudiante = e.id_estudiante " +
                               "WHERE em.id_curso = @id_curso AND em.id_profesor = @id_profesor";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Estudiante estudiante = new Estudiante
                            {
                                Nombre = reader.GetString(0),
                                ApellidoPaterno = reader.GetString(1),
                                ApellidoMaterno = reader.GetString(2),
                                Correo = reader.GetString(3)
                            };

                            Estudiantes.Add(estudiante);
                        }
                    }
                }
            }
        }


        private void Button_Matricular(object sender, RoutedEventArgs e)
        {
            string connString = conn_db.GetConnectionString();

            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Archivos de Excel (*.xlsx)|*.xlsx|Todos los archivos (*.*)|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                // Llamar al método para cargar el archivo de Excel en la base de datos
                CargarExcelEnMySQL(filePath, connString);
            }
        }

        private void CargarExcelEnMySQL(string filePath, string connString)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Open(filePath);
            Excel.Worksheet worksheet = workbook.Sheets[1];
            Excel.Range range = worksheet.UsedRange;

            int estudiantesCargados = 0;
            int estudiantesRepetidos = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        string nombre = ((Excel.Range)range.Cells[row, 1]).Value2.ToString();
                        string apellido1 = ((Excel.Range)range.Cells[row, 2]).Value2.ToString();
                        string apellido2 = ((Excel.Range)range.Cells[row, 3]).Value2.ToString();
                        string correo = ((Excel.Range)range.Cells[row, 4]).Value2.ToString();
                        string password = ((Excel.Range)range.Cells[row, 5]).Value2.ToString();
                        int id_estudiante = -1;

                        // Verificar si el estudiante ya existe en la base de datos
                        if (EstudianteExists(correo))
                        {
                            estudiantesRepetidos++;
                            continue;
                        }

                        // Verificar si el estudiante ya existe en la tabla 'estudiantes' por su correo
                        string selectEstudianteQuery = "SELECT id_estudiante FROM estudiantes WHERE correo = @correo";

                        using (MySqlCommand command = new MySqlCommand(selectEstudianteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@correo", correo);

                            var result = command.ExecuteScalar();
                            if (result != null)
                            {
                                // El estudiante ya existe, obtener su ID
                                id_estudiante = Convert.ToInt32(result);
                            }
                            else
                            {
                                // El estudiante no existe, insertarlo en la tabla 'estudiantes'
                                string insertEstudianteQuery = "INSERT INTO estudiantes (nombre, apellido1, apellido2, correo, password) " +
                                                              "VALUES (@nombre, @apellido1, @apellido2, @correo, @password); " +
                                                              "SELECT LAST_INSERT_ID();";

                                using (MySqlCommand insertCommand = new MySqlCommand(insertEstudianteQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@nombre", nombre);
                                    insertCommand.Parameters.AddWithValue("@apellido1", apellido1);
                                    insertCommand.Parameters.AddWithValue("@apellido2", apellido2);
                                    insertCommand.Parameters.AddWithValue("@correo", correo);
                                    insertCommand.Parameters.AddWithValue("@password", password);

                                    // Ejecutar la consulta y obtener el ID generado
                                    id_estudiante = Convert.ToInt32(insertCommand.ExecuteScalar());
                                }
                            }
                        }

                        // Insertar en estudiantesMatriculados con el ID del estudiante obtenido
                        string insertMatriculadosQuery = "INSERT INTO estudiantesMatriculados (id_estudiante, id_profesor, id_curso) " +
                                                         "VALUES (@id_estudiante, @id_profesor, @id_curso)";

                        using (MySqlCommand command = new MySqlCommand(insertMatriculadosQuery, connection))
                        {
                            command.Parameters.AddWithValue("@id_estudiante", id_estudiante);
                            command.Parameters.AddWithValue("@id_profesor", id_profesor);
                            command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                            command.ExecuteNonQuery();
                        }

                        // Agregar el estudiante a la colección
                        Estudiante estudiante = new Estudiante
                        {
                            Nombre = nombre,
                            ApellidoPaterno = apellido1,
                            ApellidoMaterno = apellido2,
                            Correo = correo
                        };
                        Estudiantes.Add(estudiante);

                        estudiantesCargados++;
                    }

                    connection.Close();
                }

                workbook.Close();
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                if (estudiantesCargados > 0)
                {
                    if (estudiantesRepetidos > 0)
                    {
                        System.Windows.MessageBox.Show($"El archivo de Excel se ha cargado correctamente en la base de datos. {estudiantesCargados} estudiantes cargados, {estudiantesRepetidos} estudiantes repetidos.");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"El archivo de Excel se ha cargado correctamente en la base de datos. {estudiantesCargados} estudiantes cargados.");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("No se han cargado estudiantes nuevos en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al cargar el archivo de Excel: " + ex.Message);
            }
        }

        private bool EstudianteExists(string correo)
        {
            foreach (Estudiante estudiante in Estudiantes)
            {
                if (estudiante.Correo.Equals(correo))
                {
                    return true;
                }
            }
            return false;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor Profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            Profesor.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            matricularManualmente matricularManualmente = new matricularManualmente(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            matricularManualmente.Show();
            this.Close();
        }
    }
}
