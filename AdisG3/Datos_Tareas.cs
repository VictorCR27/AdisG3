using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdisG3
{
    public class Datos_Tareas
    {
        public String Nombre { get; set; }
        public String Categoria { get; set; }
        public String Descripcion { get; set; }

        public Datos_Tareas(string nombre, string categoria, string descripcion)
        {
            Nombre = nombre;
            Categoria = categoria;
            Descripcion = descripcion;
        }

        public Datos_Tareas() { }

    }
}
