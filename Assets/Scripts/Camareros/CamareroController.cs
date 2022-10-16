using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CamareroController : MonoBehaviour
{
    public NavMeshAgent navMesh;
    private MaquinaDeEstados fsm;
    private AudioSource audioSource;

    public GameObject player;
    public GameObject entradaCocina;
    private GameObject clienteAlertado;
    public GameObject FloatingText;


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
        //targetLocation = GameObject.FindGameObjectWithTag("Cliente").transform.position;
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
                MostrarFloatingText("?");
                TimerGlobal.globalTime = 0;
                TimerGlobal.timerActivado = true;
                clienteController.floatingText.SetActive(true);
                clienteController.AlertaCliente = false;
            }
        }


        if (EstaJugadorEnRadio(radioDeteccion) && fsm.estadoActual != MaquinaDeEstados.Estado.persecucion)
        {
            audioSource.Play(); //STOP!!
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

    private void MostrarFloatingText(string text)
    {
        FloatingText.GetComponent<TextMesh>().text = text;
        FloatingText.SetActive(true);
        //Instantiate(FloatingText, transform.position, Quaternion.identity, transform);
    }

    private void OcultarFloatingText()
    {
        FloatingText.SetActive(false);
    }

    private void Patrullar()
    {
        //Tiene que ir de la entrada de la cocina a la mesa que esté disponible (sin camareros)


        if (Vector3.Distance(transform.position, mesaActual.transform.position) < 1 && irMesas)
        {
            //Está cerca de mesa, tiene que ir a cocina
            irMesas = false;
            mesaActual.tag = "MesaOcupada";
            //mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
            //DebugConsultarArrayMesasLibres(mesas);
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
            //StartCoroutine(SeekObjectWithRetard(mesaActual));
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
            camarero.GetComponent<CamareroController>().MostrarFloatingText("?");
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
            clienteAlertado.GetComponent<ClienteController>().floatingText.SetActive(false);

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
                camcontrol.OcultarFloatingText();

                camcontrol.fsm.ActivarEstado(MaquinaDeEstados.Estado.patrulla);
            }
        }

    }



    private void Perseguir()
    {
        MostrarFloatingText("!");
        navMesh.SetDestination(player.transform.position);
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
        float x, y, a, b;
        for (int i = 0; i < arrayNpc.Length; i++)
        {
            x = arrayNpc[i].transform.position.x;
            y = arrayNpc[i].transform.position.y;
            a = transform.position.x;
            b = transform.position.y;
            if ((x - a)* (x - a) + (y - b) * (y - b) <= radio*radio)
            {
                Cercanos.Add(arrayNpc[i]);
            }
        }

        return Cercanos;
    }

    private bool EstaJugadorEnRadio(int radio)
    {
        float x, y, a, b;
        x = player.transform.position.x;
        y = player.transform.position.y;
        a = transform.position.x;
        b = transform.position.y;
        if ((x - a) * (x - a) + (y - b) * (y - b) <= radio * radio)
        {
            return true;
        }
        return false;
    }

    private void DebugConsultarArrayMesasLibres(GameObject[] mesas)
    {
        string mesasLibres = "Mesas libres: ";
        for(int i = 0; i<mesas.Length; i++)
        {
            mesasLibres += mesas[i].name + ", ";
        }
        Debug.Log(mesasLibres);
    }
}
