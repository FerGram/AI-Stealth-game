using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CamareroController : MonoBehaviour
{
    public NavMeshAgent navMesh;

    public GameObject player;
    public GameObject entradaCocina;


    //Patrulla
    private bool irMesas;
    private bool irCocina;
    private GameObject[] mesas;
    private GameObject mesaActual;


    void Start()
    {
        mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
        int randomNumber = Random.Range(0, mesas.Length);
        mesaActual = mesas[randomNumber];
        Debug.Log(randomNumber);
        if(randomNumber == 0 || randomNumber == 1)
        {
            irMesas = false;
            irCocina = true;
            navMesh.destination = entradaCocina.transform.position;
        }
        else
        {
            irMesas = true;
            irCocina = false;
            navMesh.destination = mesaActual.transform.position;
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


        if (Vector3.Distance(transform.position, mesaActual.transform.position) < 1 && irMesas)
        {
            irMesas = false;
            mesaActual.tag = "MesaOcupada";
            mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
            DebugConsultarArrayMesasLibres(mesas);
            irCocina = true;
            StartCoroutine(IrHacia(entradaCocina));

        }
        //Cerca de la cocina
        else if (Vector3.Distance(transform.position, entradaCocina.transform.position) < 1 && irCocina)
        {
            irCocina = false;
            irMesas = true;
            StartCoroutine(IrHacia(mesaActual));
        }

    }
    private void Alerta()
    {

    }
    private void Perseguir()
    {
        navMesh.destination = player.transform.position;
    }

    IEnumerator IrHacia(GameObject objetivo)
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        
        if (irCocina)
        {
            mesaActual.tag = "MesaLibre";
            mesas = GameObject.FindGameObjectsWithTag("MesaLibre");
            
            mesaActual = mesas[Random.Range(0, mesas.Length)];
        }
        navMesh.destination = objetivo.transform.position;
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
