using MaterialDesignThemes.Wpf;
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
using static Org.BouncyCastle.Crypto.Digests.SkeinEngine;

namespace AdisG3
{
    public partial class LoginAdmin : Window
    {
        public LoginAdmin()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de inicio de sesión
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            restablecerPassword restablecerPassword = new restablecerPassword();
            restablecerPassword.Show();
            this.Close();
        }

        private void EnviarButton_Click(object sender, RoutedEventArgs e)
        {
            string correo = txtCorreo.Text;
            string password = txtPassword.Password;
            int id_profesor = 0;

            // Verificar si el correo termina con "ulacit.ed.cr"
            if (!correo.EndsWith("ulacit.ed.cr", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("El correo no es válido para iniciar sesión.");
                return;
            }

            // Cadena de conexión
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT id_profesor, correo, password FROM profesores WHERE correo = @correo AND password = @password";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@correo", correo);
                    command.Parameters.AddWithValue("@password", password);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id_profesor = reader.GetInt32("id_profesor");
                        }
                        else
                        {
                            MessageBox.Show("El usuario y contraseña no coinciden.");
                            return;
                        }
                    }
                }
            }

            administradorInicio administradorInicio = new administradorInicio(id_profesor);
            administradorInicio.Show();
            this.Close();
        }

    }
}
