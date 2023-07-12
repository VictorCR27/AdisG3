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
    /// Interaction logic for Profesor.xaml
    /// </summary>
    public partial class Profesor : Window
    {
        public Profesor()
        {
            InitializeComponent();
        }

        private void Button_sem1(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem1());
        }

        private void Button_sem2(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem2());
        }

        private void Button_sem3(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem3());
        }

        private void Button_sem4(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem4());
        }

        private void Button_sem5(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem5());
        }

        private void Button_sem6(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem6());
        }

        private void Button_sem7(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem7());
        }

        private void Button_sem8(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem8());
        }

        private void Button_sem9(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem9());
        }

        private void Button_sem10(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem10());
        }

        private void Button_sem11(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem11());
        }

        private void Button_sem12(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem12());
        }

        private void Button_sem13(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem13());
        }

        private void Button_sem14(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem14());
        }

        private void Button_sem15(object sender, RoutedEventArgs e)
        {
            Semanas_Frame1.NavigationService.Navigate(new Semanas.Pag_sem15());
        }

        private void Semanas1_Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Nota(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AgregarTarea agregar_Tarea = new AgregarTarea();
            this.Close();
            agregar_Tarea.Show();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            // Abrir ventana de inicio de sesión
            administradorInicio administradorInicio = new administradorInicio();
            administradorInicio.Show();
            this.Close();
        }


    }
}
