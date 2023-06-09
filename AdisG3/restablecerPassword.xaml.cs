using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text;
using System.Net.Mail;
using System.Net;

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

                // Configuración del Mensaje
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("visualcr.com");
                // Especificamos el correo desde el que se enviará el Email y el nombre de la persona que lo envía
                mail.From = new MailAddress("info@visualcr.com", "ULACIT - Proyecto AdisG3", Encoding.UTF8);
                // Aquí ponemos el asunto del correo
                mail.Subject = "Prueba de Envío de Correo Restablecer Password";
                // Aquí ponemos el mensaje que incluirá el correo
                mail.Body = "Prueba de Envío de Correo de Gmail desde C#";
                // Especificamos a quien enviaremos el Email, no es necesario que sea Gmail, puede ser cualquier otro proveedor
                mail.To.Add(destinatario);

                // Configuración del SMTP
                SmtpServer.Port = 587; // Puerto que utiliza Gmail para sus servicios
                                       // Especificamos las credenciales con las que enviaremos el mail
                SmtpServer.Credentials = new NetworkCredential("info@visualcr.com", "Mariana10&");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

                //Correo de exito
                MessageBox.Show("Correo electrónico enviado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Abrir ventana de inicio de sesión
                MainWindow MainWindow = new MainWindow();
                MainWindow.Show();
                this.Close();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar el correo electrónico: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
