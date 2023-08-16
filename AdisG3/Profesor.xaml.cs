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
using static AdisG3.CursosEstudiantes;
using System.Windows.Input;

namespace AdisG3
{
    public partial class Profesor : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public int id_estudiante { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public int semanaSeleccionada;


        public Profesor(int id_profesor = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "", int id_estudiante = 0)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
            this.id_estudiante = id_estudiante;

            curso.Content = nombreCursoSeleccionado;

            cbox_semana.SelectionChanged += ComboBox_SelectionChanged_1;
            lvAsignacionesSemana.MouseDoubleClick += lvAsignacionesSemana_MouseDoubleClick;

            CargarCursosPublicados();
            CargarSemanas();
        }

        private void CargarCursosPublicados()
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();


                string query = "SELECT DISTINCT c.nombre_curso FROM cursos AS c " +
                               "INNER JOIN asignacionesSemanas AS a ON c.id_curso = a.id_curso " +
                               "WHERE a.id_profesor = @id_profesor";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

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

            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();


                string query = "SELECT DISTINCT semana FROM asignacionesSemanas WHERE id_profesor = @id_profesor AND id_curso = @id_curso";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@id_profesor", id_profesor);
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {

                        for (int semana = 1; semana <= 15; semana++)
                        {
                            cbox_semana.Items.Add($"Semana {semana}");
                        }
                    }
                }
            }
        }

        private void CargarAsignacionesSemana(int semana)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                // Crea la consulta SQL para obtener las asignaciones de la semana seleccionada
                string query = "SELECT titulo, tipo, descripcion, FechaEntrega, valor FROM asignacionesSemanas " +
                               "WHERE id_profesor = @id_profesor AND id_curso = @id_curso AND semana = @semana";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

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
                string selectedWeek = cbox_semana.SelectedItem.ToString();

                if (int.TryParse(selectedWeek.Replace("Semana ", ""), out int semana))
                {
                    CargarAsignacionesSemana(semana);
                }
                else
                {
                    
                }
            }
        }


        private void lvAsignacionesSemana_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvAsignacionesSemana.SelectedItem != null && cbox_semana.SelectedItem != null)
            {
                string selectedWeek = cbox_semana.SelectedItem.ToString();

                if (selectedWeek.StartsWith("Semana "))
                {
                    string weekNumberStr = selectedWeek.Substring(7); // Remover "Semana "

                    if (int.TryParse(weekNumberStr, out int semana))
                    {
                        DataRowView selectedItem = (DataRowView)lvAsignacionesSemana.SelectedItem;

                        int idAsignacion = ObtenerIdAsignacionDesdeTitulo(selectedItem["titulo"].ToString(), semana);

                        editarTarea editarTarea = new editarTarea(
                            id_profesor,
                            id_cursoSeleccionado,
                            nombreCursoSeleccionado,
                            selectedItem["titulo"].ToString(),
                            selectedItem["tipo"].ToString(),
                            selectedItem["descripcion"].ToString(),
                            Convert.ToDateTime(selectedItem["FechaEntrega"]),
                            Convert.ToDouble(selectedItem["valor"]),
                            idAsignacion
                        );

                        editarTarea.Closed += EditarTarea_Closed;
                        editarTarea.Show();
                        this.Close();
                    }
                    else
                    {
                        // Manejo de error: la cadena después de "Semana " no es un número válido
                    }
                }
                else
                {
                    // Manejo de error: la cadena no tiene el formato esperado ("Semana ")
                }
            }
        }


        private int ObtenerIdAsignacionDesdeTitulo(string titulo, int semana)
        {
            int idAsignacion = 0;

            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT asignacionesSemanas FROM asignacionesSemanas " +
               "WHERE id_profesor = @id_profesor AND id_curso = @id_curso AND semana = @semana AND titulo = @titulo";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_profesor", id_profesor);
                    command.Parameters.AddWithValue("@id_curso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@semana", semana);
                    command.Parameters.AddWithValue("@titulo", titulo);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idAsignacion = reader.GetInt32("asignacionesSemanas");
                        }
                    }
                }
            }

                return idAsignacion;
        }



        private void EditarTarea_Closed(object sender, EventArgs e)
        {
            this.Close();

            if (cbox_semana.SelectedItem != null && int.TryParse(cbox_semana.SelectedItem.ToString().Replace("Semana ", ""), out int semana))
            {
                
                CargarAsignacionesSemana(semana);
            }
            else
            {
                // Manejo de error: la cadena seleccionada no es válida para conversión a entero
            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AgregarTarea agregar_Tarea = new AgregarTarea(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            
            agregar_Tarea.Show();
            this.Close();
            agregar_Tarea.Closed += AgregarTarea_Closed;

        }
        private void AgregarTarea_Closed(object sender, EventArgs e)
        {

            CargarAsignacionesSemana(Convert.ToInt32(cbox_semana.SelectedItem));
            lvAsignacionesSemana.Items.Refresh();
            this.Close();
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            administradorInicio administradorInicio = new administradorInicio(id_profesor, id_cursoSeleccionado);
            administradorInicio.Show();
            this.Close();
        }

        private void Button_Nota(object sender, RoutedEventArgs e)
        {
            notasPfs notasPfs = new notasPfs(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            notasPfs.Show();
            this.Close();
        }

        private void Button_estudiantes(object sender, RoutedEventArgs e)
        {
            cargarEstudiantes cargarEstudiantes = new cargarEstudiantes(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            cargarEstudiantes.Show();
            this.Close();
        }

        private void Button_Anuncios(object sender, RoutedEventArgs e)
        {
            cargarAnuncios cargarAnuncios = new cargarAnuncios(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            cargarAnuncios.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            calificar Calificar = new calificar(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            Calificar.Show();
            this.Close();

        }

        private void Button_Asistencia(object sender, RoutedEventArgs e)
        {
            asistencia asistencia = new asistencia(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            asistencia.Show();
            this.Close();
        }

        private void Button_notificaciones(object sender, RoutedEventArgs e)
        {
            notificacionesPfs notificacionesPfs = new notificacionesPfs(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            notificacionesPfs.Show();
            this.Close();
        }

        private void Grupos_click(object sender, RoutedEventArgs e)
        {
            gruposPfs gruposPfs = new gruposPfs(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            gruposPfs.Show();
            this.Close();
        }
    }

}