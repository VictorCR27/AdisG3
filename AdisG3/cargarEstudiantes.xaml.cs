using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for cargarEstudiantes.xaml
    /// </summary>
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

                string query = "SELECT nombre, apellido1, apellido2, correo FROM estudiantes " +
                    "WHERE id_curso = @id_curso AND id_profesor = @id_profesor";
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
                        int id_Profesor = id_profesor;
                        int id_curso = id_cursoSeleccionado;

                        // Verificar si el estudiante ya existe en la base de datos
                        if (EstudianteExists(correo))
                        {
                            estudiantesRepetidos++;
                            continue;
                        }

                        string query = "INSERT INTO estudiantes (nombre, apellido1, apellido2, id_curso, id_profesor, correo, password) " +
                            "VALUES (@nombre, @apellido1, @apellido2, @id_curso, @id_profesor, @correo, @password)";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@nombre", nombre);
                            command.Parameters.AddWithValue("@apellido1", apellido1);
                            command.Parameters.AddWithValue("@apellido2", apellido2);
                            command.Parameters.AddWithValue("@id_curso", id_curso);
                            command.Parameters.AddWithValue("@id_profesor", id_Profesor);
                            command.Parameters.AddWithValue("@correo", correo);
                            command.Parameters.AddWithValue("@password", password);
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
