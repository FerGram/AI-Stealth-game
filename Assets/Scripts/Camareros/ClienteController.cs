using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClienteController : MonoBehaviour
{
    public bool AlertaCliente = false;
    public GameObject floatingText;
    private AudioSource audiosource;
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (AlertaCliente)
        {
            audiosource.Play();
        }
    }
}
