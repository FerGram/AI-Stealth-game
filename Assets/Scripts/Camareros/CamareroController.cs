using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CamareroController : MonoBehaviour
{
    public NavMeshAgent navMesh;
    private MaquinaDeEstados fsm;
    private AudioSource audioSource;

    public GameObject jugador;
    public GameObject entradaCocina;
    private GameObject clienteAlertado;
    public GameObject textoFlotante;


    //Patrulla
    private bool irMesas;
    private bool irCocina;
    private GameObject[] mesas;
    private GameObject mesaActual;

    //Alerta
    public int radioDeteccion;
    public float segundosDeAlerta = 5;
    private GameObject[] camareros;
    private GameObject[] clientes;


    void Start()
    {
        mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
        camareros = GameObject.FindGameObjectsWithTag("Camarero");
        clientes = GameObject.FindGameObjectsWithTag("Cliente");
        fsm = GetComponent<MaquinaDeEstados>();
        audioSource = GetComponent<AudioSource>();

        int randomNumber = Random.Range(0, mesas.Length);
        mesaActual = mesas[randomNumber];
        if(randomNumber == 0 || randomNumber == 1)
        {
            irMesas = false;
            irCocina = true;
            navMesh.SetDestination(entradaCocina.transform.position);
        }
        else
        {
            irMesas = true;
            irCocina = false;
            navMesh.SetDestination(mesaActual.transform.position);
        }

        
    }

    void Update()
    {
        foreach (GameObject cliente in clientes)
        {
            ClienteController clienteController = cliente.GetComponent<ClienteController>();
            if (clienteController.AlertaCliente && fsm.estadoActual != MaquinaDeEstados.Estado.persecucion)
            {
                clienteAlertado = cliente;
                fsm.ActivarEstado(MaquinaDeEstados.Estado.alerta);
                MostrarTextoFlotante("?");
                TimerGlobal.globalTime = 0;
                TimerGlobal.timerActivado = true;
                clienteController.textoFlotanteCliente.SetActive(true);
                clienteController.AlertaCliente = false;
            }
        }


        if (EstaJugadorEnRadio(radioDeteccion) && fsm.estadoActual != MaquinaDeEstados.Estado.persecucion)
        {
            audioSource.Play();
            fsm.ActivarEstado(MaquinaDeEstados.Estado.persecucion);
        }


        if (fsm.estadoActual == MaquinaDeEstados.Estado.patrulla)
        {          
            Patrullar();
        }
        else if (fsm.estadoActual == MaquinaDeEstados.Estado.alerta)
        {
            
            Alerta();
        }

        else
        {
            Perseguir();
        }
    }

    private void MostrarTextoFlotante(string text)
    {
        textoFlotante.GetComponent<TextMesh>().text = text;
        textoFlotante.SetActive(true);
    }

    private void OcultarTextoFlotante()
    {
        textoFlotante.SetActive(false);
    }

    private void Patrullar()
    {
        //Tiene que ir de la entrada de la cocina a la mesa que esté disponible (sin camareros)


        if (Vector3.Distance(transform.position, mesaActual.transform.position) < 1 && irMesas)
        {
            //Está cerca de mesa, tiene que ir a cocina
            irMesas = false;
            mesaActual.tag = "MesaOcupada";
            irCocina = true;
            StartCoroutine(SeekObjectWithRetard(entradaCocina));

        }
        
        else if (Vector3.Distance(transform.position, entradaCocina.transform.position) < 1 && irCocina)
        {
            //Está cerca de la cocina, tiene que ir a una mesa
            irCocina = false;
            irMesas = true;
            mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
            mesaActual = mesas[Random.Range(0, mesas.Length)];
            Seek(mesaActual.transform.position);
        }

    }
    private void Alerta()
    {
        //Buscar camareros en un rango determinado de un circulo de vision
        //Ir a zona donde se ha producido la alerta
        //Si en 5 segundos no detecta mapache en circulo de vision: estado patrulla. Sino, estado perseguir

        List<GameObject> camarerosCercanos = BuscarNPCSEnRadio(radioDeteccion, camareros);
        foreach (GameObject camarero in camarerosCercanos)
        {
            camarero.GetComponent<CamareroController>().fsm.ActivarEstado(MaquinaDeEstados.Estado.alerta);
            camarero.GetComponent<CamareroController>().MostrarTextoFlotante("?");
            camarero.GetComponent<CamareroController>().clienteAlertado = clienteAlertado;
        }
        Seek(clienteAlertado.transform.position);


        if (EstaJugadorEnRadio(radioDeteccion))
        {
            fsm.ActivarEstado(MaquinaDeEstados.Estado.persecucion);
        }
            

        if (TimerGlobal.globalTime >= segundosDeAlerta)
        {
            fsm.ActivarEstado(MaquinaDeEstados.Estado.patrulla);
            TimerGlobal.globalTime = 0;
            TimerGlobal.timerActivado = false;
            clienteAlertado.GetComponent<ClienteController>().textoFlotanteCliente.SetActive(false);

            foreach (GameObject camarero in camareros)
            {
                CamareroController camcontrol = camarero.GetComponent<CamareroController>();
                int randomNumber = Random.Range(0, camcontrol.mesas.Length);
                camcontrol.mesaActual = camcontrol.mesas[randomNumber];
                if (randomNumber == 0 || randomNumber == 1)
                {
                    camcontrol.irMesas = false;
                    camcontrol.irCocina = true;
                    camcontrol.navMesh.SetDestination(entradaCocina.transform.position);
                }
                else
                {
                    camcontrol.irMesas = true;
                    camcontrol.irCocina = false;
                    camcontrol.navMesh.SetDestination(mesaActual.transform.position);
                }
                camcontrol.OcultarTextoFlotante();

                camcontrol.fsm.ActivarEstado(MaquinaDeEstados.Estado.patrulla);
            }
        }

    }



    private void Perseguir()
    {
        MostrarTextoFlotante("!");
        navMesh.SetDestination(jugador.transform.position);
    }

    IEnumerator SeekObjectWithRetard(GameObject objetivo)
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        
        if (irCocina)
        {
            mesaActual.tag = "MesaLibre";
            //mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
            
            //mesaActual = mesas[Random.Range(0, mesas.Length)];
        }
        navMesh.SetDestination(objetivo.transform.position);
    }

    public void Seek(Vector3 location)
    {
        navMesh.SetDestination(location);
    }

    private List<GameObject> BuscarNPCSEnRadio(int radio, GameObject[] arrayNpc)
    {
        List<GameObject> Cercanos = new List<GameObject>();
        //Para determinar si un punto (x, y) pertenece a una
        //circunferencia con centro (a, b) y radio r, se prueba que
        //la distancia entre (x, y) y el centro (a, b) es ( x - a ) 2 + ( y - b ) 2 = r2 .
        //float x, y, a, b;
        for (int i = 0; i < arrayNpc.Length; i++)
        {
            /*x = arrayNpc[i].transform.position.x;
            y = arrayNpc[i].transform.position.y;
            a = transform.position.x;
            b = transform.position.y;
            if ((x - a)* (x - a) + (y - b) * (y - b) <= radio*radio)
            {
                Cercanos.Add(arrayNpc[i]);
            }*/

            if (Vector3.Distance(arrayNpc[i].transform.position, transform.position) < radio) Cercanos.Add(arrayNpc[i]);
        }

        return Cercanos;
    }

    private bool EstaJugadorEnRadio(int radio)
    {
        /*float x, y, a, b;
        a = jugador.transform.position.x;
        b = jugador.transform.position.y;
        x = transform.position.x;
        y = transform.position.y;
        if ((x - a) * (x - a) + (y - b) * (y - b) <= radio * radio)
        {
            return true;
        }
        return false;*/

        if (Vector3.Distance(jugador.transform.position, transform.position) < radio) return true;
        else return false;
    }
}
