using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ManageTime : MonoBehaviour
{
    [SerializeField] GameObject cakePrefab;
    [SerializeField] GameObject textTimer;
    public GameObject fogonInicialCake;
    public bool cakeInPlace = true;
    public float timer;      
    public bool isTimer = true;    
    private GameObject cake;
    private void Awake()
    {
        timer = 0.0f;        
        GameObject[] fogones = GameObject.FindGameObjectsWithTag("FogonLibre");
        int randomNumber = UnityEngine.Random.Range(0, fogones.Length);
        fogonInicialCake = fogones[randomNumber];
        cake = Instantiate(cakePrefab, new Vector3(fogonInicialCake.transform.position.x, fogonInicialCake.transform.position.y + 0.8f,fogonInicialCake.transform.position.z ), Quaternion.identity);

    }
    void Update()
    {
        if (isTimer)
        {
            timer += Time.deltaTime;            
        }
        DisplayTime();
        if(Vector3.Distance(cake.transform.position, fogonInicialCake.transform.position) > 1f)
        {
            cakeInPlace = false;
        }        
    }  
    private void DisplayTime()
    {
        int minutes = Mathf.FloorToInt(timer / 60.0f);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        if(textTimer != null)
        {
            textTimer.GetComponent<TextMeshProUGUI>().text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);            
        }        
    }

   
    
}
