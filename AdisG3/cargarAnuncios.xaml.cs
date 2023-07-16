using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace AdisG3
{
    public partial class cargarAnuncios : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        public class Anuncio
        {
            public string Nombre { get; set; }
            public string Mensaje { get; set; }
        }
        public ObservableCollection<Anuncio> Estudiantes { get; set; }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Profesor Profesor = new Profesor(id_profesor, id_cursoSeleccionado, nombreCursoSeleccionado);
            Profesor.Show();
            this.Close();
        }

        private void Button_Enviar(object sender, RoutedEventArgs e)
        {

        }
    }
}
