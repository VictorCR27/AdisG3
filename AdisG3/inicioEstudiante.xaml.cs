using System.Windows;
using System.Windows.Controls;

namespace AdisG3
{
    public partial class inicioEstudiante : Window
    {
        public inicioEstudiante()
        {
            InitializeComponent();
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
    }
}
