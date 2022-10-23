using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeDestruction : MonoBehaviour
{
    public bool isMortal = true;
    private void Awake()
    {
        StartCoroutine("DestroyKnife");
    }

    IEnumerator DestroyKnife()
    {
        yield return new WaitForSeconds(20.0f);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isMortal && collision.gameObject.tag == "Player")
        {
            print("Is Dead");
        }
        else
        {
            isMortal = false;
            //GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
