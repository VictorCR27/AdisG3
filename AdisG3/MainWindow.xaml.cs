using System.Windows;
using MySql.Data.MySqlClient;

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // String de conexion
        string conn = conn_db.GetConnectionString();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnAdministrativo_Click(object sender, RoutedEventArgs e)
        {
            // Redirigir a la página para administrativos
            LoginAdmin loginAdministrativo = new LoginAdmin();
            loginAdministrativo.Show();
            this.Close();
        }

        private void btnEstudiantes_Click(object sender, RoutedEventArgs e)
        {
            // Redirigir a la página para estudiantes
            LoginEstudiante loginEstudiante = new LoginEstudiante();
            loginEstudiante.Show();
            this.Close();
        }


    }
}
