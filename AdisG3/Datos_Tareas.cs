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

        public String Fecha { get; set; }

        public String Valor { get; set; }

        public String Semanas { get; set; }

        public Datos_Tareas(string nombre, string categoria, string descripcion, string fecha, string valor, string semanas)
        {
            Nombre = nombre;
            Categoria = categoria;
            Descripcion = descripcion;
            Fecha = fecha;
            Valor = valor;
            Semanas = semanas;
        }

        public Datos_Tareas() { }

    }
}
