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
    public static Estado estadoActual;

    // Start is called before the first frame update
    void Start()
    {
        ActivarEstado(estadoInicial);
    }

    private void ActivarEstado(Estado estado)
    {
        /*if (estadoActual != null)*/ estadoActual = estado;
    }
}
