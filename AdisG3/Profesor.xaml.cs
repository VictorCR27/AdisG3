using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using OfficeOpenXml;

namespace AdisG3
{
    public partial class Profesor : Window
    {
        public int id_profesor { get; set; }

        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }


        public Profesor(int id_profesor = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "")
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            curso.Content = nombreCursoSeleccionado;

            cbox_semana.SelectionChanged += ComboBox_SelectionChanged_1;

            CargarCursosPublicados();
            CargarSemanas();
        }

        private void CargarCursosPublicados()
        {
            string connString = conn_db.GetConnectionString(); // Reemplaza con tu cadena de conexión

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                // Crea la consulta SQL para obtener los cursos publicados por el profesor
                string query = "SELECT DISTINCT c.nombre_curso FROM cursos AS c " +
                               "INNER JOIN asignacionesSemanas AS a ON c.id_curso = a.id_curso " +
                               "WHERE a.id_profesor = @id_profesor";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asigna el valor al parámetro de la consulta
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        List<string> cursos = new List<string>();

                        while (reader.Read())
                        {
                            string curso = reader.GetString("nombre_curso");
                            cursos.Add(curso);
                        }

                        lvAsignacionesSemana.ItemsSource = cursos;
                    }
                }
            }
        }

        private void CargarSemanas()
        {

            string connString = conn_db.GetConnectionString(); // Reemplaza con tu cadena de conexión

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                // Crea la consulta SQL para obtener las semanas
                string query = "SELECT DISTINCT semana FROM asignacionesSemanas WHERE id_profesor = @id_profesor AND id_curso = @id_curso";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asigna los valores a los parámetros de la consulta
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {

                        for (int semana = 1; semana <= 15; semana++)
                        {
                            cbox_semana.Items.Add(semana);
                        }
                    }
                }
            }
        }

        private void CargarAsignacionesSemana(int semana)
        {
            string connString = conn_db.GetConnectionString(); // Reemplaza con tu cadena de conexión

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                // Crea la consulta SQL para obtener las asignaciones de la semana seleccionada
                string query = "SELECT titulo, tipo, descripcion, FechaEntrega, valor FROM asignacionesSemanas " +
                               "WHERE id_profesor = @id_profesor AND id_curso = @id_curso AND semana = @semana";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Asigna los valores a los parámetros de la consulta
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@semana", semana);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        lvAsignacionesSemana.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (cbox_semana.SelectedItem != null)
            {
                int semana = Convert.ToInt32(cbox_semana.SelectedItem);
                CargarAsignacionesSemana(semana);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void Button_Matricula(object sender, RoutedEventArgs e)
        {
            string connString = conn_db.GetConnectionString();

            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Archivos de Excel (*.xlsx)|*.xlsx|Todos los archivos (*.*)|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                // Llamar al método para cargar el archivo de Excel en la base de datos
                CargarExcelEnMySQL(filePath);
            }
        }

        private void CargarExcelEnMySQL(string filePath)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Open(filePath);
            Excel.Worksheet worksheet = workbook.Sheets[1];
            Excel.Range range = worksheet.UsedRange;
            string connString = conn_db.GetConnectionString();

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
                        int id_Profesor = id_profesor;
                        int id_curso = id_cursoSeleccionado;


                        string query = $"INSERT INTO estudiantes (nombre, apellido1, apellido2,id_curso ,id_profesor) VALUES (@nombre, @apellido1,@apellido2, @id_curso, @id_profesor)";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@nombre", nombre);
                            command.Parameters.AddWithValue("@apellido1", apellido1);
                            command.Parameters.AddWithValue("@apellido2", apellido2);
                            command.Parameters.AddWithValue("@id_curso", id_curso);
                            command.Parameters.AddWithValue("@id_profesor", id_Profesor);
                            command.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }

                workbook.Close();
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                System.Windows.MessageBox.Show("El archivo de Excel se ha cargado correctamente en la base de datos.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al cargar el archivo de Excel: " + ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AgregarTarea agregar_Tarea = new AgregarTarea(id_profesor, id_cursoSeleccionado);
            agregar_Tarea.Closed += AgregarTarea_Closed; // Suscribir al evento Closed de la ventana AgregarTarea
            this.Close();
            agregar_Tarea.Show();
        }
        private void AgregarTarea_Closed(object sender, EventArgs e)
        {
            // Vuelve a cargar las asignaciones de tareas después de cerrar la ventana AgregarTarea
            CargarAsignacionesSemana(Convert.ToInt32(cbox_semana.SelectedItem));
            lvAsignacionesSemana.Items.Refresh(); // Actualiza la vista de la lista de asignaciones de tareas
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            administradorInicio administradorInicio = new administradorInicio(id_profesor, id_cursoSeleccionado);
            administradorInicio.Show();
            this.Close();
        }

        private void Button_Nota(object sender, RoutedEventArgs e)
        {

        }
    }
}
