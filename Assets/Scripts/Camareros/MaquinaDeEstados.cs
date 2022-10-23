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

    void Start()
    {
        ActivarEstado(estadoInicial);
    }

    public void ActivarEstado(Estado estado)
    {
        estadoActual = estado;
       
    }
}
