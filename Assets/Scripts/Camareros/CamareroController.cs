//#define DEBUG_INFO

using System.Collections;
using UnityEngine;

public class CamareroController : MonoBehaviour
{
    //[SerializeField] NavMeshAgent navMesh;

    [SerializeField] GameObject _player;
    [SerializeField] GameObject _entradaCocina;
    [Space]
    [Header("Navigation")]
    [SerializeField] Pathfinder _pathfinder;
    [SerializeField] float _movementSpeed = 100f;     
    [SerializeField] float _rotationSpeed = 180f;    //Translates to degrees per second
    [SerializeField] float _stoppingNodeDistance = 0.01f;

    //Patrulla
    private bool _irMesas;
    private bool _irCocina;
    private GameObject[] _mesas;
    private GameObject _mesaActual;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
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

        if(MaquinaDeEstados.estadoActual == MaquinaDeEstados.Estado.patrulla)
        {          
            Patrullar();
        }
        else if (MaquinaDeEstados.estadoActual == MaquinaDeEstados.Estado.alerta)
        {
            Alerta();
        }

        else
        {
            Perseguir();
        }
    }

    private void Patrullar()
    {
        //Tiene que ir de la entrada de la cocina a la mesa que esté disponible (sin camareros)


        if (Vector3.Distance(transform.position, _mesaActual.transform.position) < 1 && _irMesas)
        {
            //Está cerca de mesa, tiene que ir a cocina
            _irMesas = false;
            _mesaActual.tag = "MesaOcupada";

#if DEBUG_INFO
            DebugConsultarArrayMesasLibres(_mesas);
#endif
            _irCocina = true;
            StartCoroutine(IrHacia(_entradaCocina));

        }
        
        else if (Vector3.Distance(transform.position, _entradaCocina.transform.position) < 1 && _irCocina)
        {
            //Está cerca de la cocina, tiene que ir a una mesa
            _irCocina = false;
            _irMesas = true;
            _mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
            _mesaActual = _mesas[Random.Range(0, _mesas.Length)];
            StartCoroutine(IrHacia(_mesaActual));
        }

    }
    private void Alerta()
    {

    }
    private void Perseguir()
    {
        //WARNING: THIS IS IN UPDATE, WE DON'T WANT TO CONSTANTLY CALL THIS FUNCTION
        _pathfinder.StartPathfinding(_rb, _player.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
    }

    IEnumerator IrHacia(GameObject objetivo)
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        
        if (_irCocina)
        {
            _mesaActual.tag = "MesaLibre";
        }
        _pathfinder.StartPathfinding(_rb, objetivo.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
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
