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
    private Estado estadoActual;

    // Start is called before the first frame update
    void Start()
    {
        ActivarEstado(estadoInicial);
    }

    private void ActivarEstado(Estado estado)
    {
        if (estadoActual != null) estadoActual = estado;
        //Debug.Log(estadoActual);
    }

    void Update()
    {
        if(estadoActual == Estado.patrulla)
        {

        }

        else if(estadoActual == Estado.alerta)
        {

        }

        else
        {

        }
    }
}
