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

namespace AdisG3
{
    /// <summary>
    /// Interaction logic for AgregarTarea.xaml
    /// </summary>
    public partial class AgregarTarea : Window
    {
        List<String> Semanas = new List<String>();

        public static List<Datos_Tareas> Datos = new List<Datos_Tareas>();

        public int id_profesor { get; set; }

        public int id_cursoSeleccionado { get; set; }

        public string nombreCursoSeleccionado { get; set; }


        public AgregarTarea(int id_profesor = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "")
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            Semanas.Add("1");
            Semanas.Add("2");
            Semanas.Add("3");
            Semanas.Add("4");
            Semanas.Add("5");
            Semanas.Add("6");
            Semanas.Add("7");
            Semanas.Add("8");
            Semanas.Add("9");
            Semanas.Add("10");
            Semanas.Add("11");
            Semanas.Add("12");
            Semanas.Add("13");
            Semanas.Add("14");
            Semanas.Add("15");

            cbox_semana.ItemsSource = Semanas;

            // Establecer la fecha mínima del DatePicker como la fecha actual
            fecha_entrega.SelectedDate = DateTime.Today;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
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


        private void toggleVisibilidad_Checked(object sender, RoutedEventArgs e)
        {
            CambiarVisibilidadTarea(true); // Llama al método para cambiar visibilidad a 1
        }

        private void toggleVisibilidad_Unchecked(object sender, RoutedEventArgs e)
        {
            CambiarVisibilidadTarea(false); // Llama al método para cambiar visibilidad a 0
        }

        private void CambiarVisibilidadTarea(bool isVisible)
        {
            try
            {
                string connString = conn_db.GetConnectionString();

                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    // Actualiza la visibilidad en la tabla asignacionesSemanas
                    string updateQuery = @"UPDATE asignacionesSemanas SET Visibilidad = @visibilidad WHERE id_asignacionSemana = @idAsignacion";

                    using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@visibilidad", isVisible ? 1 : 0);
                        command.Parameters.AddWithValue("@idAsignacion", id_cursoSeleccionado);

                        command.ExecuteNonQuery();
                    }

                    //MessageBox.Show(isVisible ? "Visibilidad actualizada a 1" : "Visibilidad actualizada a 0");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error al cambiar la visibilidad: " + ex.Message);
            }
        }

        // Boton Añadir
        private void button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (cbox_semana.SelectedItem != null && !string.IsNullOrWhiteSpace(txt_categoria_tarea.Text) &&
                !string.IsNullOrWhiteSpace(txt_nombre_tarea.Text) && !string.IsNullOrWhiteSpace(txt_descripcion_tarea.Text))
            {
                // Obtener los valores de los controles
                int semana = Convert.ToInt32(cbox_semana.SelectedItem);
                string nombreAsignacion = txt_nombre_tarea.Text;
                string descripcion = txt_descripcion_tarea.Text;
                string tipo = txt_categoria_tarea.Text;

                int valor;

                if (!int.TryParse(txt_valor_tarea.Text, out valor) || valor < 0 || valor > 100)
                {
                    MessageBox.Show("El valor debe ser un número válido y estar en el rango de 0 a 100.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Salir del evento sin continuar con la inserción
                }

                if (!fecha_entrega.SelectedDate.HasValue || fecha_entrega.SelectedDate.Value < DateTime.Today)
                {
                    MessageBox.Show("Debe seleccionar una fecha de entrega válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Salir del evento sin continuar con la inserción
                }

                bool visibilidad = toggleVisibilidad.IsChecked ?? false;

                // Realizar la conexión a la base de datos y la inserción de datos
                string connString = conn_db.GetConnectionString();

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connString))
                    {
                        connection.Open();

                        // Obtener el id de asignación del profesor y el id del curso
                        int id_Profesor = id_profesor;
                        int id_Curso = id_cursoSeleccionado;

                        // Crear la consulta SQL INSERT
                        string query = "INSERT INTO asignacionesSemanas (id_profesor, id_curso, titulo, tipo, descripcion, FechaEntrega, valor, semana, Visibilidad) " +
                                       "VALUES (@id_profesor, @id_curso, @titulo, @tipo, @descripcion, @fechaEntrega, @valor, @semana, @visibilidad)";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            // Asignar los valores a los parámetros de la consulta
                            command.Parameters.AddWithValue("@id_profesor", id_Profesor);
                            command.Parameters.AddWithValue("@id_curso", id_Curso);
                            command.Parameters.AddWithValue("@titulo", nombreAsignacion);
                            command.Parameters.AddWithValue("@tipo", tipo);
                            command.Parameters.AddWithValue("@descripcion", descripcion);
                            command.Parameters.AddWithValue("@fechaEntrega", fecha_entrega.SelectedDate.Value);
                            command.Parameters.AddWithValue("@valor", valor);
                            command.Parameters.AddWithValue("@semana", semana);
                            command.Parameters.AddWithValue("@visibilidad", visibilidad);

                            // Ejecutar la consulta
                            command.ExecuteNonQuery();

                            // Mostrar un mensaje de éxito
                            MessageBox.Show("La asignación se ha agregado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Actualizar visibilidad
                            CambiarVisibilidadTarea(visibilidad);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar la asignación: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Limpiar los controles
                txt_nombre_tarea.Clear();
                txt_descripcion_tarea.Clear();
                txt_categoria_tarea.Clear();
                txt_valor_tarea.Clear();
            }
            else
            {
                MessageBox.Show("Debe llenar todos los espacios.", "¡Falta información!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        // Fin Boton Añadir

        private void lb_Tareas_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void button_Subir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Regresar_Click(object sender, RoutedEventArgs e)
        {
            Profesor profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            this.Close();
            profesor.Show();
        }

        private void button_Editar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


        }
    }
}
