using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModification : MonoBehaviour
{
    
    [SerializeField] float detectCakeRadius;
    bool cakeDetected = false;
    bool transportingCake = false;
    [SerializeField] LayerMask cakeMask;
    [SerializeField] GameObject cake;
    
    void Update()
    {
       
        Collider[] objectsDetected = Physics.OverlapSphere(transform.position, detectCakeRadius, cakeMask);

        if(objectsDetected.Length > 0)
        {
            cakeDetected = true;
            cake.GetComponent<RemarkableObject>().objectSelected = true;
            
        }
        else
        {
            cakeDetected = false;
            cake.GetComponent<RemarkableObject>().objectSelected = false;
        }

        if(Input.GetKeyDown(KeyCode.E) && !transportingCake && cakeDetected)
        {
            cake.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            cake.transform.parent = gameObject.transform;
            cake.GetComponent<Rigidbody>().isKinematic = true;
            transportingCake = true;
        }

        else if(Input.GetKeyDown(KeyCode.E) && transportingCake)
        {
            
            cake.transform.parent = null;
            cake.GetComponent<Rigidbody>().isKinematic = false;
            transportingCake = false;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.CompareTag("Goal") && transportingCake)
        {
            print("Meta");
            GameObject.Find("GameManager").GetComponent<ManageTime>().isTimer = false;
            GameObject.Find("GameManager").GetComponent<SaveScore>().CheckTime();
        }
    }
}
