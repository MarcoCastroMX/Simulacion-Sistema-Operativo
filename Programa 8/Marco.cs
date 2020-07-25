using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programa_8
{
    class Marco
    {
        int NumPaginas;
        int IdActual;
        string Estado;

        public Marco()
        {
            this.IdActual = -1;
            this.NumPaginas = 0;
            this.Estado = "Vacio";
        }
        public int GetNumPaginas()
        {
            return NumPaginas;
        }

        public void SetNumPaginas(int NumPaginas)
        {
            this.NumPaginas = NumPaginas;
        }

        public int GetIdActual()
        {
            return IdActual;
        }

        public void SetIdActual(int IdActual)
        {
            this.IdActual = IdActual;
        }

        public string GetEstado()
        {
            return Estado;
        }

        public void SetEstado(string Estado)
        {
            this.Estado = Estado;
        }
    }
}
