using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using MySql.Data.MySqlClient;

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for restablecerPassword.xaml
    /// </summary>
    public partial class restablecerPassword : Window
    {
        public restablecerPassword()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            // Abrir ventana de inicio de sesión
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void RestablecerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string destinatario = txtCorreo.Text;

                // Verificar si el campo de texto está vacío
                if (string.IsNullOrEmpty(destinatario))
                {
                    MessageBox.Show("Por favor, ingrese una dirección de correo válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verificar si el correo existe en la base de datos
                string connectionString = conn_db.GetConnectionString();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT id_estudiante FROM estudiantes WHERE correo = @correo";
                    MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
                    selectCommand.Parameters.AddWithValue("@correo", destinatario);
                    int count = Convert.ToInt32(selectCommand.ExecuteScalar());

                    if (count == 0)
                    {
                        MessageBox.Show("El correo ingresado no existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Generar contraseña temporal de 4 dígitos
                    Random random = new Random();
                    int password = random.Next(1000, 9999);

                    // Actualizar la contraseña en la base de datos
                    string updateQuery = "UPDATE estudiantes SET password = @password WHERE correo = @correo";
                    MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@password", password);
                    updateCommand.Parameters.AddWithValue("@correo", destinatario);
                    updateCommand.ExecuteNonQuery();

                    // Configuración del Mensaje
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("visualcr.com");

                    // Especificamos el correo desde el que se enviará el Email y el nombre de la persona que lo envía
                    mail.From = new MailAddress("info@visualcr.com", "Proyecto AdisG3 - ULACIT", Encoding.UTF8);

                    // Aquí ponemos el asunto del correo
                    mail.Subject = "Restablecimiento de Contraseña";

                    // Aquí ponemos el mensaje que incluirá el correo
                    mail.Body = "Su contraseña temporal es: " + password.ToString();

                    // Especificamos a quien enviaremos el Email, no es necesario que sea Gmail, puede ser cualquier otro proveedor
                    mail.To.Add(destinatario);

                    // Configuración del SMTP
                    SmtpServer.Port = 587; // Puerto que utiliza Gmail para sus servicios

                    // Especificamos las credenciales con las que enviaremos el mail
                    SmtpServer.Credentials = new NetworkCredential("info@visualcr.com", "Mariana10&");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);

                    // Correo exitoso
                    MessageBox.Show("Se ha enviado un corro electrónico con la contraseña temporal.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Abrir ventana de inicio de sesión
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar el correo electrónico: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtCorreo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
