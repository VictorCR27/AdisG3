using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AdisG3
{
    public partial class inicioEstudiante : Window
    {
        public int id_estudiante { get; set; }
        public int id_cursoSeleccionado { get; set; }

        string nombreCurso = "";

        int idCurso = 0;

        WrapPanel coursesWrapPanel; // Agregamos el WrapPanel

        public inicioEstudiante(int id_estudiante = 0, int id_cursoSeleccionado = 0)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;

            //MessageBox.Show($"Este es el id {id_estudiante}");

            string query = "SELECT COUNT(*) FROM estudiantes WHERE id_estudiante = @id_estudiante;";
            string cantidad;

            // Cadena de conexión
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_estudiante", id_estudiante);

                    // Ejecutar el query y obtener el resultado
                    object result = command.ExecuteScalar();

                    // Verificar si el resultado es nulo
                    if (result != null)
                    {
                        cantidad = result.ToString();
                    }
                    else
                    {
                        // Si el resultado es nulo, asignar un valor predeterminado o mostrar un mensaje de error, según tus necesidades
                        cantidad = "0";
                    }
                }
            }

            // Limita la cantidad de cursos a un máximo de 5
            int cantidadMaxima = int.Parse(cantidad);

            // Elimina los botones de cursos existentes en el grid
            CursosGrid.Children.Clear();
            CursosGrid.RowDefinitions.Clear();

            // Calcula la cantidad de filas necesarias
            int filas = (int)Math.Ceiling((double)cantidadMaxima / 4);

            // Agrega las filas al grid
            for (int i = 0; i < filas; i++)
            {
                CursosGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Crea el WrapPanel y configura su orientación
            coursesWrapPanel = new WrapPanel();
            coursesWrapPanel.Orientation = Orientation.Horizontal;

            // Agrega los botones de cursos al grid
            int index = 0; // Variable para controlar el índice de los resultados de la consulta

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                string query1 = "SELECT cursos.id_curso, cursos.nombre_curso FROM cursos JOIN estudiantesMatriculados ON cursos.id_curso = estudiantesMatriculados.id_curso WHERE estudiantesMatriculados.id_estudiante = @id_estudiante;";

                using (MySqlCommand command = new MySqlCommand(query1, connection))
                {
                    command.Parameters.AddWithValue("@id_estudiante", id_estudiante);

                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            idCurso = reader.GetInt32("id_curso");
                            nombreCurso = reader.GetString("nombre_curso");

                            Button cursoButton = new Button();
                            cursoButton.Width = 150;
                            cursoButton.Height = 150;
                            cursoButton.Margin = new Thickness(10);
                            cursoButton.Tag = idCurso;
                            cursoButton.Click += CursoButton_Click;

                            Grid grid = new Grid();

                            RowDefinition rowDefinition1 = new RowDefinition();
                            rowDefinition1.Height = new GridLength(1, GridUnitType.Star);
                            grid.RowDefinitions.Add(rowDefinition1);

                            RowDefinition rowDefinition2 = new RowDefinition();
                            rowDefinition2.Height = new GridLength(1, GridUnitType.Star);
                            grid.RowDefinitions.Add(rowDefinition2);

                            ColumnDefinition columnDefinition = new ColumnDefinition();
                            columnDefinition.Width = new GridLength(1, GridUnitType.Star);
                            grid.ColumnDefinitions.Add(columnDefinition);

                            Label nombreLabel = new Label();
                            nombreLabel.Content = nombreCurso;
                            nombreLabel.HorizontalAlignment = HorizontalAlignment.Left;
                            nombreLabel.VerticalAlignment = VerticalAlignment.Bottom;
                            nombreLabel.Margin = new Thickness(-25, 0, 5, 0);

                            Grid.SetRow(nombreLabel, 1);
                            Grid.SetColumn(nombreLabel, 0);

                            grid.Children.Add(nombreLabel);

                            cursoButton.Content = grid;

                            cursoButton.Click += CursoButton_Click;
                            cursoButton.Tag = idCurso;

                            cursoButton.Background = new SolidColorBrush(Colors.LightBlue);

                            coursesWrapPanel.Children.Add(cursoButton);

                            index++;
                        }

                        CursosGrid.Children.Add(coursesWrapPanel);
                    }
                }
            }
        }

        private void CursoButton_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el ID del curso seleccionado del botón
            Button cursoButton = (Button)sender;
            int idCursoSeleccionado = (int)cursoButton.Tag;
            string nombreCursoSeleccionado = ((Label)((Grid)cursoButton.Content).Children[0]).Content.ToString();

            CursosEstudiantes CursosEstudiantes = new CursosEstudiantes(id_estudiante, idCursoSeleccionado, nombreCursoSeleccionado);
            CursosEstudiantes.Show();
            this.Close();
        }

        private void Button_Cursos(object sender, RoutedEventArgs e)
        {
            // Ocultar el contenido del perfil
            PerfilGrid.Visibility = Visibility.Collapsed;

            // Mostrar el contenido de los cursos
            CursosGrid.Visibility = Visibility.Visible;

            // Ocultar el contenido de la institución
            TextoInstitucion.Visibility = Visibility.Collapsed;
        }

        private void Button_Cuenta(object sender, RoutedEventArgs e)
        {
            // Mostrar el contenido del perfil
            PerfilGrid.Visibility = Visibility.Visible;

            // Ocultar el contenido de los cursos
            CursosGrid.Visibility = Visibility.Collapsed;

            // Ocultar el contenido de la institución
            TextoInstitucion.Visibility = Visibility.Collapsed;
        }

        private void Button_Insti(object sender, RoutedEventArgs e)
        {
            // Ocultar el contenido del perfil
            PerfilGrid.Visibility = Visibility.Collapsed;

            // Ocultar el contenido de los cursos
            CursosGrid.Visibility = Visibility.Collapsed;

            // Mostrar el contenido de la institución
            TextoInstitucion.Visibility = Visibility.Visible;

            // Establecer el texto
            TextoInstitucion.Text = "   Para ULACIT, el aprendizaje es la capacidad de un individuo de utilizar el conocimiento en \n" +
                                    "   situaciones novedosas (por ejemplo, solucionar problemas, \n" +
                                    "   diseñar productos o argumentar puntos de vista),\n" +
                                    "   de formas semejantes a las que modelan los expertos en disciplinas específicas. Los estudiantes \n" +
                                    "   demuestran su aprendizaje cuando son capaces de ir más allá de la acumulación de información y \n" +
                                    "   realizan proyectos que son valorados por las comunidades en que viven. Estos proyectos los llevan a \n" +
                                    "   cabo en equipos de trabajo, utilizando las metodologías de gestión de proyectos usadas \n" +
                                    "   en los ambientes laborales." +
                                    "   Las lecciones se imparten en aulas invertidas: la teoría se ve en casa, \n" +
                                    "   en las aulas virtuales; y la práctica, en el aula \n" +
                                    "   presencial. Por tanto, el tiempo en el aula se centra en ejercicios, proyectos y discusiones, \n" +
                                    "   no en lecciones magistrales.\n" +
                                    "   Familiarícese con nuestro modelo educativo y metodología de enseñanza basada en proyectos.";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }
    }
}
