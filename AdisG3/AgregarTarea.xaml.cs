﻿using MySql.Data.MySqlClient;
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

        public AgregarTarea(int id_profesor = 0)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;

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

        //Boton de añadir tarea
        private void button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (cbox_semana.SelectedItem != null && txt_categoria_tarea.Text != null && txt_nombre_tarea.Text != null &&
                txt_nombre_tarea.Text != " " && txt_descripcion_tarea.Text != null && txt_descripcion_tarea.Text != " " &&
                txt_nombre_tarea.Text != string.Empty && txt_descripcion_tarea.Text != string.Empty)
            {
                // Obtener los valores de los controles
                int semana = Convert.ToInt32(cbox_semana.SelectedItem);
                string nombreAsignacion = txt_nombre_tarea.Text;
                string descripcion = txt_descripcion_tarea.Text;
                string tipo =""; // Reemplazar con el valor correcto
                DateTime fechaEntrega;
                bool visibilidad = true; // Reemplazar con el valor correcto
                decimal puntaje = Convert.ToDecimal(txt_valor_tarea.Text); // Reemplazar con el valor correcto

                // Validar y obtener la fecha seleccionada del control DatePicker
                if (fecha_entrega.SelectedDate.HasValue)
                {
                    fechaEntrega = fecha_entrega.SelectedDate.Value;
                }
                else
                {
                    // No se ha seleccionado una fecha
                    MessageBox.Show("Debe seleccionar una fecha de entrega.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Salir del evento sin continuar con la inserción
                }

                // Realizar la conexión a la base de datos y la inserción de datos
                string connString = conn_db.GetConnectionString();

                using (MySqlConnection connection = new MySqlConnection(connString))
                {
                    connection.Open();

                    // Obtener el id de asignación del profesor y el id del curso
                    int idProfesor = 1; // Reemplazar con el valor correcto
                    int idCurso = 1; // Reemplazar con el valor correcto

                    // Crear la consulta SQL INSERT
                    string query = "INSERT INTO asignacionesSemanas (id_asignacion, id_semana, CedulaUsuario, IDCurso, NombreAsignacion, Descripcion, Semana, Tipo, FechaEntrega, Visibilidad, Puntaje) " +
                                   "VALUES (@id_asignacion, @id_semana, @cedulaUsuario, @idCurso, @nombreAsignacion, @descripcion, @semana, @tipo, @fechaEntrega, @visibilidad, @puntaje)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Asignar los valores a los parámetros de la consulta
                        command.Parameters.AddWithValue("@id_asignacion", 1); // Reemplazar con el valor correcto
                        command.Parameters.AddWithValue("@id_semana", 1); // Reemplazar con el valor correcto
                        command.Parameters.AddWithValue("@cedulaUsuario", 12345); // Reemplazar con el valor correcto
                        command.Parameters.AddWithValue("@idCurso", idCurso);
                        command.Parameters.AddWithValue("@nombreAsignacion", nombreAsignacion);
                        command.Parameters.AddWithValue("@descripcion", descripcion);
                        command.Parameters.AddWithValue("@semana", semana);
                        command.Parameters.AddWithValue("@tipo", tipo);
                        command.Parameters.AddWithValue("@fechaEntrega", fechaEntrega);
                        command.Parameters.AddWithValue("@visibilidad", visibilidad);
                        command.Parameters.AddWithValue("@puntaje", puntaje);

                        // Ejecutar la consulta
                        command.ExecuteNonQuery();

                        // Mostrar un mensaje de éxito
                        MessageBox.Show("La asignación se ha agregado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                // Limpiar los controles
                txt_nombre_tarea.Clear();
                txt_descripcion_tarea.Clear();
            }
            else
            {
                MessageBox.Show("Debe llenar todos los espacios.", "¡Falta información!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        // Fin del boton


        private void Llenar_Tareas()
        {
            try
            {
                //Asigna los valores de los textbox, combobox y select date a variables, para guardarlas en "Datos"
                string nombre = txt_nombre_tarea.Text;
                string categoria = txt_categoria_tarea.Text;
                string descripcion = txt_descripcion_tarea.Text;
                string fecha = fecha_entrega.Text;
                string valor = txt_valor_tarea.Text;
                string semana= cbox_semana.SelectedItem.ToString();

                //Guardas las variables en la lista
                Datos.Add(new Datos_Tareas($"{nombre}          ", $"|          {categoria}          |", $"           {descripcion}          |", $"           {fecha}          |", $"           {valor}          |", $"           {semana}          |"));

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
            Profesor profesor = new Profesor(id_profesor);
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
