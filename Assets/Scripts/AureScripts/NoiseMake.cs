using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMake : MonoBehaviour
{
    Collider[] coliders;
    [SerializeField] float soundRadius;
    [SerializeField] float soundLifeTime;
    void Start()
    {
        coliders = Physics.OverlapSphere(transform.position, soundRadius);
        foreach (Collider c in coliders)
        {
            if (c.CompareTag("Waiter"))
            {                
                print("Noticed");
            }
        }
        StartCoroutine("DestroySound");        
    }


    IEnumerator DestroySound()
    {
        yield return new WaitForSeconds(soundLifeTime);
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, soundRadius);
    }
}
