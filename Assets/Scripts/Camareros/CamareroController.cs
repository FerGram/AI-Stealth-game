using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CamareroController : MonoBehaviour
{
    //[SerializeField] NavMeshAgent navMesh;

    [SerializeField] GameObject _player;
    [SerializeField] GameObject _entradaCocina;
    [Space]
    [Header("Navigation")]
    [SerializeField] Pathfinder _pathfinder;
    [SerializeField] float _speed = 3f;
    [SerializeField] float _stoppingNodeDistance = 0.3f;

    //Patrulla
    private bool _irMesas;
    private bool _irCocina;
    private GameObject[] _mesas;
    private GameObject _mesaActual;


    void Start()
    {
        _mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
        int randomNumber = Random.Range(0, _mesas.Length);
        _mesaActual = _mesas[randomNumber];
        Debug.Log(randomNumber);
        if(randomNumber == 0 || randomNumber == 1)
        {
            _irMesas = false;
            _irCocina = true;
            //navMesh.destination = _entradaCocina.transform.position;
            _pathfinder.StartPathfinding(transform, _entradaCocina.transform, _speed, _stoppingNodeDistance);
        }
        else
        {
            _irMesas = true;
            _irCocina = false;
            //navMesh.destination = _mesaActual.transform.position;
            _pathfinder.StartPathfinding(transform, _mesaActual.transform, _speed, _stoppingNodeDistance);
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
            //mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
            DebugConsultarArrayMesasLibres(_mesas);
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
        //navMesh.destination = _player.transform.position;
        _pathfinder.StartPathfinding(transform, _player.transform, _speed, _stoppingNodeDistance);
    }

    IEnumerator IrHacia(GameObject objetivo)
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        
        if (_irCocina)
        {
            _mesaActual.tag = "MesaLibre";
            //mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
            
            //mesaActual = mesas[Random.Range(0, mesas.Length)];
        }
        //navMesh.destination = objetivo.transform.position;
        _pathfinder.StartPathfinding(transform, objetivo.transform, _speed, _stoppingNodeDistance);
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
