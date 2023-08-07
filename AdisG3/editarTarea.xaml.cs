using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace AdisG3
{
    public partial class editarTarea : Window
    {

        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public int idAsignacion;

        public ObservableCollection<int> Semanas { get; set; }


        public editarTarea(int id_profesor, int id_cursoSeleccionado, string nombreCursoSeleccionado, string titulo, string tipo, string descripcion, DateTime fechaEntrega, double valor, int idAsignacion)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            this.idAsignacion = idAsignacion;

            txt_nombre_tarea.Text = titulo;
            txt_categoria_tarea.Text = tipo;
            txt_descripcion_tarea.Text = descripcion;
            fecha_entrega.SelectedDate = fechaEntrega;
            txt_valor_tarea.Text = valor.ToString();

            // Inicializar la colección de semanas
            Semanas = new ObservableCollection<int>();
            for (int semana = 1; semana <= 15; semana++)
            {
                Semanas.Add(semana);
            }

            cbox_semana.ItemsSource = Semanas;

            // Establecer la fecha mínima del DatePicker como la fecha actual
            fecha_entrega.SelectedDate = DateTime.Today;
            

        }

        private void button_Regresar_Click(object sender, RoutedEventArgs e)
        {
            Profesor profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            profesor.Show();
            this.Close();
        }


        private void button_Editar_Click(object sender, RoutedEventArgs e)
        {

            if (cbox_semana.SelectedItem != null && !string.IsNullOrWhiteSpace(txt_categoria_tarea.Text) &&
                !string.IsNullOrWhiteSpace(txt_nombre_tarea.Text) && !string.IsNullOrWhiteSpace(txt_descripcion_tarea.Text))
            {
                // Obtener los valores de los controles
                int semana = Convert.ToInt32(cbox_semana.SelectedItem);
                string nombreAsignacion = txt_nombre_tarea.Text;
                string descripcion = txt_descripcion_tarea.Text;
                string tipo = txt_categoria_tarea.Text;
                DateTime fechaEntrega;
                //bool visibilidad = true;
                int valor;

                if (!int.TryParse(txt_valor_tarea.Text, out valor))
                {
                    MessageBox.Show("El valor debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Salir del evento sin continuar con la actualización
                }

                // Validar y obtener la fecha seleccionada del control DatePicker
                if (fecha_entrega.SelectedDate.HasValue)
                {
                    fechaEntrega = fecha_entrega.SelectedDate.Value;

                    // Verificar si la fecha seleccionada es anterior a la fecha actual
                    if (fechaEntrega < DateTime.Today)
                    {
                        MessageBox.Show("La fecha de entrega no puede ser anterior a la fecha actual.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; // Salir del evento sin continuar con la inserción
                    }
                }
                else
                {
                    // No se ha seleccionado una fecha
                    MessageBox.Show("Debe seleccionar una fecha de entrega.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Salir del evento sin continuar con la inserción
                }

                // Realizar la conexión a la base de datos y la actualización de datos
                string connString = conn_db.GetConnectionString();

                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();


                    // Crear la consulta SQL UPDATE
                    string query = "UPDATE asignacionesSemanas SET titulo = @titulo, tipo = @tipo, descripcion = @descripcion, FechaEntrega = @fechaEntrega, valor = @valor, semana = @semana WHERE asignacionesSemanas = @idAsignacion;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Asignar los valores a los parámetros de la consulta
                        command.Parameters.AddWithValue("@titulo", nombreAsignacion);
                        command.Parameters.AddWithValue("@tipo", tipo);
                        command.Parameters.AddWithValue("@descripcion", descripcion);
                        command.Parameters.AddWithValue("@fechaEntrega", fechaEntrega);
                        command.Parameters.AddWithValue("@valor", valor);
                        command.Parameters.AddWithValue("@semana", semana);
                        command.Parameters.AddWithValue("@idAsignacion", idAsignacion);

                        // Ejecutar la consulta
                        command.ExecuteNonQuery();

                        // Mostrar un mensaje de éxito
                        MessageBox.Show("La asignación se ha actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                // Limpiar los controles
                txt_nombre_tarea.Clear();
                txt_descripcion_tarea.Clear();
                txt_categoria_tarea.Clear();
                txt_valor_tarea.Clear();

                //MessageBox.Show(nombreAsignacion);
                //MessageBox.Show(tipo);
                //MessageBox.Show(descripcion);
                //MessageBox.Show(fechaEntrega.ToString());
                //MessageBox.Show(valor.ToString());
                //MessageBox.Show(semana.ToString());
                //MessageBox.Show(idAsignacion.ToString());


            }
            else
            {
                MessageBox.Show("Debe llenar todos los espacios.", "¡Falta información!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        private void nombre_tarea_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void categoría_tarea_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void descripción_tarea_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged_2(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}