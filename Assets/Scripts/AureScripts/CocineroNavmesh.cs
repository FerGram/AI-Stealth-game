using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CocineroNavmesh : MonoBehaviour
{   
    private MaquinaDeEstados fsm;
    private NavMeshAgent navMesh;
    private AudioSource audioSource;    
    private Rigidbody _rb;
    [SerializeField] private GameObject textoFlotante;
    [SerializeField] private GameObject player;
    [Space]

    [Header("Navigation")]
    [SerializeField] Pathfinder _pathfinder;
    [SerializeField] float _movementSpeed = 100f;
    [SerializeField] float _rotationSpeed = 180f;    //Translates to degrees per second
    [SerializeField] float _stoppingNodeDistance = 0.01f;
    [Space]

    [Header("Patrulla")]
    private GameObject[] fogones;
    [SerializeField] private GameObject fogonActual;
    [SerializeField] private int radioDeteccion;
  
    public bool canSeePlayer = false;
    public bool canSeeCake = false;
    [Space]

    [Header("Perseguir")]
    [SerializeField] private GameObject knifeSpawn;
    [SerializeField] private GameObject knifePrefab;
    [SerializeField] float lookDamping = 5.0f;
    [SerializeField] float cantSeeTime = 3.0f;
    private bool coroutineStarted = false;    
    private bool knifeReady = true;    
    float cantSeeTimer = 0.0f;
    bool firstTimePatroling = true;
    Vector3 lastPlaceSeen;
    float distanceToPursuit = 2.0f;
    public bool putingCake = false;
    private bool transportingCake = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        navMesh = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        fogones = GameObject.FindGameObjectsWithTag("FogonLibre");        
        fsm = GetComponent<MaquinaDeEstados>();
        audioSource = GetComponent<AudioSource>();
        Random.InitState(System.DateTime.Now.Millisecond);
        int randomNumber = Random.Range(0, fogones.Length);
        fogonActual = fogones[randomNumber];
        navMesh.destination = fogonActual.transform.position;
    }

    void FixedUpdate()
    {            
        if (fsm.estadoActual == MaquinaDeEstados.Estado.patrulla)
        {          
            Patrullar();
            if (canSeePlayer)
            {
                audioSource.Play();
                //StopAllCoroutines();
                Seek(transform.position);
                knifeReady = true;
                fsm.ActivarEstado(MaquinaDeEstados.Estado.persecucion);
            }
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
    private void MostrarTextoRojo()
    {
        textoFlotante.GetComponent<TextMesh>().color = Color.red;
    }
    private void MostrarTextoBlanco()
    {
        textoFlotante.GetComponent<TextMesh>().color = Color.white;
    }
    private void Patrullar()
    {        
        GameObject cake = GameObject.FindGameObjectWithTag("Cake");
        if (transportingCake)
        {
            cake.transform.position = new Vector3(transform.position.x , transform.position.y + 0.5f, transform.position.z + 0.5f);
        }
        if (putingCake)
        {        
            if(Vector3.Distance(transform.position, cake.transform.position) < 0.5f && !transportingCake)
            {
                transportingCake = true;               
                Seek(GameObject.Find("GameManager").GetComponent<ManageTime>().fogonInicialCake.transform.position);
                cake.GetComponent<Rigidbody>().isKinematic = true;               
            }
            else
            {
                if (!transportingCake)
                {
                    Seek(cake.transform.position);                    
                }            
            }      
            if (transportingCake && Vector3.Distance(transform.position, GameObject.Find("GameManager").GetComponent<ManageTime>().fogonInicialCake.transform.position) < 1.0f)
            {                
                cake.GetComponent<Rigidbody>().isKinematic = false;
                cake.transform.position = GameObject.Find("GameManager").GetComponent<ManageTime>().fogonInicialCake.transform.position;
                transportingCake = false;
                putingCake = false;
                GameObject.Find("GameManager").GetComponent<ManageTime>().cakeInPlace = true;
                firstTimePatroling = true;
            }           
        }

        else
        {
            if (Vector3.Distance(transform.position, fogonActual.transform.position) < 1 || firstTimePatroling)
            {                
                firstTimePatroling = false;
                fogonActual.tag = "FogonOcupado";
                fogones = GameObject.FindGameObjectsWithTag("FogonLibre");
                GameObject nuevoFogon = fogones[Random.Range(0, fogones.Length)];
                if (!coroutineStarted)
                {
                    StartCoroutine(SeekObjectWithRetard(nuevoFogon));
                    coroutineStarted = true;
                }
            }
            else
            {
                coroutineStarted = false;
            }
        }    
    }
    private void Perseguir()
    {       
        MostrarTextoFlotante("!");       
        if (knifeReady && canSeePlayer)
        {
            StartCoroutine("PrepareKnife");
            knifeReady = false;
        }
        if (canSeePlayer)
        {
            cantSeeTimer = 0.0f;
            lastPlaceSeen = player.transform.position;
            if(Vector3.Distance(transform.position, player.transform.position) > distanceToPursuit)
            {
                Seek(player.transform.position);
            }
            else
            {
                Seek(transform.position);
            }           
        }
        else
        {
            cantSeeTimer += Time.deltaTime;
            Vector3 lookPos = player.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookDamping);
            Seek(lastPlaceSeen);
        }
        if (cantSeeTimer >= cantSeeTime)
        {
            firstTimePatroling = true;
            coroutineStarted = false;
            fsm.ActivarEstado(MaquinaDeEstados.Estado.patrulla);
            OcultarTextoFlotante();
        }
    }
    IEnumerator PrepareKnife()
    {
        MostrarTextoRojo();
        yield return new WaitForSeconds(2.0f);
        ThrowKnife();
    }
    IEnumerator KnifeCooldown()
    {
        MostrarTextoBlanco();
        yield return new WaitForSeconds(1.0f);
        knifeReady = true;
    }
    private void ThrowKnife()
    {        
         GameObject knifeThrowed = Instantiate(knifePrefab, knifeSpawn.transform.position, knifePrefab.transform.rotation);
         Vector3 predictedPoint = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) + (player.GetComponent<Rigidbody>().velocity * Time.deltaTime * 250);
         Vector3 directionedForce = predictedPoint - transform.position;
         knifeThrowed.GetComponent<Rigidbody>().AddForce(directionedForce.normalized * 15, ForceMode.Impulse);
         knifeThrowed.GetComponent<Rigidbody>().AddTorque(transform.forward * 10, ForceMode.Impulse);
         StartCoroutine("KnifeCooldown");               
    }
    IEnumerator SeekObjectWithRetard(GameObject objetivo)
    {
        yield return new WaitForSeconds(Random.Range(3, 8));        
        fogonActual.tag = "FogonLibre";
        fogonActual = objetivo;               
        _pathfinder.StartPathfinding(_rb, fogonActual.transform, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
    }

    public void Seek(Vector3 location)
    {
        _pathfinder.StartPathfinding(_rb, location, _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
    }
}
