using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CocineroController : MonoBehaviour
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
    private GameObject[] fogones;
    private GameObject fogonActual;

    //Alerta
    public int radioDeteccion;
    public float segundosDeAlerta = 5;
    private GameObject[] cocineros;
    public bool canSeePlayer = false;
    public bool canSeeCake = false;
    private bool coroutineStarted = false;
    private Vector3 lastPlayerPosition;
    [SerializeField] private GameObject knifeSpawn;
    [SerializeField] private GameObject knifePrefab;
    private bool knifeReady = true;
    [SerializeField] private float knifeOffset = 0.1f;


    [SerializeField] float cantSeeTime = 3.0f;
    [SerializeField] float cantSeeTimer = 0.0f;

    [SerializeField] float lookDamping = 5.0f;
    [SerializeField] bool firstTimePatroling = true;

    Vector3 lastPlaceSeen;
    float distanceToPursuit = 2.0f;

    public bool putingCake = false;
    private bool transportingCake = false;

    void Start()
    {
        fogones = GameObject.FindGameObjectsWithTag("FogonLibre");
        cocineros = GameObject.FindGameObjectsWithTag("Cocinero");
        
        fsm = GetComponent<MaquinaDeEstados>();
        audioSource = GetComponent<AudioSource>();

        int randomNumber = Random.Range(0, fogones.Length);
        fogonActual = fogones[randomNumber];
        
        navMesh.SetDestination(fogonActual.transform.position);
        
        
    }

    void FixedUpdate()
    {
        


        if (canSeePlayer && fsm.estadoActual != MaquinaDeEstados.Estado.persecucion)
        {
            audioSource.Play();
            StopAllCoroutines();
            Seek(transform.position);
            knifeReady = true;
            fsm.ActivarEstado(MaquinaDeEstados.Estado.persecucion);
        }


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
                Debug.Log("Hola");
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
                //Está cerca de fogon, tiene que ir a otro fogon
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
            lastPlaceSeen = jugador.transform.position;

            if(Vector3.Distance(transform.position, jugador.transform.position) > distanceToPursuit)
            {
                Seek(jugador.transform.position);
            }
            else
            {
                Seek(transform.position);
            }
            //Seek(lastPlaceSeen / 2);
        }
        else
        {
            cantSeeTimer += Time.deltaTime;

            Vector3 lookPos = jugador.transform.position - transform.position;

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

         Vector3 predictedPoint = new Vector3(jugador.transform.position.x, jugador.transform.position.y, jugador.transform.position.z) + (jugador.GetComponent<Rigidbody>().velocity * Time.deltaTime * 250);
         Vector3 directionedForce = predictedPoint - transform.position;
         //directionedForce = new Vector3(directionedForce.x + Random.Range(-knifeOffset, knifeOffset), directionedForce.y + Random.Range(-knifeOffset, knifeOffset), directionedForce.z + Random.Range(-knifeOffset, knifeOffset)).normalized;
         knifeThrowed.GetComponent<Rigidbody>().AddForce(directionedForce.normalized * 15, ForceMode.Impulse);
         knifeThrowed.GetComponent<Rigidbody>().AddTorque(transform.forward * 10, ForceMode.Impulse);
         StartCoroutine("KnifeCooldown");
        
        
    }
    IEnumerator SeekObjectWithRetard(GameObject objetivo)
    {
        yield return new WaitForSeconds(Random.Range(3, 8));
        
        fogonActual.tag = "FogonLibre";

        fogonActual = objetivo;
        
        navMesh.SetDestination(fogonActual.transform.position);
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
        x = jugador.transform.position.x;
        y = jugador.transform.position.y;
        a = transform.position.x;
        b = transform.position.y;
        if ((x - a) * (x - a) + (y - b) * (y - b) <= radio * radio)
        {
            return true;
        }
        return false;
    }
}
