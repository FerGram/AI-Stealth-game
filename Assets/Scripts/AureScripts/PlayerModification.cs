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

    public Vector3 initialPos;

    private void Awake()
    {
        initialPos = transform.position;
    }
    void Update()
    {
        if (transportingCake)
        {
            cake.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        }
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
            cake.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            //cake.transform.parent = gameObject.transform;
            cake.GetComponent<Rigidbody>().isKinematic = true;
            transportingCake = true;
        }

        else if(Input.GetKeyDown(KeyCode.E) && transportingCake)
        {
            
            //cake.transform.parent = null;
            cake.GetComponent<Rigidbody>().isKinematic = false;
            transportingCake = false;
            //cake.GetComponent<Rigidbody>().AddForce(transform.forward * 2, ForceMode.Impulse);

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
