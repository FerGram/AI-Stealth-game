//#define DEBUG_INFO

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CamareroController : Agent
{
    private MaquinaDeEstados fsm;
    private AudioSource audioSource;

    [SerializeField] GameObject _entradaCocina;
    [SerializeField] GameObject jugador;
    [SerializeField] GameObject textoFlotante;
    [Space]
    [Header("Navigation")]
    [SerializeField] Pathfinder _pathfinder;
    [SerializeField] float _movementSpeed = 100f;     
    [SerializeField] float _rotationSpeed = 180f;    //Translates to degrees per second
    [SerializeField] float _stoppingNodeDistance = 0.01f;

    private GameObject clienteAlertado;

    //Patrulla
    private bool _irMesas;
    private bool _irCocina;
    private GameObject[] _mesas;
    private GameObject _mesaActual;
    private Rigidbody _rb;

    //Alerta
    [Space]
    [Header("Alerta")]
    public int radioDeteccion;
    public float segundosDeAlerta = 5;
    private GameObject[] camareros;
    private GameObject[] clientes;

    private bool _isPursuing = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }


    void Start()
    {
        _mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
        camareros = GameObject.FindGameObjectsWithTag("Camarero");
        clientes = GameObject.FindGameObjectsWithTag("Cliente");
        fsm = GetComponent<MaquinaDeEstados>();
        audioSource = GetComponent<AudioSource>();

        int randomNumber = Random.Range(0, _mesas.Length);
        _mesaActual = _mesas[randomNumber];
        if(randomNumber == 0 || randomNumber == 1)
        {
            _irMesas = false;
            _irCocina = true;
            StartNavigation(_rb, _pathfinder.GetPath(_rb, _entradaCocina.transform), _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
        }
        else
        {
            _irMesas = true;
            _irCocina = false;
            StartNavigation(_rb, _pathfinder.GetPath(_rb, _mesaActual.transform), _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
        }

        
    }

    void Update()
    {



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
        //Tiene que ir de la entrada de la cocina a la mesa que est? disponible (sin camareros)


        if (Vector3.Distance(transform.position, _mesaActual.transform.position) < _stoppingNodeDistance && _irMesas)
        {
            //Est? cerca de mesa, tiene que ir a cocina
            _irMesas = false;
            _mesaActual.tag = "MesaLibre";
            _irCocina = true;
            StartCoroutine(SeekObjectWithRetard(_entradaCocina));
            //SeekObjectWithRetard(_entradaCocina);
        }
        
        else if (Vector3.Distance(transform.position, _entradaCocina.transform.position) < _stoppingNodeDistance && _irCocina)
        {
            //Est? cerca de la cocina, tiene que ir a una mesa
            _irCocina = false;
            _irMesas = true;
            _mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
            _mesaActual = _mesas[Random.Range(0, _mesas.Length)];
            _mesaActual.tag = "MesaOcupada";
            Debug.Log("Voy a la mesa");
            StartCoroutine(SeekObjectWithRetard(_mesaActual));
        }

        //Alerta cliente
        foreach (GameObject cliente in clientes)
        {
            ClienteController clienteController = cliente.GetComponent<ClienteController>();
            if (clienteController.AlertaCliente)
            {
                clienteAlertado = cliente;
                Seek(clienteAlertado.transform.GetChild(0).gameObject);
                fsm.ActivarEstado(MaquinaDeEstados.Estado.alerta);
                MostrarTextoFlotante("?");
                TimerGlobal.globalTime = 0;
                TimerGlobal.timerActivado = true;
            }
        }

    }
    private void Alerta()
    {
        //Buscar camareros en un rango determinado de un circulo de vision
        //Ir a zona donde se ha producido la alerta
        //Si en 5 segundos no detecta mapache en circulo de vision: estado patrulla. Sino, estado perseguir

        /*List<GameObject> camarerosCercanos = BuscarNPCSEnRadio(radioDeteccion, camareros);
        foreach (GameObject camarero in camarerosCercanos)
        {
            camarero.GetComponent<CamareroController>().fsm.ActivarEstado(MaquinaDeEstados.Estado.alerta);
            camarero.GetComponent<CamareroController>().MostrarTextoFlotante("?");
            camarero.GetComponent<CamareroController>().clienteAlertado = clienteAlertado;
        }*/
        

        if (EstaJugadorEnRadio(radioDeteccion) && fsm.estadoActual != MaquinaDeEstados.Estado.persecucion)
        {
            fsm.ActivarEstado(MaquinaDeEstados.Estado.persecucion);
        }
            

        if (TimerGlobal.globalTime >= segundosDeAlerta)
        {          
            clienteAlertado.GetComponent<ClienteController>().AlertaCliente = false;
            fsm.ActivarEstado(MaquinaDeEstados.Estado.patrulla);
            TimerGlobal.globalTime = 0;
            TimerGlobal.timerActivado = false;
            

            foreach (GameObject camarero in camareros)
            {
                CamareroController camcontrol = camarero.GetComponent<CamareroController>();
                int randomNumber = Random.Range(0, camcontrol._mesas.Length);
                camcontrol._mesaActual = camcontrol._mesas[randomNumber];
                if (randomNumber == 0 || randomNumber == 1)
                {
                    camcontrol._irMesas = false;
                    camcontrol._irCocina = true;
                    Rigidbody camRb = camcontrol.gameObject.GetComponent<Rigidbody>();
                    StartNavigation(camRb, _pathfinder.GetPath(_rb, _entradaCocina.transform), _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
                }
                else
                {
                    camcontrol._irMesas = true;
                    camcontrol._irCocina = false;
                    Rigidbody camRb = camcontrol.GetComponent<Rigidbody>();
                    StartNavigation(camRb, _pathfinder.GetPath(_rb, _mesaActual.transform), _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
                }
                camcontrol.OcultarTextoFlotante();

                camcontrol.fsm.ActivarEstado(MaquinaDeEstados.Estado.patrulla);
            }
        }

    }



    private void Perseguir()
    {
        MostrarTextoFlotante("!");
        List<GameObject> camarerosCercanos = BuscarNPCSEnRadio(radioDeteccion, camareros);
        foreach (GameObject camarero in camarerosCercanos)
        {
            if(camarero.GetComponent<CamareroController>().fsm.estadoActual != MaquinaDeEstados.Estado.persecucion)
            {
                camarero.GetComponent<CamareroController>().fsm.ActivarEstado(MaquinaDeEstados.Estado.persecucion);
            }    
        }
        if (!_isPursuing)
        {
            StartNavigation(_rb, _pathfinder.GetPath(_rb, jugador.transform), _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
            _isPursuing = true;
            Invoke("ToggleIsPursuing", 1.5f);
        }
    }

    IEnumerator SeekObjectWithRetard(GameObject objetivo)
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        StartNavigation(_rb, _pathfinder.GetPath(_rb, objetivo.transform), _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
    }

    public void Seek(GameObject location)
    {
        StartNavigation(_rb, _pathfinder.GetPath(_rb, location.transform), _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
    }

    private List<GameObject> BuscarNPCSEnRadio(int radio, GameObject[] arrayNpc)
    {
        List<GameObject> Cercanos = new List<GameObject>();
        for (int i = 0; i < arrayNpc.Length; i++)
        {
            if (Vector3.Distance(arrayNpc[i].transform.position, transform.position) <= radio)
            {
                Cercanos.Add(arrayNpc[i]);
            }
        }
        return Cercanos;

    }

    private bool EstaJugadorEnRadio(int radio)
    {
        if (Vector3.Distance(jugador.transform.position, transform.position) <= radio)
        {
            return true;
        }
        else return false;
    }

    private void ToggleIsPursuing()
    {
        _isPursuing = !_isPursuing;
    }
}
