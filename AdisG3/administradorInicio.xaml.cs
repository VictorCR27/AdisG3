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
    /// Interaction logic for administradorInicio.xaml
    /// </summary>
    public partial class administradorInicio : Window
    {
        public int id_profesor { get; set; }

        public administradorInicio(int id_profesor = 0)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            //MessageBox.Show("Esta es el Id:" + id_profesor);

            string query = "SELECT count(*) from asignacionesProfesor WHERE id_profesor = @id";
            string cantidad;

            // Cadena de conexión
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id_profesor);

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
            for (int i = 0; i < cantidadMaxima; i++)
            {
                Button cursoButton = new Button();
                cursoButton.Width = 200;
                cursoButton.Height = 150;
                cursoButton.Margin = new Thickness(10);

                // Obtener el nombre del curso
                string nombreCurso = ObtenerNombreCurso(i);

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

                // Establecer el fondo del botón del curso como un color sólido
                cursoButton.Background = new SolidColorBrush(Colors.LightBlue);

                // Calcula la posición de la fila y columna en la cuadrícula
                int fila = i / 4;
                int columna = i % 4;

                // Establecer la posición de la fila y columna en el botón del curso
                Grid.SetRow(cursoButton, fila);
                Grid.SetColumn(cursoButton, columna);

                CursosGrid.Children.Add(cursoButton);
            }



        }

        // Método para obtener el nombre del curso desde la base de datos
        private string ObtenerNombreCurso(int cursoIndex)
        {
            string nombreCurso = string.Empty;

            // Establece tu cadena de conexión
            string connString = conn_db.GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();

                // Crea y ejecuta la consulta para obtener el nombre del curso
                string query = "SELECT nombre_curso FROM cursos WHERE id_curso = @id_curso";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Establece el parámetro de la consulta
                    command.Parameters.AddWithValue("@id_curso", cursoIndex + 1); // +1 para adaptarse a tu estructura de base de datos

                    // Ejecuta la consulta y obtén el resultado
                    object result = command.ExecuteScalar();

                    // Verifica si el resultado no es nulo
                    if (result != null)
                    {
                        nombreCurso = result.ToString();
                    }
                }
            }

            return nombreCurso;
        }

        private void CursoButton_Click(object sender, RoutedEventArgs e)
        {
           
            Profesor profesor = new Profesor(id_profesor); 
            profesor.Show();
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
            TextoInstitucion.Text = "                                                                                                \n" +
                                    "                                                                                                \n" +
                                    "   Para ULACIT, el aprendizaje es la capacidad de un individuo de utilizar el conocimiento en \n" +
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

        private void CursosButton_Click(object sender, RoutedEventArgs e)
        {
            // Crear una instancia de la ventana CursosEstudiantes
            CursosEstudiantes cursosEstudiantes = new CursosEstudiantes();

            // Mostrar la ventana de CursosEstudiantes
            cursosEstudiantes.Show();

            // Cerrar la ventana actual (inicioEstudiante)
            this.Close();
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AgregarTarea agregar_Tarea = new AgregarTarea(id_profesor);
            this.Close();
            agregar_Tarea.Show();

        }
    }
}
