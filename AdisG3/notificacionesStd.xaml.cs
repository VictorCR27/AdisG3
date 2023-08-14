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
    /// Interaction logic for notificacionesStd.xaml
    /// </summary>
    public partial class notificacionesStd : Window
    {
        public int id_estudiante { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public notificacionesStd(int id_estudiante, int id_cursoSeleccionado, string nombreCursoSeleccionado)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            LoadStudentNotifications();
        }

        private void LoadStudentNotifications()
        {
            string connString = conn_db.GetConnectionString();
            string query = @"SELECT COUNT(*) FROM asistencia  
                             WHERE id_estudiante = @id_estudiante AND estado_estudiante = 'Ausente';";

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_estudiante", id_estudiante);

                    int ausenceCount = Convert.ToInt32(command.ExecuteScalar());

                    if (ausenceCount >= 3)
                    {
                        StudentListView.Items.Add(new { Descripcion = "Perdió el curso por ausencias" });
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStudentNotifications();
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CursosEstudiantes CursosEstudiantes = new CursosEstudiantes(id_estudiante, id_cursoSeleccionado, nombreCursoSeleccionado);
            CursosEstudiantes.Show();
            this.Close();
        }
    }
}
