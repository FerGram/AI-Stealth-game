using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenObject : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] GameObject noisePrefab;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.CompareTag("Ground"))
        {
            print("Alo");
            Destroy(Instantiate(noisePrefab), 1f);
            Destroy(gameObject);
        }
    }
}
