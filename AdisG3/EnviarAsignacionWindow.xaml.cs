using System.IO;
using System.Windows;
using Microsoft.Win32;
using static AdisG3.CursosEstudiantes;

namespace AdisG3
{
    public partial class EnviarAsignacionWindow : Window
    {
        private AsignacionSemana asignacion;

        public EnviarAsignacionWindow(AsignacionSemana asignacion)
        {
            InitializeComponent();
            this.asignacion = asignacion;
            DataContext = this.asignacion; // Asignar la asignación como el contexto de datos para el enlace de datos
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            // Aquí puedes obtener el texto ingresado por el usuario en el TextBox (txtTexto.Text)
            // y realizar la lógica para enviar la tarea con el texto.
            // Por ejemplo, podrías guardar el texto en una base de datos o en un archivo.

            MessageBox.Show("Tarea enviada correctamente.");
            this.Close();
        }

        private void BtnCargarArchivo_Click(object sender, RoutedEventArgs e)
        {
            // Abrir el cuadro de diálogo para seleccionar un archivo
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // Obtener el nombre del archivo seleccionado y mostrarlo en el TextBlock
                string archivoSeleccionado = openFileDialog.FileName;
                txtArchivoSeleccionado.Text = Path.GetFileName(archivoSeleccionado);

                // Aquí puedes guardar el archivo en la ubicación que desees o enviarlo como parte de la tarea.
                // Por ejemplo, podrías guardar el archivo en una carpeta específica en el servidor.
            }
        }
    }
}
