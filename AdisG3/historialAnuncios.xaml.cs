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
    /// Interaction logic for historialAnuncios.xaml
    /// </summary>
    public partial class historialAnuncios : Window
    {
        public int id_profesor { get; set; }

        public int id_cursoSeleccionado { get; set; }

        public string nombreCursoSeleccionado { get; set; }



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
            List<Anuncio> historialAnuncios = new List<Anuncio>();

            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT titulo, descripcion FROM anuncios WHERE id_curso = @idCurso AND id_profesor = @idProfesor";

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
                                Titulo = reader.GetString("titulo"),
                                Descripcion = reader.GetString("descripcion")
                            };

                            historialAnuncios.Add(anuncio);
                        }
                    }
                }
            }

            lvAnuncios.ItemsSource = historialAnuncios;
        }

        public class Anuncio
        {
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor Profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            Profesor.Show();
            this.Close();
        }


        private void lvAnuncios_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
