﻿using System;
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
using MySql.Data.MySqlClient;

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for LoginEstudiante.xaml
    /// </summary>
    public partial class LoginEstudiante : Window
    {
        public LoginEstudiante()
        {
            InitializeComponent();
        }

        private void EnviarButton_Click(object sender, RoutedEventArgs e)
        {
            string correo = txtCorreo.Text; // Obtener el valor del TextBox de usuario
            string password = txtPassword.Text; // Obtener el valor del TextBox de contraseña

            bool credencialesValidas = CompararCredenciales(correo, password);

            if (credencialesValidas)
            {
                MessageBox.Show("Acceso permitido.");

                // Crear una instancia de la pantalla InicioEstudiante
                inicioEstudiante inicioEstudiante = new inicioEstudiante();

                // Mostrar la pantalla InicioEstudiante
                inicioEstudiante.Show();

                // Cerrar la pantalla actual (LoginEstudiante)
                this.Close();
            }
            else
            {
                MessageBox.Show("Acceso denegado.");
            }

        }

        private bool CompararCredenciales(string correo, string password)
        {
            // Cadena de conexión
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM estudiantesRegistrados WHERE correo = @correo AND password = @password";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@correo", correo);
                    command.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }
    }
}
