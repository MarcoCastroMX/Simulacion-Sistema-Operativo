using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Programa_8
{
    public partial class Form1 : Form
    {
        int Procesos;
        int Quantum;
        int ContadorQuantum=0;
        int ID = 0;
        int Contador;
        bool Resultado;
        bool LetraI = false;
        bool LetraE = false;
        bool LetraP = false;
        bool LetraN = false;
        bool LetraB = false;
        bool LetraT = false;
        bool LetraS = false;
        bool LetraR = false;

        List<Informacion> Lista = new List<Informacion>();
        List<Informacion> Memoria = new List<Informacion>();
        List<Informacion> Bloqueado = new List<Informacion>();
        List<Informacion> Finalizados = new List<Informacion>();
        List<Informacion> Suspendidos = new List<Informacion>();
        List<Marco> ListaMarcos = new List<Marco>();
        public Form1()
        {
            InitializeComponent();
            for(int i = 0; i < 42; i++)
            {
                Marco m = new Marco();
                ListaMarcos.Add(m);
            }
        }
        private void BtnIniciarProcesos_Click(object sender, EventArgs e)
        {
            Resultado = int.TryParse(TxtNumProcesos.Text, out Procesos);
            if (TxtNumProcesos.Text.Equals("") || Procesos <= 0 || !Resultado)
            {
                MessageBox.Show("Campos Vacios O Numero Invalido");
                TxtNumProcesos.Text = "";
                return;
            }
            Resultado = int.TryParse(TxtQuantum.Text, out Quantum);
            if (TxtQuantum.Text.Equals("") || Quantum <= 0 || !Resultado)
            {
                MessageBox.Show("Campos Vacios O Numero Invalido");
                TxtNumProcesos.Text = "";
                return;
            }
            LbValorQuantum.Text = Quantum.ToString();
            LbValorQuantum.Refresh();
            TxtNumProcesos.Enabled = false;
            TxtNumProcesos.Text = "";
            TxtQuantum.Enabled = false;
            TxtQuantum.Text = "";
            LbNumSuspendidos.Text = "0";
            LbNumSuspendidos.Refresh();
            BtnIniciarProcesos.Enabled = false;
            LbNumNuevos.Text = Procesos.ToString();
            LbNumNuevos.Refresh();
            LbContador.Text = "0";

            int Tiempo;
            int Tamaño;
            int PrimerNumero;
            int SegundoNumero;
            string Operacion = "";
            Random rand = new Random();
            for (int i = 0; i < Procesos; i++)
            {
                Tiempo = rand.Next(8, 19);
                Tamaño = rand.Next(6, 36);
                PrimerNumero = rand.Next(0, 100000);
                int OperacionAleatoria = rand.Next(0, 4);
                if (OperacionAleatoria == 0)
                    Operacion = "+";
                if (OperacionAleatoria == 1)
                    Operacion = "-";
                if (OperacionAleatoria == 2)
                    Operacion = "*";
                if (OperacionAleatoria == 3)
                    Operacion = "/";
                if (OperacionAleatoria == 4)
                    Operacion = "Residuo";
                SegundoNumero = rand.Next(0, 100000);
                if ((Operacion.Equals("Residuo") || Operacion.Equals("/")) && SegundoNumero.Equals(0))
                    while (SegundoNumero == 0)
                        SegundoNumero = rand.Next(0, 100000);
                Informacion Obj = new Informacion(PrimerNumero, Operacion, SegundoNumero, Tiempo, ID,Tamaño);

                Lista.Add(Obj);
                ID++;
            }
            Contador = 0;
            Ejecucion();
        }
        public async void Ejecucion()
        {
            while(true)
            {
                if (Lista.Count == 0)
                    break;
                ActualizarSigNuevo();
                if (Lista[0].GetMarcosTotales()>GetMarcosVacios())
                    break;
                IngresarPaginas();
                Lista[0].SetTiempoLlegada(Contador);
                Memoria.Add(Lista[0]);
                ListBoxColaListo.Items.Add(Lista[0].ProcesoInformacion());
                LbNumNuevos.Text = (Procesos - Memoria.Count).ToString();
                ListBoxColaListo.Refresh();
                LbNumNuevos.Refresh();
                Lista.RemoveAt(0);
                Thread.Sleep(500);
            }
            while(Memoria.Count!=0)
            {
                if (Lista.Count == 0)
                {
                    LbSigNuevo.Text = "";
                    LbSigNuevo.Refresh();
                } 
                ListBoxColaListo.Items.RemoveAt(0);
                ListBoxColaListo.Refresh();
                ContadorQuantum = 0;
                foreach (Marco pag in ListaMarcos)
                {
                    if (pag.GetIdActual() == Memoria[0].GetNumero())
                        pag.SetEstado("Ejecucion");
                }
                ActualizarMarcos();
                for (int Time = Memoria[0].GetTiempoTranscurrido(); Time != Memoria[0].GetTiempo(); Time++)
                {
                    ListBoxProceso.Items.Add("Numero de Programa: " + Memoria[0].GetNumero());
                    ListBoxProceso.Items.Add("Operacion: " + Memoria[0].GetOperacion());
                    ListBoxProceso.Items.Add("Tiempo Maximo Estimado: " + Memoria[0].GetTiempo());
                    ListBoxProceso.Items.Add("Tiempo Restante: " + Memoria[0].GetTiempoRestante());
                    ListBoxProceso.Items.Add("Tiempo Transcurrido: " + Memoria[0].GetTiempoTranscurrido());
                    ListBoxProceso.Items.Add("Tiempo Transcurrido del Quantum: " + ContadorQuantum);
                    ListBoxProceso.Items.Add("Tamaño: " + Memoria[0].GetTamaño());
                    ListBoxProceso.Items.Add("Marcos: " + Memoria[0].GetMarcosTotales());
                    ListBoxProceso.Refresh();
                    await Teclazos();
                    if (LetraP)
                        while (LetraP)
                            await Teclazos();
                    if (LetraE)
                    {
                        ListBoxProceso.Items.Clear();
                        Memoria[0].SetError(true);
                        break;
                    }
                    if (LetraI)
                    {
                        ListBoxProceso.Items.Clear();
                        Memoria[0].SetInterrupcion(true);
                        break;
                    }
                    if (LetraN)
                    {
                        CrearNuevoProceso();
                        LetraN = false;
                    }
                    if (LetraB)
                    {
                        Form2 BCP = new Form2(Finalizados, Memoria, Bloqueado, Lista,Suspendidos, Contador);
                        BCP.ShowDialog();
                        while (LetraB)
                            await Teclazos();
                    }
                    if (LetraT)
                    {
                        while (LetraT)
                            await Teclazos();
                    }
                    if (LetraS)
                    {
                        if (Bloqueado.Count != 0)
                        {
                            ListBoxBloqueados.Items.RemoveAt(0);
                            ListBoxBloqueados.Refresh();
                            Suspendidos.Add(Bloqueado[0]);
                            LbNumSuspendidos.Text = Suspendidos.Count.ToString();
                            LbNumSuspendidos.Refresh();
                            ActualizarSigSuspendido();
                            foreach (Marco pag in ListaMarcos)
                            {
                                if (pag.GetIdActual() == Bloqueado[0].GetNumero())
                                {
                                    pag.SetEstado("Vacio");
                                    pag.SetIdActual(-1);
                                    pag.SetNumPaginas(0);
                                }
                            }
                            ActualizarMarcos();
                            Bloqueado.RemoveAt(0);
                            while (Lista.Count != 0)
                            {
                                ActualizarSigNuevo();
                                if (Lista[0].GetMarcosTotales() > GetMarcosVacios())
                                    break;
                                IngresarPaginas();
                                Lista[0].SetTiempoLlegada(Contador);
                                Memoria.Add(Lista[0]);
                                ListBoxColaListo.Items.Add(Lista[0].ProcesoInformacion());
                                Lista.RemoveAt(0);
                                ListBoxColaListo.Refresh();
                                int Numero = int.Parse(LbNumNuevos.Text);
                                Numero--;
                                LbNumNuevos.Text = Numero.ToString();
                                LbNumNuevos.Refresh();
                            }
                            ActualizarDocumento();
                        }
                        LetraS = false;
                    }
                    if (LetraR)
                    {
                        if (Suspendidos.Count != 0)
                        {
                            if (Suspendidos[0].GetMarcosTotales() > GetMarcosVacios())
                            {
                                ListBoxProceso.Items.Clear();
                                ListBoxProceso.Refresh();
                                LetraR = false;
                                continue;
                            }
                            Lista.Insert(0, Suspendidos[0]);
                            IngresarPaginas();
                            Lista.RemoveAt(0);
                            Memoria.Add(Suspendidos[0]);
                            ListBoxColaListo.Items.Add(Suspendidos[0].ProcesoInformacion());
                            ListBoxColaListo.Refresh();
                            Suspendidos.RemoveAt(0);
                            if (Suspendidos.Count >= 1)
                            {
                                ActualizarSigSuspendido();
                            }
                            else
                            {
                                LbSigSuspendido.Text = "";
                                LbSigSuspendido.Refresh();
                            }
                            ActualizarDocumento();
                            LbNumSuspendidos.Text = Suspendidos.Count.ToString();
                            LbNumSuspendidos.Refresh();
                            
                        }
                        LetraR = false;
                    }
                    if (Quantum == ContadorQuantum)
                    {
                        Memoria[0].SetQuantum(true);
                        ListBoxProceso.Items.Clear();
                        break;
                    }
                    Memoria[0].SetTiempoRestante(Memoria[0].GetTiempoRestante() - 1);
                    Memoria[0].SetTiempoTranscurrido(Memoria[0].GetTiempoTranscurrido() + 1);
                    if (Bloqueado.Count != 0)
                    {
                        ListBoxBloqueados.Items.Clear();
                        for(int i=0; i<Bloqueado.Count; i++)
                        {
                            int ContadorBloqueado = Bloqueado[i].GetTiempoBloqueado();
                            if (ContadorBloqueado == 8)
                            {
                                Bloqueado[i].SetTiempoBloqueado(0);
                                foreach (Marco pag in ListaMarcos)
                                {
                                    if (pag.GetIdActual() == Bloqueado[i].GetNumero())
                                        pag.SetEstado("Listo");
                                }
                                ActualizarMarcos();
                                Memoria.Add(Bloqueado[i]);
                                ListBoxColaListo.Items.Add(Bloqueado[i].ProcesoInformacion());
                                Bloqueado.Remove(Bloqueado[i]);
                                i--;
                            }
                            else
                            {
                                Bloqueado[i].SetTiempoBloqueado(ContadorBloqueado+1);
                                ListBoxBloqueados.Items.Add("ID: " + Bloqueado[i].GetNumero().ToString() + " " + "TB: " + Bloqueado[i].GetTiempoBloqueado().ToString());
                                ListBoxBloqueados.Refresh();
                            }
                        }
                    }
                    if (Memoria[0].GetRespuesta() == false)
                    {
                        Memoria[0].SetRespuesta(true);
                        Memoria[0].SetTiempoRespuesta(Contador - Memoria[0].GetTiempoLlegada());
                    }
                    Contador++;
                    ContadorQuantum++;
                    LbContador.Text = Contador.ToString();
                    LbContador.Refresh();
                    ListBoxProceso.Items.Clear();
                }
                ListBoxProceso.Refresh();
                if (Memoria[0].GetInterrupcion())
                {
                    Memoria[0].SetInterrupcion(false);
                    ListBoxBloqueados.Items.Add("ID: "+Memoria[0].GetNumero().ToString()+" "+"TB: "+Memoria[0].GetTiempoBloqueado().ToString());
                    Bloqueado.Add(Memoria[0]);
                    foreach(Marco pag in ListaMarcos)
                    {
                        if (pag.GetIdActual() == Memoria[0].GetNumero())
                            pag.SetEstado("Bloqueado");
                    }
                    ActualizarMarcos();
                    Memoria.RemoveAt(0);
                    LetraI = false;
                    ListBoxBloqueados.Refresh();
                    if(Bloqueado.Count !=0 && Memoria.Count==0)
                    {
                        while (Memoria.Count==0)
                        {
                            ListBoxBloqueados.Items.Clear();
                            for (int i = 0; i < Bloqueado.Count; i++)
                            {
                                int ContadorBloqueado = Bloqueado[i].GetTiempoBloqueado();
                                if (ContadorBloqueado == 8)
                                {
                                    Bloqueado[i].SetTiempoBloqueado(0);
                                    foreach (Marco pag in ListaMarcos)
                                    {
                                        if (pag.GetIdActual() == Bloqueado[i].GetNumero())
                                            pag.SetEstado("Listo");
                                    }
                                    ActualizarMarcos();
                                    Memoria.Add(Bloqueado[i]);
                                    ListBoxBloqueados.Refresh();
                                    ListBoxColaListo.Items.Add(Bloqueado[i].ProcesoInformacion());
                                    ListBoxColaListo.Refresh();
                                    Bloqueado.Remove(Bloqueado[i]);
                                    i--;
                                }
                                else
                                {
                                    Bloqueado[i].SetTiempoBloqueado(ContadorBloqueado + 1);
                                    ListBoxBloqueados.Items.Add("ID: " + Bloqueado[i].GetNumero().ToString() + " " + "TB: " + Bloqueado[i].GetTiempoBloqueado().ToString());
                                    ListBoxBloqueados.Refresh();
                                }
                                
                            }
                            Contador++;
                            LbContador.Text = Contador.ToString();
                            LbContador.Refresh();
                            await Teclazos();
                            if (LetraN)
                            {
                                CrearNuevoProceso();
                                LetraN = false;
                            }
                            if (LetraB)
                            {
                                Form2 BCP = new Form2(Finalizados, Memoria, Bloqueado, Lista,Suspendidos,Contador);
                                BCP.ShowDialog();
                                while (LetraB)
                                    await Teclazos();
                            }
                            if (LetraT)
                            {
                                while (LetraT)
                                    await Teclazos();
                            }
                            if (LetraS)
                            {
                                if (Bloqueado.Count != 0)
                                {
                                    ListBoxBloqueados.Items.RemoveAt(0);
                                    ListBoxBloqueados.Refresh();
                                    Suspendidos.Add(Bloqueado[0]);
                                    LbNumSuspendidos.Text = Suspendidos.Count.ToString();
                                    LbNumSuspendidos.Refresh();
                                    ActualizarSigSuspendido();
                                    foreach (Marco pag in ListaMarcos)
                                    {
                                        if (pag.GetIdActual() == Bloqueado[0].GetNumero())
                                        {
                                            pag.SetEstado("Vacio");
                                            pag.SetIdActual(-1);
                                            pag.SetNumPaginas(0);
                                        }
                                    }
                                    ActualizarMarcos();
                                    Bloqueado.RemoveAt(0);
                                    while (Lista.Count != 0)
                                    {
                                        ActualizarSigNuevo();
                                        if (Lista[0].GetMarcosTotales() > GetMarcosVacios())
                                            break;
                                        IngresarPaginas();
                                        Lista[0].SetTiempoLlegada(Contador);
                                        Memoria.Add(Lista[0]);
                                        ListBoxColaListo.Items.Add(Lista[0].ProcesoInformacion());
                                        Lista.RemoveAt(0);
                                        ListBoxColaListo.Refresh();
                                        int Numero = int.Parse(LbNumNuevos.Text);
                                        Numero--;
                                        LbNumNuevos.Text = Numero.ToString();
                                        LbNumNuevos.Refresh();
                                    }
                                    ActualizarDocumento();
                                }
                                LetraS = false;
                            }
                            if (LetraR)
                            {
                                if (Suspendidos.Count != 0)
                                {
                                    if (Suspendidos[0].GetMarcosTotales() > GetMarcosVacios())
                                    {
                                        ListBoxProceso.Items.Clear();
                                        ListBoxProceso.Refresh();
                                        LetraR = false;
                                        continue;
                                    }
                                    Lista.Insert(0, Suspendidos[0]);
                                    IngresarPaginas();
                                    Lista.RemoveAt(0);
                                    Memoria.Add(Suspendidos[0]);
                                    ListBoxColaListo.Items.Add(Suspendidos[0].ProcesoInformacion());
                                    ListBoxColaListo.Refresh();
                                    Suspendidos.RemoveAt(0);
                                    if (Suspendidos.Count >= 1)
                                    {
                                        ActualizarSigSuspendido();
                                    }
                                    else
                                    {
                                        LbSigSuspendido.Text = "";
                                        LbSigSuspendido.Refresh();
                                    }
                                    ActualizarDocumento();
                                    LbNumSuspendidos.Text = Suspendidos.Count.ToString();
                                    LbNumSuspendidos.Refresh();

                                }
                                LetraR = false;
                            }
                            if (LetraP)
                                while (LetraP)
                                    await Teclazos();
                            Thread.Sleep(500);
                        }
                    }
                    Thread.Sleep(500);
                    continue;
                }
                if (Memoria[0].GetError())
                {
                    ListBoxFinalizado.Items.Add("ID: " + Memoria[0].GetNumero().ToString() + " " + "Operacion: " + " " + Memoria[0].GetOperacion() + " " + "Resultado: ERROR");
                    Memoria[0].SetResultado(-1);
                    Memoria[0].SetError(false);
                    Procesos = Procesos - 1;
                    Memoria[0].SetTiempoFinalizacion(Contador);
                    Finalizados.Add(Memoria[0]);
                    foreach (Marco pag in ListaMarcos)
                    {
                        if (pag.GetIdActual() == Memoria[0].GetNumero())
                        {
                            pag.SetEstado("Vacio");
                            pag.SetIdActual(-1);
                            pag.SetNumPaginas(0);
                        }
                    }
                    ActualizarMarcos();
                    Memoria.Remove(Memoria[0]);
                    LetraE = false;
                    ListBoxFinalizado.Refresh();
                    while (Lista.Count != 0)
                    {
                        ActualizarSigNuevo();
                        if (Lista[0].GetMarcosTotales() > GetMarcosVacios())
                            break;
                        IngresarPaginas();
                        Lista[0].SetTiempoLlegada(Contador);
                        Memoria.Add(Lista[0]);
                        ListBoxColaListo.Items.Add(Lista[0].ProcesoInformacion());
                        Lista.RemoveAt(0);
                        ListBoxColaListo.Refresh();
                        int Numero = int.Parse(LbNumNuevos.Text);
                        Numero--;
                        LbNumNuevos.Text = Numero.ToString();
                        LbNumNuevos.Refresh();
                    }
                    if (Bloqueado.Count != 0 && Memoria.Count == 0)
                    {
                        while (Memoria.Count == 0)
                        {
                            ListBoxBloqueados.Items.Clear();
                            for (int i = 0; i < Bloqueado.Count; i++)
                            {
                                int ContadorBloqueado = Bloqueado[i].GetTiempoBloqueado();
                                if (ContadorBloqueado == 8)
                                {
                                    Bloqueado[i].SetTiempoBloqueado(0);
                                    foreach (Marco pag in ListaMarcos)
                                    {
                                        if (pag.GetIdActual() == Bloqueado[i].GetNumero())
                                            pag.SetEstado("Listo");
                                    }
                                    ActualizarMarcos();
                                    Memoria.Add(Bloqueado[i]);
                                    ListBoxBloqueados.Refresh();
                                    ListBoxColaListo.Items.Add(Bloqueado[i].ProcesoInformacion());
                                    ListBoxColaListo.Refresh();
                                    Bloqueado.Remove(Bloqueado[i]);
                                    i--;
                                }
                                else
                                {
                                    Bloqueado[i].SetTiempoBloqueado(ContadorBloqueado + 1);
                                    ListBoxBloqueados.Items.Add("ID: " + Bloqueado[i].GetNumero().ToString() + " " + "TB: " + Bloqueado[i].GetTiempoBloqueado().ToString());
                                    ListBoxBloqueados.Refresh();
                                }
                            }
                            Contador++;
                            LbContador.Text = Contador.ToString();
                            LbContador.Refresh();
                            await Teclazos();
                            if (LetraN)
                            {
                                CrearNuevoProceso();
                                LetraN = false;
                            }
                            if (LetraB)
                            {
                                Form2 BCP = new Form2(Finalizados, Memoria, Bloqueado, Lista,Suspendidos, Contador);
                                BCP.ShowDialog();
                                while (LetraB)
                                    await Teclazos();
                            }
                            if (LetraT)
                            {
                                while (LetraT)
                                    await Teclazos();
                            }
                            if (LetraS)
                            {
                                if (Bloqueado.Count != 0)
                                {
                                    ListBoxBloqueados.Items.RemoveAt(0);
                                    ListBoxBloqueados.Refresh();
                                    Suspendidos.Add(Bloqueado[0]);
                                    LbNumSuspendidos.Text = Suspendidos.Count.ToString();
                                    LbNumSuspendidos.Refresh();
                                    ActualizarSigSuspendido();
                                    foreach (Marco pag in ListaMarcos)
                                    {
                                        if (pag.GetIdActual() == Bloqueado[0].GetNumero())
                                        {
                                            pag.SetEstado("Vacio");
                                            pag.SetIdActual(-1);
                                            pag.SetNumPaginas(0);
                                        }
                                    }
                                    ActualizarMarcos();
                                    Bloqueado.RemoveAt(0);
                                    while (Lista.Count != 0)
                                    {
                                        ActualizarSigNuevo();
                                        if (Lista[0].GetMarcosTotales() > GetMarcosVacios())
                                            break;
                                        IngresarPaginas();
                                        Lista[0].SetTiempoLlegada(Contador);
                                        Memoria.Add(Lista[0]);
                                        ListBoxColaListo.Items.Add(Lista[0].ProcesoInformacion());
                                        Lista.RemoveAt(0);
                                        ListBoxColaListo.Refresh();
                                        int Numero = int.Parse(LbNumNuevos.Text);
                                        Numero--;
                                        LbNumNuevos.Text = Numero.ToString();
                                        LbNumNuevos.Refresh();
                                    }
                                    ActualizarDocumento();
                                }
                                LetraS = false;
                            }
                            if (LetraR)
                            {
                                if (Suspendidos.Count != 0)
                                {
                                    if (Suspendidos[0].GetMarcosTotales() > GetMarcosVacios())
                                    {
                                        ListBoxProceso.Items.Clear();
                                        ListBoxProceso.Refresh();
                                        LetraR = false;
                                        continue;
                                    }
                                    Lista.Insert(0, Suspendidos[0]);
                                    IngresarPaginas();
                                    Lista.RemoveAt(0);
                                    Memoria.Add(Suspendidos[0]);
                                    ListBoxColaListo.Items.Add(Suspendidos[0].ProcesoInformacion());
                                    ListBoxColaListo.Refresh();
                                    Suspendidos.RemoveAt(0);
                                    if (Suspendidos.Count >= 1)
                                    {
                                        ActualizarSigSuspendido();
                                    }
                                    else
                                    {
                                        LbSigSuspendido.Text = "";
                                        LbSigSuspendido.Refresh();
                                    }
                                    ActualizarDocumento();
                                    LbNumSuspendidos.Text = Suspendidos.Count.ToString();
                                    LbNumSuspendidos.Refresh();

                                }
                                LetraR = false;
                            }
                            if (LetraP)
                                while (LetraP)
                                    await Teclazos();
                            Thread.Sleep(500);
                        }
                    }
                    if (Suspendidos.Count != 0 && Memoria.Count == 0)
                    {
                        while (Memoria.Count == 0)
                        {
                            Contador++;
                            LbContador.Text = Contador.ToString();
                            LbContador.Refresh();
                            await Teclazos();
                            if (LetraN)
                            {
                                CrearNuevoProceso();
                                LetraN = false;
                            }
                            if (LetraB)
                            {
                                Form2 BCP = new Form2(Finalizados, Memoria, Bloqueado, Lista,Suspendidos,Contador);
                                BCP.ShowDialog();
                                while (LetraB)
                                    await Teclazos();
                            }
                            if (LetraT)
                            {
                                while (LetraT)
                                    await Teclazos();
                            }
                            if (LetraR)
                            {
                                if (Suspendidos.Count != 0)
                                {
                                    if (Suspendidos[0].GetMarcosTotales() > GetMarcosVacios())
                                    {
                                        ListBoxProceso.Items.Clear();
                                        ListBoxProceso.Refresh();
                                        LetraR = false;
                                        continue;
                                    }
                                    Lista.Insert(0, Suspendidos[0]);
                                    IngresarPaginas();
                                    Lista.RemoveAt(0);
                                    Memoria.Add(Suspendidos[0]);
                                    ListBoxColaListo.Items.Add(Suspendidos[0].ProcesoInformacion());
                                    ListBoxColaListo.Refresh();
                                    Suspendidos.RemoveAt(0);
                                    if (Suspendidos.Count >= 1)
                                    {
                                        ActualizarSigSuspendido();
                                    }
                                    else
                                    {
                                        LbSigSuspendido.Text = "";
                                        LbSigSuspendido.Refresh();
                                    }
                                    ActualizarDocumento();
                                    LbNumSuspendidos.Text = Suspendidos.Count.ToString();
                                    LbNumSuspendidos.Refresh();

                                }
                                LetraR = false;
                            }
                            if (LetraP)
                                while (LetraP)
                                    await Teclazos();
                            Thread.Sleep(500);
                        }
                    }
                    Thread.Sleep(500);
                    continue;
                }
                if (Memoria[0].GetQuantum())
                {
                    Memoria[0].SetQuantum(false);
                    foreach(Marco pag in ListaMarcos)
                    {
                        if (pag.GetIdActual() == Memoria[0].GetNumero())
                            pag.SetEstado("Listo");
                    }
                    ActualizarMarcos();
                    Memoria.Add(Memoria[0]);
                    ListBoxColaListo.Items.Add(Memoria[0].ProcesoInformacion());
                    ListBoxColaListo.Refresh();
                    Memoria.RemoveAt(0);
                    ContadorQuantum = 0;
                    Thread.Sleep(500);
                    continue;
                }
                int ResultadoOperacion = 0;
                int Primer = Memoria[0].GetPrimerNumero();
                int Segundo = Memoria[0].GetSegundoNumero();
                string Operando = Memoria[0].GetOperando();
                if (Operando == "+")
                    ResultadoOperacion = Primer + Segundo;
                if (Operando == "-")
                    ResultadoOperacion = Primer - Segundo;
                if (Operando == "*")
                    ResultadoOperacion = Primer * Segundo;
                if (Operando == "/")
                    ResultadoOperacion = Primer / Segundo;
                if (Operando == "Residuo")
                    ResultadoOperacion = Primer % Segundo;
                ListBoxFinalizado.Items.Add("ID: " + Memoria[0].GetNumero().ToString() + " " + "Operacion: " + " " + Memoria[0].GetOperacion() + " " + "Resultado: " + ResultadoOperacion.ToString());
                Memoria[0].SetResultado(ResultadoOperacion);
                ListBoxFinalizado.Refresh();
                Procesos = Procesos - 1;
                Memoria[0].SetTiempoFinalizacion(Contador);
                Finalizados.Add(Memoria[0]);
                foreach (Marco pag in ListaMarcos)
                {
                    if (pag.GetIdActual() == Memoria[0].GetNumero())
                    {
                        pag.SetEstado("Vacio");
                        pag.SetIdActual(-1);
                        pag.SetNumPaginas(0);
                    }
                }
                ActualizarMarcos();
                Memoria.Remove(Memoria[0]);
                while(Lista.Count != 0)
                {
                    ActualizarSigNuevo();
                    if (Lista[0].GetMarcosTotales() > GetMarcosVacios())
                        break;
                    IngresarPaginas();
                    Lista[0].SetTiempoLlegada(Contador);
                    Memoria.Add(Lista[0]);
                    ListBoxColaListo.Items.Add(Lista[0].ProcesoInformacion());
                    Lista.RemoveAt(0);
                    ListBoxColaListo.Refresh();
                    int Numero = int.Parse(LbNumNuevos.Text);
                    Numero--;
                    LbNumNuevos.Text = Numero.ToString();
                    LbNumNuevos.Refresh();
                }
                if (Bloqueado.Count != 0 && Memoria.Count == 0)
                {
                    while (Memoria.Count == 0)
                    {
                        ListBoxBloqueados.Items.Clear();
                        for (int i = 0; i < Bloqueado.Count; i++)
                        {
                            int ContadorBloqueado = Bloqueado[i].GetTiempoBloqueado();
                            if (ContadorBloqueado == 8)
                            {
                                Bloqueado[i].SetTiempoBloqueado(0);
                                foreach (Marco pag in ListaMarcos)
                                {
                                    if (pag.GetIdActual() == Bloqueado[i].GetNumero())
                                        pag.SetEstado("Listo");
                                }
                                ActualizarMarcos();
                                Memoria.Add(Bloqueado[i]);
                                ListBoxColaListo.Items.Add(Bloqueado[i].ProcesoInformacion());
                                ListBoxColaListo.Refresh();
                                Bloqueado.Remove(Bloqueado[i]);
                                i--;
                            }
                            else
                            {
                                ListBoxBloqueados.Items.Add("ID: " + Bloqueado[i].GetNumero().ToString() + " " + "TB: " + Bloqueado[i].GetTiempoBloqueado().ToString());
                                ListBoxBloqueados.Refresh();
                                Bloqueado[i].SetTiempoBloqueado(ContadorBloqueado + 1);
                            }
                        }
                        Contador++;
                        LbContador.Text = Contador.ToString();
                        LbContador.Refresh();
                        await Teclazos();
                        if (LetraN)
                        {
                            CrearNuevoProceso();
                            LetraN = false;
                        }
                        if (LetraB)
                        {
                            Form2 BCP = new Form2(Finalizados, Memoria, Bloqueado, Lista,Suspendidos,Contador);
                            BCP.ShowDialog();
                            while (LetraB)
                                await Teclazos();
                        }
                        if (LetraT)
                        {
                            while (LetraT)
                                await Teclazos();
                        }
                        if (LetraS)
                        {
                            if (Bloqueado.Count != 0)
                            {
                                ListBoxBloqueados.Items.RemoveAt(0);
                                ListBoxBloqueados.Refresh();
                                Suspendidos.Add(Bloqueado[0]);
                                LbNumSuspendidos.Text = Suspendidos.Count.ToString();
                                LbNumSuspendidos.Refresh();
                                ActualizarSigSuspendido();
                                foreach (Marco pag in ListaMarcos)
                                {
                                    if (pag.GetIdActual() == Bloqueado[0].GetNumero())
                                    {
                                        pag.SetEstado("Vacio");
                                        pag.SetIdActual(-1);
                                        pag.SetNumPaginas(0);
                                    }
                                }
                                ActualizarMarcos();
                                Bloqueado.RemoveAt(0);
                                while (Lista.Count != 0)
                                {
                                    ActualizarSigNuevo();
                                    if (Lista[0].GetMarcosTotales() > GetMarcosVacios())
                                        break;
                                    IngresarPaginas();
                                    Lista[0].SetTiempoLlegada(Contador);
                                    Memoria.Add(Lista[0]);
                                    ListBoxColaListo.Items.Add(Lista[0].ProcesoInformacion());
                                    Lista.RemoveAt(0);
                                    ListBoxColaListo.Refresh();
                                    int Numero = int.Parse(LbNumNuevos.Text);
                                    Numero--;
                                    LbNumNuevos.Text = Numero.ToString();
                                    LbNumNuevos.Refresh();
                                }
                                ActualizarDocumento();
                            }
                            LetraS = false;
                        }
                        if (LetraR)
                        {
                            if (Suspendidos.Count != 0)
                            {
                                if (Suspendidos[0].GetMarcosTotales() > GetMarcosVacios())
                                {
                                    ListBoxProceso.Items.Clear();
                                    ListBoxProceso.Refresh();
                                    LetraR = false;
                                    continue;
                                }
                                Lista.Insert(0, Suspendidos[0]);
                                IngresarPaginas();
                                Lista.RemoveAt(0);
                                Memoria.Add(Suspendidos[0]);
                                ListBoxColaListo.Items.Add(Suspendidos[0].ProcesoInformacion());
                                ListBoxColaListo.Refresh();
                                Suspendidos.RemoveAt(0);
                                if (Suspendidos.Count >= 1)
                                {
                                    ActualizarSigSuspendido();
                                }
                                else
                                {
                                    LbSigSuspendido.Text = "";
                                    LbSigSuspendido.Refresh();
                                }
                                ActualizarDocumento();
                                LbNumSuspendidos.Text = Suspendidos.Count.ToString();
                                LbNumSuspendidos.Refresh();

                            }
                            LetraR = false;
                        }
                        if (LetraP)
                            while (LetraP)
                                await Teclazos();
                        Thread.Sleep(500);
                    }
                }
                if (Suspendidos.Count != 0 && Memoria.Count == 0)
                {
                    while (Memoria.Count == 0)
                    {
                        Contador++;
                        LbContador.Text = Contador.ToString();
                        LbContador.Refresh();
                        await Teclazos();
                        if (LetraN)
                        {
                            CrearNuevoProceso();
                            LetraN = false;
                        }
                        if (LetraB)
                        {
                            Form2 BCP = new Form2(Finalizados, Memoria, Bloqueado, Lista,Suspendidos,Contador);
                            BCP.ShowDialog();
                            while (LetraB)
                                await Teclazos();
                        }
                        if (LetraT)
                        {
                            while (LetraT)
                                await Teclazos();
                        }
                        if (LetraR)
                        {
                            if (Suspendidos.Count != 0)
                            {
                                if (Suspendidos[0].GetMarcosTotales() > GetMarcosVacios())
                                {
                                    ListBoxProceso.Items.Clear();
                                    ListBoxProceso.Refresh();
                                    LetraR = false;
                                    continue;
                                }
                                Lista.Insert(0, Suspendidos[0]);
                                IngresarPaginas();
                                Lista.RemoveAt(0);
                                Memoria.Add(Suspendidos[0]);
                                ListBoxColaListo.Items.Add(Suspendidos[0].ProcesoInformacion());
                                ListBoxColaListo.Refresh();
                                Suspendidos.RemoveAt(0);
                                if (Suspendidos.Count >= 1)
                                {
                                    ActualizarSigSuspendido();
                                }
                                else
                                {
                                    LbSigSuspendido.Text = "";
                                    LbSigSuspendido.Refresh();
                                }
                                ActualizarDocumento();
                                LbNumSuspendidos.Text = Suspendidos.Count.ToString();
                                LbNumSuspendidos.Refresh();

                            }
                            LetraR = false;
                        }
                        if (LetraP)
                            while (LetraP)
                                await Teclazos();
                        Thread.Sleep(500);
                    }
                }
                Thread.Sleep(500);
            }
            Form2 Ventana = new Form2(Finalizados,Memoria,Bloqueado,Lista,Suspendidos,Contador);
            Ventana.ShowDialog();
        }

        private  async Task Teclazos()
        {
            await Task.Delay(750);
        }

        private void listBoxLoteEjecutandose_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'P')
            {
                LetraP = true;
            }
            if (e.KeyChar == 'C' && LetraP == true)
            {
                LetraP = false;
            }
            if(e.KeyChar == 'C' && LetraB == true)
            {
                LetraB = false;
            }
            if(e.KeyChar == 'C' && LetraT == true)
            {
                LetraT = false;
            }
            if (e.KeyChar == 'E' && LetraP == false)
            {
                LetraE = true;
            }
            if (e.KeyChar == 'I' && LetraP == false)
            {
                LetraI = true;
            }
            if(e.KeyChar == 'N' && LetraP == false)
            {
                LetraN = true;
            }
            if(e.KeyChar == 'B' && LetraP == false)
            {
                LetraB = true;
            }
            if(e.KeyChar == 'T' && LetraP == false)
            {
                LetraT = true;
            }
            if(e.KeyChar=='S' && LetraP== false)
            {
                LetraS = true;
            }
            if (e.KeyChar == 'R' && LetraP == false)
            {
                LetraR = true;
            }
        }

        private void CrearNuevoProceso()
        {
            int Tiempo;
            int Tamaño;
            int PrimerNumero;
            int SegundoNumero;
            string Operacion = "";
            Random rand = new Random();
            Tiempo = rand.Next(8, 18);
            Tamaño = rand.Next(5, 36);
            PrimerNumero = rand.Next(0, 100000);
            int OperacionAleatoria = rand.Next(0, 4);
            if (OperacionAleatoria == 0)
                Operacion = "+";
            if (OperacionAleatoria == 1)
                Operacion = "-";
            if (OperacionAleatoria == 2)
                Operacion = "*";
            if (OperacionAleatoria == 3)
                Operacion = "/";
            if (OperacionAleatoria == 4)
                Operacion = "Residuo";
            SegundoNumero = rand.Next(0, 100000);
            if ((Operacion.Equals("Residuo") || Operacion.Equals("/")) && SegundoNumero.Equals(0))
                while (SegundoNumero == 0)
                    SegundoNumero = rand.Next(0, 100000);
            Informacion Obj = new Informacion(PrimerNumero, Operacion, SegundoNumero, Tiempo, ID, Tamaño);
            Obj.SetTiempoLlegada(Contador);
            ID++;
            if (Obj.GetMarcosTotales()>GetMarcosVacios())
            {
                Lista.Add(Obj);
                ActualizarSigNuevo();
                int NumeroNuevos = int.Parse(LbNumNuevos.Text);
                NumeroNuevos++;
                LbNumNuevos.Text = NumeroNuevos.ToString();
                LbNumNuevos.Refresh();
            }
            else
            {
                Memoria.Add(Obj);
                Lista.Add(Obj);
                ActualizarSigNuevo();
                IngresarPaginas();
                Lista.Remove(Obj);
                ListBoxColaListo.Items.Add(Obj.ProcesoInformacion());
                ListBoxColaListo.Refresh();
            }
        }

        public void ActualizarSigNuevo()
        {
            LbSigNuevo.Text = "ID: " + Lista[0].GetNumero().ToString() + " " + "Tamaño: " + Lista[0].GetTamaño();
            LbSigNuevo.Refresh();
        }

        public void ActualizarSigSuspendido()
        {
            LbSigSuspendido.Text = "ID: " + Suspendidos[0].GetNumero().ToString() + " " + "Tamaño: " + Suspendidos[0].GetTamaño();
            LbSigSuspendido.Refresh();
        }

        public int GetMarcosVacios()
        {
            int i = 0;
            foreach(Marco a in ListaMarcos){
                if (a.GetIdActual() == -1)
                    i++;
            }
            return i;
        }

        public void IngresarPaginas()
        {
            int Marco = Lista[0].GetMarcosTotales();
            int Residuo = Lista[0].GetResiduo();
            int i = 0;
            foreach(Marco obj in ListaMarcos)
            {
                if (i == Marco)
                    break;
                if (obj.GetIdActual() == -1)
                {
                    if(i+1==Marco && Residuo != 0)
                    {
                        obj.SetIdActual(Lista[0].GetNumero());
                        obj.SetNumPaginas(Residuo);
                        obj.SetEstado("Listo");
                    }
                    else
                    {
                        obj.SetIdActual(Lista[0].GetNumero());
                        obj.SetNumPaginas(4);
                        obj.SetEstado("Listo");
                    }
                    i++;
                }
            }
            ActualizarMarcos();
        }
        
        public void ActualizarDocumento()
        {
            TextWriter Escribe = new StreamWriter("Suspendidos.txt");
            foreach(Informacion i in Suspendidos)
            {
                Escribe.WriteLine("ID: " + i.GetNumero().ToString() + " " + "Tamaño: " + i.GetTamaño());
            }
            Escribe.Close();
        }
        public void ActualizarMarcos()
        {
            int Indice = 4;
            foreach(Marco pag in ListaMarcos)
            {
                if (Indice == 4)
                {
                    Marco4.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco4.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco4.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco4.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco4.BackColor = Color.Red;
                    Marco4.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 5)
                {
                    Marco5.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco5.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco5.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco5.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco5.BackColor = Color.Red;
                    Marco5.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 6)
                {
                    Marco6.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco6.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco6.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco6.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco6.BackColor = Color.Red;
                    Marco6.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 7)
                {
                    Marco7.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco7.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco7.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco7.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco7.BackColor = Color.Red;
                    Marco7.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 8)
                {
                    Marco8.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco8.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco8.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco8.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco8.BackColor = Color.Red;
                    Marco8.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 9)
                {
                    Marco9.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco9.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco9.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco9.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco9.BackColor = Color.Red;
                    Marco9.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 10)
                {
                    Marco10.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco10.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco10.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco10.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco10.BackColor = Color.Red;
                    Marco10.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 11)
                {
                    Marco11.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco11.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco11.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco11.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco11.BackColor = Color.Red;
                    Marco11.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 12)
                {
                    Marco12.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco12.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco12.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco12.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco12.BackColor = Color.Red;
                    Marco12.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 13)
                {
                    Marco13.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco13.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco13.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco13.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco13.BackColor = Color.Red;
                    Marco13.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 14)
                {
                    Marco14.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco14.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco14.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco14.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco14.BackColor = Color.Red;
                    Marco14.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 15)
                {
                    Marco15.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco15.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco15.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco15.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco15.BackColor = Color.Red;
                    Marco15.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 16)
                {
                    Marco16.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco16.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco16.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco16.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco16.BackColor = Color.Red;
                    Marco16.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 17)
                {
                    Marco17.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco17.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco17.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco17.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco17.BackColor = Color.Red;
                    Marco17.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 18)
                {
                    Marco18.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco18.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco18.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco18.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco18.BackColor = Color.Red;
                    Marco18.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 19)
                {
                    Marco19.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco19.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco19.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco19.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco19.BackColor = Color.Red;
                    Marco19.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 20)
                {
                    Marco20.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco20.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco20.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco20.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco20.BackColor = Color.Red;
                    Marco20.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 21)
                {
                    Marco21.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco21.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco21.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco21.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco21.BackColor = Color.Red;
                    Marco21.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 22)
                {
                    Marco22.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco22.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco22.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco22.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco22.BackColor = Color.Red;
                    Marco22.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 23)
                {
                    Marco23.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco23.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco23.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco23.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco23.BackColor = Color.Red;
                    Marco23.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 24)
                {
                    Marco24.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco24.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco24.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco24.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco24.BackColor = Color.Red;
                    Marco24.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 25)
                {
                    Marco25.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco25.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco25.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco25.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco25.BackColor = Color.Red;
                    Marco25.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 26)
                {
                    Marco26.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco26.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco26.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco26.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco26.BackColor = Color.Red;
                    Marco26.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 27)
                {
                    Marco27.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco27.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco27.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco27.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco27.BackColor = Color.Red;
                    Marco27.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 28)
                {
                    Marco28.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco28.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco28.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco28.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco28.BackColor = Color.Red;
                    Marco28.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 29)
                {
                    Marco29.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco29.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco29.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco29.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco29.BackColor = Color.Red;
                    Marco29.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 30)
                {
                    Marco30.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco30.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco30.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco30.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco30.BackColor = Color.Red;
                    Marco30.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 31)
                {
                    Marco31.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco31.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco31.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco31.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco31.BackColor = Color.Red;
                    Marco31.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 32)
                {
                    Marco32.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco32.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco32.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco32.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco32.BackColor = Color.Red;
                    Marco32.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 33)
                {
                    Marco33.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco33.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco33.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco33.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco33.BackColor = Color.Red;
                    Marco33.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 34)
                {
                    Marco34.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco34.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco34.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco34.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco34.BackColor = Color.Red;
                    Marco34.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 35)
                {
                    Marco35.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco35.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco35.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco35.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco35.BackColor = Color.Red;
                    Marco35.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 36)
                {
                    Marco36.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco36.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco36.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco36.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco36.BackColor = Color.Red;
                    Marco36.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 37)
                {
                    Marco37.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco37.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco37.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco37.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco37.BackColor = Color.Red;
                    Marco37.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 38)
                {
                    Marco38.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco38.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco38.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco38.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco38.BackColor = Color.Red;
                    Marco38.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 39)
                {
                    Marco39.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco39.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco39.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco39.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco39.BackColor = Color.Red;
                    Marco39.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 40)
                {
                    Marco40.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco40.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco40.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco40.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco40.BackColor = Color.Red;
                    Marco40.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 41)
                {
                    Marco41.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco41.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco41.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco41.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco41.BackColor = Color.Red;
                    Marco41.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 42)
                {
                    Marco42.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco42.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco42.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco42.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco42.BackColor = Color.Red;
                    Marco42.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 43)
                {
                    Marco43.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco43.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco43.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco43.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco43.BackColor = Color.Red;
                    Marco43.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 44)
                {
                    Marco44.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco44.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco44.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco44.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco44.BackColor = Color.Red;
                    Marco44.Refresh();
                    Indice++;
                    continue;
                }
                if (Indice == 45)
                {
                    Marco45.Text = pag.GetNumPaginas().ToString() + "/4" + " " + pag.GetIdActual().ToString();
                    if (pag.GetEstado() == "Vacio")
                        Marco45.BackColor = Color.White;
                    if (pag.GetEstado() == "Listo")
                        Marco45.BackColor = Color.RoyalBlue;
                    if (pag.GetEstado() == "Bloqueado")
                        Marco45.BackColor = Color.Purple;
                    if (pag.GetEstado() == "Ejecucion")
                        Marco45.BackColor = Color.Red;
                    Marco45.Refresh();
                    Indice++;
                    continue;
                }
            }
        }
    }
}
