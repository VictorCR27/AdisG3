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
        public inicioEstudiante(int id_estudiante = 0)
        {
            InitializeComponent();

            this.id_estudiante = id_estudiante;
            MessageBox.Show("El Id del estudiante es: "+ id_estudiante);

            // Limita la cantidad de cursos a un máximo de 5
            int cantidadMaxima = 5;

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
                cursoButton.Width = 150;
                cursoButton.Height = 150;
                cursoButton.Margin = new Thickness(10);
                cursoButton.Click += CursoButton_Click;

                // Establecer el fondo del botón del curso como un color sólido
                cursoButton.Background = new SolidColorBrush(Colors.LightBlue);

                // Calcula la posición de la fila y columna en la cuadrícula
                int fila = i / 4;
                int columna = i % 4;

                // Establecer la posición de la fila y columna en el botón del curso
                Grid.SetRow(cursoButton, fila);
                Grid.SetColumn(cursoButton, columna);

                // Establecer la alineación vertical del botón como "Top"
                cursoButton.VerticalAlignment = VerticalAlignment.Top;

                CursosGrid.Children.Add(cursoButton);
            }

        }

        private void CursoButton_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para manejar el evento de clic en el botón del curso
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
    }
}
