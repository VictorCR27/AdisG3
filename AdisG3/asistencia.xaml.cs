﻿using System;
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
    /// Lógica de interacción para asistencia.xaml
    /// </summary>
    public partial class asistencia : Window
    {
        public asistencia(int id_profesor, int id_cursoSeleccionado, string nombreCursoSeleccionado)
        {
            InitializeComponent();
        }
    }
}
