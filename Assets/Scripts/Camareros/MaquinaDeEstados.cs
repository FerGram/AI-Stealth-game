using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaquinaDeEstados : MonoBehaviour
{
    public enum Estado{
        patrulla,
        alerta,
        persecucion
    }

    public Estado estadoInicial;
    public Estado estadoActual;

    // Start is called before the first frame update
    void Start()
    {
        ActivarEstado(estadoInicial);
    }

    public void ActivarEstado(Estado estado)
    {
        /*if (estadoActual != null)*/ estadoActual = estado;
       // Debug.Log("Estado " + estadoActual + " activado");
    }
}
