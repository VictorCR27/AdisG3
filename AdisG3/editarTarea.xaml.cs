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
    /// Interaction logic for editarTarea.xaml
    /// </summary>
    public partial class editarTarea : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public editarTarea(int id_profesor, int id_cursoSeleccionado, string nombreCursoSeleccionado, string titulo, string tipo, string descripcion, DateTime fechaEntrega, double valor)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            // Populate the fields with the provided data
            txt_nombre_tarea.Text = titulo;
            txt_categoria_tarea.Text = tipo;
            txt_descripcion_tarea.Text = descripcion;
            fecha_entrega.SelectedDate = fechaEntrega;
            txt_valor_tarea.Text = valor.ToString();
        }

        private void button_Regresar_Click(object sender, RoutedEventArgs e)
        {
            Profesor profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            profesor.Show();
            this.Close();
            
        }


        private void nombre_tarea_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void categoría_tarea_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void descripción_tarea_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        private void button_Editar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
