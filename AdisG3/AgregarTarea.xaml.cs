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
    /// Interaction logic for AgregarTarea.xaml
    /// </summary>
    public partial class AgregarTarea : Window
    {
        public static List<Datos_Tareas> Datos = new List<Datos_Tareas>();
        public AgregarTarea()
        {
            InitializeComponent();
        }

        private void nombre_tarea_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void descripción_tarea_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void categoría_tarea_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button_Add_Click(object sender, RoutedEventArgs e)
        {
            //Si se cumplen las condiciones permite que se agreguen los elementos a la lista y que se muestre en el listbox
            if (txt_categoria_tarea.Text != null && txt_nombre_tarea.Text != null && txt_nombre_tarea.Text != " " &&
            txt_descripcion_tarea.Text != null && txt_descripcion_tarea.Text != " " && txt_nombre_tarea.Text != string.Empty && txt_descripcion_tarea.Text != string.Empty)
            {

                Llenar_Tareas();

                txt_nombre_tarea.Clear();//Vacia el textbox del nombre
                txt_descripcion_tarea.Clear();//Vacia el textbox de la descripción

            }
            else
            {
                MessageBox.Show("Debe llenar todos los espacios.", "¡Falta informacion!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }


        }

        private void Llenar_Tareas()
        {
            try
            {
                //Asigna los valores de los textbox, combobox y select date a variables, para guardarlas en "Datos"
                string nombre = txt_nombre_tarea.Text;
                string categoria = txt_categoria_tarea.Text;
                string descripcion = txt_descripcion_tarea.Text;

                //Guardas las variables en la lista
                Datos.Add(new Datos_Tareas($"{nombre}          ", $"|          {categoria}          |", $"           {descripcion}          |"));

                //Refresca el listbox de tareas
                lb_Tareas.ItemsSource = null;
                lb_Tareas.ItemsSource = Datos;


            }
            catch
            {
                MessageBox.Show("Debe llenar todos los espacios.", "¡Falta informacion!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void lb_Tareas_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void button_Subir_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("¡La tarea se subió con éxito!");
        }

        private void button_Regresar_Click(object sender, RoutedEventArgs e)
        {
            Profesor profesor = new Profesor();
            this.Close();
            profesor.Show();
        }
    }

}
