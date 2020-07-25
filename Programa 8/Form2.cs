using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Programa_8
{
    public partial class Form2 : Form
    {
        List<Informacion> Lista = new List<Informacion>();
        List<Informacion> Memoria = new List<Informacion>();
        List<Informacion> Bloqueado = new List<Informacion>();
        List<Informacion> Finalizados = new List<Informacion>();
        List<Informacion> Suspendidos = new List<Informacion>();
        public Form2()
        {
            InitializeComponent();
        }

        public Form2(List<Informacion> Terminados, List<Informacion> EnMemoria, List<Informacion> Bloqueados, List<Informacion> Nuevos, List<Informacion> Suspendidos,int Contador)
        {
            InitializeComponent();
            Finalizados = Terminados;
            Memoria = EnMemoria;
            Bloqueado = Bloqueados;
            Lista = Nuevos;
            foreach (Informacion i in Lista)
            {
                ListBoxDatos.Items.Add("Numero de Programa: " + i.GetNumero());
                ListBoxDatos.Items.Add("Estado del Proceso: Nuevo");
                ListBoxDatos.Items.Add("Tiempo Maximo Estimado: " + i.GetTiempo());
                ListBoxDatos.Items.Add("Tamaño: " + i.GetTamaño());
                ListBoxDatos.Items.Add("Marcos: " + i.GetMarcosTotales());
                ListBoxDatos.Items.Add("");
                ListBoxDatos.Refresh();
            }
            foreach(Informacion i in Memoria)
            {

                ListBoxDatos.Items.Add("Numero de Programa: " + i.GetNumero());
                ListBoxDatos.Items.Add("Estado del Proceso: Memoria");
                ListBoxDatos.Items.Add("Operacion: " + i.GetOperacion());
                ListBoxDatos.Items.Add("Tiempo Maximo Estimado: " + i.GetTiempo());
                ListBoxDatos.Items.Add("Tiempo Llegada: " + i.GetTiempoLlegada());
                ListBoxDatos.Items.Add("Tiempo Espera: " + ((Contador - i.GetTiempoLlegada()) - i.GetTiempoTranscurrido()));
                ListBoxDatos.Items.Add("Tiempo Servicio: " + i.GetTiempoTranscurrido());
                ListBoxDatos.Items.Add("Tiempo Restante: " + i.GetTiempoRestante());
                ListBoxDatos.Items.Add("Tiempo Respuesta: " + i.GetTiempoRespuesta());
                ListBoxDatos.Items.Add("Tamaño: " + i.GetTamaño());
                ListBoxDatos.Items.Add("Marcos: " + i.GetMarcosTotales());
                ListBoxDatos.Items.Add("");
                ListBoxDatos.Refresh();

            }
            foreach(Informacion i in Bloqueado)
            {
                ListBoxDatos.Items.Add("Numero de Programa: " + i.GetNumero());
                ListBoxDatos.Items.Add("Estado del Proceso: Bloqueado");
                ListBoxDatos.Items.Add("Operacion: " + i.GetOperacion());
                ListBoxDatos.Items.Add("Tiempo Maximo Estimado: " + i.GetTiempo());
                ListBoxDatos.Items.Add("Tiempo Llegada: " + i.GetTiempoLlegada());
                ListBoxDatos.Items.Add("Tiempo Espera: " + ((Contador - i.GetTiempoLlegada()) - i.GetTiempoTranscurrido()));
                ListBoxDatos.Items.Add("Tiempo Servicio: " + i.GetTiempoTranscurrido());
                ListBoxDatos.Items.Add("Tiempo Restante: " + i.GetTiempoRestante());
                ListBoxDatos.Items.Add("Tiempo Respuesta: " + i.GetTiempoRespuesta());
                ListBoxDatos.Items.Add("Tiempo Bloqueado: " + i.GetTiempoBloqueado());
                ListBoxDatos.Items.Add("Tamaño: " + i.GetTamaño());
                ListBoxDatos.Items.Add("Marcos: " + i.GetMarcosTotales());
                ListBoxDatos.Items.Add("");
                ListBoxDatos.Refresh();

            }
            foreach (Informacion i in Finalizados)
            {
                ListBoxDatos.Items.Add("Numero de Programa: " + i.GetNumero());
                ListBoxDatos.Items.Add("Estado del Proceso: Finalizado");
                ListBoxDatos.Items.Add("Operacion: " + i.GetOperacion());
                if(i.GetResultado()==-1)
                    ListBoxDatos.Items.Add("Resultado: ERROR");
                else
                    ListBoxDatos.Items.Add("Resultado: "+ i.GetResultado().ToString());
                ListBoxDatos.Items.Add("Tiempo Maximo Estimado: " +i.GetTiempo());
                ListBoxDatos.Items.Add("Tiempo Restante: " + i.GetTiempoRestante());
                ListBoxDatos.Items.Add("Tiempo Transcurrido: " + i.GetTiempoTranscurrido());
                ListBoxDatos.Items.Add("Tiempo Llegada: " + i.GetTiempoLlegada());
                ListBoxDatos.Items.Add("Tiempo Finalizacion: " + i.GetTiempoFinalizacion());
                ListBoxDatos.Items.Add("Tiempo Servicio: " + i.GetTiempoTranscurrido());
                ListBoxDatos.Items.Add("Tiempo Retorno: " + (i.GetTiempoFinalizacion() - i.GetTiempoLlegada()).ToString());
                ListBoxDatos.Items.Add("Tiempo Respuesta: " + i.GetTiempoRespuesta());
                ListBoxDatos.Items.Add("Tiempo Espera: " + ((i.GetTiempoFinalizacion() - i.GetTiempoLlegada())-i.GetTiempoTranscurrido()));
                ListBoxDatos.Items.Add("Tamaño: " + i.GetTamaño());
                ListBoxDatos.Items.Add("Marcos: " + i.GetMarcosTotales());
                ListBoxDatos.Items.Add("");
                ListBoxDatos.Refresh();
            }
            foreach (Informacion i in Suspendidos)
            {
                ListBoxDatos.Items.Add("Numero de Programa: " + i.GetNumero());
                ListBoxDatos.Items.Add("Estado del Proceso: Bloqueado y Suspendido");
                ListBoxDatos.Items.Add("Operacion: " + i.GetOperacion());
                ListBoxDatos.Items.Add("Tiempo Maximo Estimado: " + i.GetTiempo());
                ListBoxDatos.Items.Add("Tiempo Restante: " + i.GetTiempoRestante());
                ListBoxDatos.Items.Add("Tiempo Transcurrido: " + i.GetTiempoTranscurrido());
                ListBoxDatos.Items.Add("Tiempo Llegada: " + i.GetTiempoLlegada());
                ListBoxDatos.Items.Add("Tiempo Servicio: " + i.GetTiempoTranscurrido());
                ListBoxDatos.Items.Add("Tiempo Respuesta: " + i.GetTiempoRespuesta());
                ListBoxDatos.Items.Add("Tiempo Espera: " + ((Contador - i.GetTiempoLlegada()) - i.GetTiempoTranscurrido()));
                ListBoxDatos.Items.Add("Tamaño: " + i.GetTamaño());
                ListBoxDatos.Items.Add("Marcos: " + i.GetMarcosTotales());
                ListBoxDatos.Items.Add("");
                ListBoxDatos.Refresh();
            }
        }
    }
}
