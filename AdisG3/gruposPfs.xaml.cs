using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AdisG3
{
    public partial class gruposPfs : Window
    {
        public int id_profesor { get; set; }
        public int id_cursoSeleccionado { get; set; }
        public string nombreCursoSeleccionado { get; set; }

        

        public gruposPfs(int id_profesor, int id_cursoSeleccionado, string nombreCursoSeleccionado)
        {
            InitializeComponent();

            this.id_profesor = id_profesor;
            this.id_cursoSeleccionado = id_cursoSeleccionado;
            this.nombreCursoSeleccionado = nombreCursoSeleccionado;

            
        }

        
    }
}
