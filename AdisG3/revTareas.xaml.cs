using System.Windows;

namespace AdisG3
{
    public partial class revTareas : Window
    {
        private string nombreEstudiante;
        private string tituloTarea;
        private string tareaTXT;
        private string tareaArchivo;

        public revTareas(string nombreEstudiante, string tituloTarea, string tareaTXT, string tareaArchivo)
        {
            InitializeComponent();

            this.nombreEstudiante = nombreEstudiante;
            this.tituloTarea = tituloTarea;
            this.tareaTXT = tareaTXT;
            this.tareaArchivo = tareaArchivo;

            // Lógica para mostrar los datos en la interfaz
            MostrarDatos();
        }

        private void MostrarDatos()
        {
            // Asignar los valores a los elementos de la interfaz (por ejemplo, TextBoxes)
            txtNombreEstudiante.Text = nombreEstudiante;
            txtTituloTarea.Text = tituloTarea;
            txtTareaTXT.Text = tareaTXT;
            txtTareaArchivo.Text = tareaArchivo;
        }

    }
}
