using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programa_8
{
    public class Informacion
    {
        int PrimerNumero;
        string Operacion;
        int SegundoNumero;
        int TiempoMaximo;
        int TiempoRestante;
        int TiempoTranscurrido;
        int TiempoBloqueado;
        int Tamaño;
        int Numero;
        int Resultado;
        bool Error = false;
        bool Respuesta = false;
        int TiempoRespuesta;
        bool Interrupcion = false;
        bool Quantum = false;
        int TiempoLlegada;
        int TiempoFinalizacion;

        public Informacion(int PrimerNumero, string Ope, int SegundoNumero, int Time, int Num, int tamaño)
        {
            Operacion = Ope;
            TiempoMaximo = Time;
            Numero = Num;
            this.PrimerNumero = PrimerNumero;
            this.SegundoNumero = SegundoNumero;
            TiempoRestante = Time;
            TiempoTranscurrido = 0;
            TiempoBloqueado = 0;
            this.Tamaño = tamaño;
        }
        public string ProcesoInformacion()
        {
            string s = "ID:" + Numero.ToString() + " " + "TME:" + TiempoMaximo.ToString() + " " + "TR:" + TiempoRestante.ToString();
            return s;
        }
        public string GetOperando()
        {
            return Operacion;
        }

        public string GetOperacion()
        {
            string s = PrimerNumero.ToString() + Operacion + SegundoNumero.ToString();
            return s;
        }

        public int GetTiempo()
        {
            return TiempoMaximo;
        }

        public int GetNumero()
        {
            return Numero;
        }

        public int GetTamaño()
        {
            return Tamaño;
        }
        
        public void SetTamaño(int tamaño)
        {
            Tamaño = tamaño;
        }

        public int GetPrimerNumero()
        {
            return PrimerNumero;
        }
        public int GetTiempoRestante()
        {
            return TiempoRestante;
        }
        public void SetTiempoRestante(int Tiempo)
        {
            TiempoRestante = Tiempo;
        } 

        public int GetTiempoTranscurrido()
        {
            return TiempoTranscurrido;
        }

        public void SetTiempoTranscurrido(int Tiempo)
        {
            TiempoTranscurrido = Tiempo;
        }

        public int GetSegundoNumero()
        {
            return SegundoNumero;
        }

        public void SetError(bool Booleano)
        {
            Error = Booleano;
        }

        public bool GetError()
        {
            return Error;
        }

        public bool GetQuantum()
        {
            return Quantum;
        }
        public void SetQuantum(bool Booleano)
        {
            Quantum = Booleano;
        }
        
        public void SetInterrupcion(bool Booleano)
        {
            Interrupcion = Booleano;
        }

        public bool GetInterrupcion()
        {
            return Interrupcion;
        }

        public int GetTiempoBloqueado()
        {
            return TiempoBloqueado;
        }

        public void SetTiempoBloqueado(int Tiempo)
        {
            TiempoBloqueado = Tiempo;
        }

        public int GetResultado()
        {
            return Resultado;
        }

        public void SetResultado(int Numero)
        {
            Resultado = Numero;
        }

        public int GetTiempoLlegada()
        {
            return TiempoLlegada;
        }

        public void SetTiempoLlegada(int Numero)
        {
            TiempoLlegada = Numero;
        }

        public int GetTiempoFinalizacion()
        {
            return TiempoFinalizacion;
        }

        public void SetTiempoFinalizacion(int Numero)
        {
            TiempoFinalizacion = Numero;
        }

        public int GetTiempoRespuesta()
        {
            return TiempoRespuesta;
        }

        public void SetTiempoRespuesta(int Numero)
        {
            TiempoRespuesta = Numero;
        }

        public void SetRespuesta(bool Booleano)
        {
            Respuesta = Booleano;
        }

        public bool GetRespuesta()
        {
            return Respuesta;
        }

        public int GetMarcosTotales()
        {
            int Marco = Tamaño / 4;
            if (Tamaño % 4 != 0)
                Marco++;
            return Marco;
        }

        public int GetMarcos()
        {
            return Tamaño / 4;
        }

        public int GetResiduo()
        {
            return Tamaño % 4;
        }
    }
}
