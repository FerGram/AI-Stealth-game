using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ManageTime : MonoBehaviour
{
    public GameObject fogonInicialCake;
    public bool cakeInPlace = true;

    public float timer;
    [SerializeField] GameObject textTimer;
    
    public bool isTimer = true;
    [SerializeField] GameObject cakePrefab;

    private GameObject cake;

    private void Awake()
    {
        timer = 0.0f;        
        GameObject[] fogones = GameObject.FindGameObjectsWithTag("FogonLibre");
        int randomNumber = UnityEngine.Random.Range(0, fogones.Length);
        fogonInicialCake = fogones[randomNumber];
        cake = Instantiate(cakePrefab, fogonInicialCake.transform.position, Quaternion.identity);
    }
    void Update()
    {
        if (isTimer)
        {

            timer += Time.deltaTime;
            
        }
        DisplayTime();

        if(Vector3.Distance(cake.transform.position, fogonInicialCake.transform.position) > 0.5f)
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
