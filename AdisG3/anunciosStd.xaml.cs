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
    /// Interaction logic for anunciosStd.xaml
    /// </summary>
    public partial class anunciosStd : Window
    {
        public int id_estudiante { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }
        public int idProfesorSeleccionado { get; set; }

        public anunciosStd(int id_estudiante = 0, int id_cursoSeleccionado = 0, string nombreCursoSeleccionado = "", int idProfesorSeleccionado = 0)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;
            this.idProfesorSeleccionado = idProfesorSeleccionado;

            LoadAnuncios();
        }

        private void LoadAnuncios()
        {
            List<Anuncio> anuncios = new List<Anuncio>();

            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                string query = "SELECT a.titulo, a.descripcion, p.nombre " +
                                             "FROM anuncios a " +
                                             "INNER JOIN profesores p ON a.id_profesor = p.id_profesor " +
                                             "WHERE a.id_curso = @idCurso AND a.id_profesor = @idProfesor";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idCurso", id_cursoSeleccionado);
                    command.Parameters.AddWithValue("@idProfesor", idProfesorSeleccionado);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Anuncio anuncio = new Anuncio
                            {
                                Titulo = reader.GetString("titulo"),
                                Descripcion = reader.GetString("descripcion"),
                                Profesor = reader.GetString("nombre")
                            };

                            anuncios.Add(anuncio);
                        }
                    }
                }
            }

            lvAnuncios.ItemsSource = anuncios;
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

        public class Anuncio
        {
            public int Id { get; set; } // Agregar esta propiedad para el id_anuncios
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public string Profesor { get; set; }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CursosEstudiantes CursosEstudiantes = new CursosEstudiantes(id_estudiante, id_cursoSeleccionado, nombreCursoSeleccionado, idProfesorSeleccionado);
            CursosEstudiantes.Show();
            this.Close();
        }

        private void lvAnuncios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lvAnuncios_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}