using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenObject : MonoBehaviour
{   
    [SerializeField] GameObject noisePrefab;    
    private void OnCollisionEnter(Collision collision)
    {        
        if (collision.collider.CompareTag("Ground"))
        {            
            Destroy(Instantiate(noisePrefab), 1f);
            Destroy(gameObject);
        }
    }
}
