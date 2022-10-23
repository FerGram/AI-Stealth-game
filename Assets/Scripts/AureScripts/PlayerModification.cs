using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using TMPro;

public class PlayerModification : MonoBehaviour
{
    
    [SerializeField] float detectCakeRadius;
    bool cakeDetected = false;
    public bool transportingCake = false;
    [SerializeField] LayerMask cakeMask;
    [SerializeField] GameObject cake;

    public Vector3 initialPos;
    public GameObject winText;
    public GameObject gameOverText;
    public GameObject retryButton;

    private void Awake()
    {
        initialPos = transform.position;
       
    }
    void Update()
    {
        if(cake == null)
        {
            cake = GameObject.FindGameObjectWithTag("Cake");
        }
        if (transportingCake)
        {
            GetComponent<PlayerMovement>().movementSpeed = 0.04f;
            cake.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }
        else
        {
            if (GetComponent<PlayerMovement>().movementSpeed == 0.04f)
            {
                GetComponent<PlayerMovement>().movementSpeed = 0.07f;
            }
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
            GameObject.Find("GameManager").GetComponent<ManageTime>().cakeInPlace = false;
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

        if (other.CompareTag("Goal") && transportingCake)
        {
            print("Meta");
            ManageTime timeManager = GameObject.Find("GameManager").GetComponent<ManageTime>();

            int minutes = Mathf.FloorToInt(timeManager.timer / 60.0f);
            int seconds = Mathf.FloorToInt(timeManager.timer - minutes * 60);
            string score = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
            timeManager.isTimer = false;
            winText.GetComponent<TextMeshProUGUI>().text = "Lo conseguiste!\n" + score;
            winText.SetActive(true);
            retryButton.SetActive(true);

        }

        if (other.gameObject.tag == "Camarero")
        {
            print("Has perdido");
            GameObject.Find("GameManager").GetComponent<ManageTime>().isTimer = false;
            gameOverText.SetActive(true);
            retryButton.SetActive(true);

        }
    }
}
