using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AdisG3
{
    public partial class historialAnuncios : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public int id_anuncios { get; set; }

        public historialAnuncios(int id_profesor = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "")
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            LoadHistorialAnuncios();
        }

        private void LoadHistorialAnuncios()
        {
            List<Anuncio> historialAnunciosList = new List<Anuncio>();

            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                //Traer id_anuncio ya que sale como 0
                string query = "SELECT a.id_anuncios, a.titulo, a.descripcion, p.nombre " +
                               "FROM anuncios a " +
                               "INNER JOIN profesores p ON a.id_profesor = p.id_profesor " +
                               "WHERE a.id_curso = @idCurso AND a.id_profesor = @idProfesor";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idCurso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@idProfesor", id_profesor);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Anuncio anuncio = new Anuncio
                            {
                                Id = reader.GetInt16("id_anuncios"),
                                Titulo = reader.GetString("titulo"),
                                Descripcion = reader.GetString("descripcion"),
                                Profesor = reader.GetString("nombre")
                            };

                            historialAnunciosList.Add(anuncio);
                        }
                    }
                }
            }

            lvAnuncios.ItemsSource = historialAnunciosList;
        }

        private void VerMasButton_Click(object sender, RoutedEventArgs e)
        {
            Button verMasButton = sender as Button;
            if (verMasButton != null)
            {
                Anuncio selectedAnuncio = verMasButton.DataContext as Anuncio;
                if (selectedAnuncio != null)
                {
                    DetalleAnuncio ventanaDetalles = new DetalleAnuncio(selectedAnuncio.Titulo, selectedAnuncio.Descripcion, selectedAnuncio.Profesor);
                    ventanaDetalles.ShowDialog();
                }
            }
        }

        private void EliminarButton_Click(object sender, RoutedEventArgs e)
        {
            Button eliminarButton = sender as Button;
            if (eliminarButton != null)
            {
                Anuncio anuncioAEliminar = eliminarButton.DataContext as Anuncio;
                if (anuncioAEliminar != null)
                {

                    if (MessageBox.Show("¿Estás seguro de que deseas eliminar este anuncio?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        // Realizar la eliminación en la base de datos
                        EliminarAnuncio(anuncioAEliminar);
                        //MessageBox.Show(anuncioAEliminar.Id.ToString());

                        // Remover el anuncio de la lista
                        var historialAnunciosList = lvAnuncios.ItemsSource as List<Anuncio>;
                        historialAnunciosList.Remove(anuncioAEliminar);
                    }
                }
            }
        }

        private void EliminarAnuncio(Anuncio anuncio)
        {
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "DELETE FROM anuncios WHERE id_anuncios = @idAnuncio";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idAnuncio", anuncio.Id); // Usar la propiedad Id

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Anuncio eliminado correctamente, actualiza la lista
                        LoadHistorialAnuncios();
                    }
                }
            }
        }

        public class Anuncio
        {
            public int Id { get; set; } // Agregar esta propiedad para el id_anuncios
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public string Profesor { get; set; }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor Profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            Profesor.Show();
            this.Close();
        }

        private void lvAnuncios_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            // Aquí puedes manejar el cambio de selección si es necesario
        }
    }
}