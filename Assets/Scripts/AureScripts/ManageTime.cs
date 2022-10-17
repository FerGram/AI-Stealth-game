using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ManageTime : MonoBehaviour
{


    public float timer;
    [SerializeField] GameObject textTimer;
    
    public bool isTimer = true;
    [SerializeField] GameObject cakePrefab;

    private void Awake()
    {
        timer = 0.0f;        
        GameObject[] fogones = GameObject.FindGameObjectsWithTag("FogonLibre");
        int randomNumber = UnityEngine.Random.Range(0, fogones.Length);
        GameObject fogonActual = fogones[randomNumber];
        Instantiate(cakePrefab, fogonActual.transform.position, Quaternion.identity);
    }
    void Update()
    {
        if (isTimer)
        {

            timer += Time.deltaTime;
            
        }
        DisplayTime();
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
