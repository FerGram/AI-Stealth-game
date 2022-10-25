using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class CocineroController : Agent
{   
    private MaquinaDeEstados fsm;
    private AudioSource audioSource;    
    private Rigidbody _rb;
    [SerializeField] private GameObject textoFlotante;
    [SerializeField] private GameObject player;
    [Space]

    [Header("Navigation")]
    [SerializeField] Pathfinder _pathfinder;
    [SerializeField] float _movementSpeed = 100f;
    [SerializeField] float _rotationSpeed = 180f;    //Translates to degrees per second
    [SerializeField] float _stoppingNodeDistance = 0.2f;
    [Space]

    [Header("Patrulla")]
    private GameObject[] fogones;
    [SerializeField] private GameObject fogonActual;
    [SerializeField] private int radioDeteccion;
  
    public bool canSeePlayer = false;
    [Space]

    [Header("Perseguir")]
    [SerializeField] private GameObject knifeSpawn;
    [SerializeField] private GameObject knifePrefab;
    //[SerializeField] float lookDamping = 5.0f;
    //[SerializeField] float cantSeeTime = 3.0f;
    private bool knifeReady = true;    

    private bool _isPursuing = false;
    private bool _readyToChangeFire = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        fogones = GameObject.FindGameObjectsWithTag("FogonLibre");        
        fsm = GetComponent<MaquinaDeEstados>();
        audioSource = GetComponent<AudioSource>();
        Random.InitState(System.DateTime.Now.Millisecond);
        int randomNumber = Random.Range(0, fogones.Length);
        fogonActual = fogones[randomNumber];

        StartNavigation(_rb, _pathfinder.GetPath(_rb, fogonActual.transform), _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
        Invoke("ToggleChangeFire", 0.5f);
    }

    void FixedUpdate()
    {            
        if (fsm.estadoActual == MaquinaDeEstados.Estado.patrulla)
        {          
            Patrullar();
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
        #region NotEnoughTime
        //NOT ENOUGH TIME TO IMPLEMENT
        //GameObject cake = GameObject.FindGameObjectWithTag("Cake");
        //if (transportingCake)
        //{
        //    cake.transform.position = new Vector3(transform.position.x , transform.position.y + 0.5f, transform.position.z + 0.5f);
        //}
        //if (putingCake)
        //{        
        //    if(Vector3.Distance(transform.position, cake.transform.position) < 0.5f && !transportingCake)
        //    {
        //        transportingCake = true;          
        //        Seek(GameObject.Find("GameManager").GetComponent<ManageTime>().fogonInicialCake.transform.position);
        //        cake.GetComponent<Rigidbody>().isKinematic = true;   
        //    }
        //    else
        //    {
        //        if (!transportingCake)
        //        {
        //            Seek(cake.transform.position);                    
        //        }            
        //    }      
        //    if (transportingCake && Vector3.Distance(transform.position, GameObject.Find("GameManager").GetComponent<ManageTime>().fogonInicialCake.transform.position) < 1.0f)
        //    {                
        //        cake.GetComponent<Rigidbody>().isKinematic = false;
        //        cake.transform.position = GameObject.Find("GameManager").GetComponent<ManageTime>().fogonInicialCake.transform.position;
        //        transportingCake = false;
        //        putingCake = false;
        //        GameObject.Find("GameManager").GetComponent<ManageTime>().cakeInPlace = true;
        //        firstTimePatroling = true;
        //    }           
        //}
        #endregion

        if (_readyToChangeFire && Vector3.Distance(transform.position, fogonActual.transform.position) < _stoppingNodeDistance)
        {
            Debug.Log("Going towards objective");
            fogonActual.tag = "FogonOcupado";
            fogones = GameObject.FindGameObjectsWithTag("FogonLibre");
            GameObject nuevoFogon = fogones[Random.Range(0, fogones.Length)];

            _readyToChangeFire = false;

            StartCoroutine(SeekObjectWithRetard(nuevoFogon));
            
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
        if (canSeePlayer && !_isPursuing)
        {
            Seek(player.transform.position);
            _isPursuing = true;
            Invoke("ToggleIsPursuing", 1.5f);

            //cantSeeTimer = 0.0f;
            //lastPlaceSeen = player.transform.position;
            //if(Vector3.Distance(transform.position, player.transform.position) > distanceToPursuit)
            //{
            //    Seek(player.transform.position);
            //}
            //else
            //{
            //    Seek(transform.position);
            //}           
        }
        else if (!_isPursuing)
        {
            //firstTimePatroling = true;
            //coroutineStarted = false;
            fsm.ActivarEstado(MaquinaDeEstados.Estado.patrulla);
            OcultarTextoFlotante();
            //cantSeeTimer += Time.deltaTime;
            //Vector3 lookPos = player.transform.position - transform.position;
            //Quaternion rotation = Quaternion.LookRotation(lookPos);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookDamping);
            //Seek(lastPlaceSeen);
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
        yield return new WaitForSeconds(Random.Range(2, 5));
        Invoke("ToggleChangeFire", 1f);
        fogonActual.tag = "FogonLibre";
        fogonActual = objetivo;
        StartNavigation(_rb, _pathfinder.GetPath(_rb, fogonActual.transform), _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
    }

    public void Seek(Vector3 location)
    {
        StartNavigation(_rb, _pathfinder.GetPath(_rb, location), _movementSpeed, _stoppingNodeDistance, _rotationSpeed);
    }

    private void ToggleIsPursuing()
    {
        _isPursuing = !_isPursuing;
    }

    private void ToggleChangeFire()
    {
        _readyToChangeFire = !_readyToChangeFire;
    }
}
