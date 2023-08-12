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

        int id_profesor = 0;

        WrapPanel coursesWrapPanel; // Agregamos el WrapPanel

        List<int> idProfesores = new List<int>();

        public inicioEstudiante(int id_estudiante = 0, int id_cursoSeleccionado = 0)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            this.id_cursoSeleccionado = id_cursoSeleccionado;

            string connString = conn_db.GetConnectionString();
            string perfilQuery = "SELECT nombre, apellido1, apellido2, correo, password FROM estudiantes WHERE id_estudiante = @id_estudiante";

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                // Obtener información del perfil del usuario
                using (MySqlCommand perfilCommand = new MySqlCommand(perfilQuery, connection))
                {
                    perfilCommand.Parameters.AddWithValue("@id_estudiante", id_estudiante);

                    using (MySqlDataReader perfilReader = perfilCommand.ExecuteReader())
                    {
                        if (perfilReader.Read())
                        {
                            string nombreUsuario = perfilReader.GetString("nombre");
                            string apellido1Usuario = perfilReader.GetString("apellido1");
                            string apellido2Usuario = perfilReader.GetString("apellido2");
                            string correoUsuario = perfilReader.GetString("correo");
                            string password = perfilReader.GetString("password");

                            // Combinar los apellidos y nombre en un solo campo
                            string nombreCompleto = $"{nombreUsuario} {apellido1Usuario} {apellido2Usuario}";

                            // Mostrar la información en los controles de la interfaz
                            NombreTextBlock.Text = nombreCompleto;
                            CorreoTextBlock.Text = correoUsuario;
                            passwordtxt.Text = password;
                        }
                    }
                }
            }


            string query = "SELECT COUNT(*) FROM estudiantes WHERE id_estudiante = @id_estudiante;";
            string cantidad;

            // Cadena de conexión
            //string connString = conn_db.GetConnectionString();

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
            //MessageBox.Show(cantidad);
            int cantidadMaxima = int.Parse(cantidad);

            // Elimina los botones de cursos existentes en el grid
            CursosGrid.Children.Clear();
            CursosGrid.ColumnDefinitions.Clear();

            // Calcula la cantidad de filas necesarias
            int filas = (int)Math.Ceiling((double)cantidadMaxima / 4);

            // Agrega las filas y columnas al grid
            for (int i = 0; i < filas; i++)
            {
                CursosGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < 4; i++)
            {
                CursosGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Agrega los botones de cursos al grid
            int index = 0; // Variable para controlar el índice de los resultados de la consulta

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                // Crea y ejecuta la consulta para obtener los nombres de los cursos
                string query1 = "SELECT cursos.id_curso, cursos.nombre_curso, estudiantesMatriculados.id_profesor FROM cursos JOIN estudiantesMatriculados " +
                    "ON cursos.id_curso = estudiantesMatriculados.id_curso WHERE estudiantesMatriculados.id_estudiante = @id_estudiante;";

                using (MySqlCommand command = new MySqlCommand(query1, connection))
                {
                    command.Parameters.AddWithValue("@id_estudiante", id_estudiante);

                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        Random r = new Random();

                        while (reader.Read())
                        {
                            idCurso = reader.GetInt32("id_curso");
                            nombreCurso = reader.GetString("nombre_curso");
                            int idProfesor = reader.GetInt32("id_profesor");
                            idProfesores.Add(idProfesor);

                            Button cursoButton = new Button();
                            cursoButton.Width = 150;
                            cursoButton.Height = 150;
                            cursoButton.Margin = new Thickness(3);

                            // Configurar el contenido del botón con el nombre y la descripción abajo a la izquierda
                            Grid grid = new Grid();

                            RowDefinition rowDefinition1 = new RowDefinition();
                            rowDefinition1.Height = new GridLength(1, GridUnitType.Auto);
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
                            cursoButton.Tag = idCurso; // Almacena el ID del curso en la propiedad Tag del botón

                            // Establecer el fondo del botón del curso como un color sólido

                            cursoButton.Background = new SolidColorBrush(Color.FromRgb((byte)r.Next(100, 255), (byte)r.Next(100, 255), (byte)r.Next(100, 255)));

                            // Calcula la posición de la fila y columna en la cuadrícula
                            int fila = index / 4;
                            int columna = index % 4;

                            // Establecer la posición de la fila y columna en el botón del curso
                            Grid.SetRow(cursoButton, fila);
                            Grid.SetColumn(cursoButton, columna);

                            CursosGrid.Children.Add(cursoButton);

                            index++;
                        }
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

            // Obtener el índice del botón seleccionado
            int index = CursosGrid.Children.IndexOf(cursoButton);

            // Obtener el id_profesor correspondiente al índice del botón
            int idProfesorSeleccionado = idProfesores[index];

            CursosEstudiantes CursosEstudiantes = new CursosEstudiantes(id_estudiante, idCursoSeleccionado, nombreCursoSeleccionado, idProfesorSeleccionado);
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
            TextoInstitucion.TextAlignment = TextAlignment.Justify;
            TextoInstitucion.Text = "   Para ULACIT, el aprendizaje es la capacidad de un individuo de utilizar el conocimiento en situaciones novedosas (por ejemplo, solucionar problemas," +
                                    " diseñar productos o argumentar puntos de vista), de formas semejantes a las que modelan los expertos en disciplinas específicas. Los estudiantes" +
                                    " demuestran su aprendizaje cuando son capaces de ir más allá de la acumulación de información y realizan proyectos que son valorados por las comunidades en que viven. Estos proyectos los llevan a" +
                                    " cabo en equipos de trabajo, utilizando las metodologías de gestión de proyectos usadas en los ambientes laborales. Las lecciones se imparten en aulas invertidas: la teoría se ve en casa," +
                                    " en las aulas virtuales; y la práctica, en el aula presencial. Por tanto, el tiempo en el aula se centra en ejercicios, proyectos y discusiones, no en lecciones magistrales" +
                                    " Familiarícese con nuestro modelo educativo y metodología de enseñanza basada en proyectos.";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }
    }
}