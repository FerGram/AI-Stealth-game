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
    private bool coroutineStarted = false;
    private Vector3 lastPlayerPosition;
    [SerializeField] private GameObject knifeSpawn;
    [SerializeField] private GameObject knifePrefab;
    private bool knifeReady = true;
    [SerializeField] private float knifeOffset = 0.1f;
 
    
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

    void Update()
    {
       


        if (canSeePlayer && fsm.estadoActual != MaquinaDeEstados.Estado.persecucion)
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
            
            //Alerta();
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



        if (Vector3.Distance(transform.position, fogonActual.transform.position) < 1)
        {
            //Está cerca de fogon, tiene que ir a otro fogon
            
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

    private void Perseguir()
    {
        MostrarTextoFlotante("!");
        lastPlayerPosition = jugador.transform.position;

        if (knifeReady)
        {
            StartCoroutine("PrepareKnife");
            knifeReady = false;
        }
       
    }

    IEnumerator PrepareKnife()
    {
        yield return new WaitForSeconds(2.0f);
        ThrowKnife();
    }

    IEnumerator KnifeCooldown()
    {
        yield return new WaitForSeconds(1.0f);
        knifeReady = true;
    }

    private void ThrowKnife()
    {
        GameObject knifeThrowed = Instantiate(knifePrefab, knifeSpawn.transform.position, Quaternion.identity);
        
        Vector3 directionedForce =  lastPlayerPosition - transform.position;
        directionedForce = new Vector3(directionedForce.x + Random.Range(-knifeOffset, knifeOffset), directionedForce.y + Random.Range(-knifeOffset, knifeOffset), directionedForce.z + Random.Range(-knifeOffset, knifeOffset));
        knifeThrowed.GetComponent<Rigidbody>().AddForce(directionedForce * 4, ForceMode.Impulse);
        knifeThrowed.GetComponent<Rigidbody>().AddTorque(Vector3.forward * 100);
        StartCoroutine("KnifeCooldown");
    }
    IEnumerator SeekObjectWithRetard(GameObject objetivo)
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
        
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
