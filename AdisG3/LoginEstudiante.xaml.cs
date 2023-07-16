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
            int id_estudiante = 0;

            // Cadena de conexión
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT id_estudiante, correo, password FROM estudiantes WHERE correo = @correo AND password = @password";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@correo", correo);
                    command.Parameters.AddWithValue("@password", password);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id_estudiante = reader.GetInt32("id_estudiante");
                        }
                        else
                        {
                            MessageBox.Show("El usuario y contraseña no coinciden.");
                            return;
                        }
                    }
                }
            }

            inicioEstudiante inicioEstudiante = new inicioEstudiante(id_estudiante);
            inicioEstudiante.Show();
            this.Close();
        }
    }
}