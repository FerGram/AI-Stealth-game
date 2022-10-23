//#define DEBUG_INFO

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CamareroController : MonoBehaviour
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
            _pathfinder.StartPathfinding(_rb, _entradaCocina.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
        }
        else
        {
            _irMesas = true;
            _irCocina = false;
            _pathfinder.StartPathfinding(_rb, _mesaActual.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
        }

        
    }

    void Update()
    {
        foreach (GameObject cliente in clientes)
        {
            ClienteController clienteController = cliente.GetComponent<ClienteController>();
            if (clienteController.AlertaCliente && fsm.estadoActual == MaquinaDeEstados.Estado.patrulla)
            {
                clienteAlertado = cliente;
                fsm.ActivarEstado(MaquinaDeEstados.Estado.alerta);
                MostrarTextoFlotante("?");
                TimerGlobal.globalTime = 0;
                TimerGlobal.timerActivado = true;
                clienteController.textoFlotanteCliente.SetActive(true);
            }
        }


        if (EstaJugadorEnRadio(radioDeteccion) && fsm.estadoActual != MaquinaDeEstados.Estado.persecucion)
        {
            audioSource.Play();
            //_pathfinder.StartPathfinding(_rb, jugador.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
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


        if (Vector3.Distance(transform.position, _mesaActual.transform.position) < 1 && _irMesas)
        {
            //Está cerca de mesa, tiene que ir a cocina
            _irMesas = false;
            _mesaActual.tag = "MesaOcupada";
            _irCocina = true;
            StartCoroutine(SeekObjectWithRetard(_entradaCocina));
            //SeekObjectWithRetard(_entradaCocina);
        }
        
        else if (Vector3.Distance(transform.position, _entradaCocina.transform.position) < 1 && _irCocina)
        {
            //Está cerca de la cocina, tiene que ir a una mesa
            _irCocina = false;
            _irMesas = true;
            _mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
            _mesaActual = _mesas[Random.Range(0, _mesas.Length)];
            Debug.Log("Voy a la mesa");
            StartCoroutine(SeekObjectWithRetard(_mesaActual));
            //SeekObjectWithRetard(_mesaActual);
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
        StartCoroutine(SeekObjectWithRetard(clienteAlertado));


        if (EstaJugadorEnRadio(radioDeteccion))
        {
            fsm.ActivarEstado(MaquinaDeEstados.Estado.persecucion);
        }
            

        if (TimerGlobal.globalTime >= segundosDeAlerta)
        {
            clienteAlertado.GetComponent<ClienteController>().textoFlotanteCliente.SetActive(false);
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
                    _pathfinder.StartPathfinding(camRb, _entradaCocina.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
                }
                else
                {
                    camcontrol._irMesas = true;
                    camcontrol._irCocina = false;
                    Rigidbody camRb = camcontrol.GetComponent<Rigidbody>();
                    _pathfinder.StartPathfinding(camRb, _mesaActual.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);

                }
                camcontrol.OcultarTextoFlotante();

                camcontrol.fsm.ActivarEstado(MaquinaDeEstados.Estado.patrulla);
            }
        }

    }



    private void Perseguir()
    {
        MostrarTextoFlotante("!");
        if (EstaJugadorEnRadio(radioDeteccion))
        {
            _pathfinder.StartPathfinding(_rb, jugador.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
        }
    }

    IEnumerator SeekObjectWithRetard(GameObject objetivo)
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        
        if (_irCocina)
        {
            _mesaActual.tag = "MesaLibre";
        }
        _pathfinder.StartPathfinding(_rb, objetivo.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
    }

    public void Seek(GameObject location)
    {
        _pathfinder.StartPathfinding(_rb, location.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
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
}
