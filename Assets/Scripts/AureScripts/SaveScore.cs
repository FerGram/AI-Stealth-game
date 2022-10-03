using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using TMPro;


public class SaveScore : MonoBehaviour
{
    public bool gameFinished = false;
    float currentScore = 100000;
    string currentName = "Aure";
    [SerializeField] GameObject inputField;
    [SerializeField] GameObject buttonOk;
    [SerializeField] GameObject highscoreText;


    [SerializeField] GameObject player;
    [SerializeField] GameObject cake;
    private Vector3 initialPosCake;
    bool scoreChecked = false;



    private void Awake()
    {
        SaveFile();
        initialPosCake = cake.transform.position;
    }
    public void CheckTime()
    {
        print("CheckTime");
        if (!scoreChecked)
        {
            LoadFile();

            if (currentScore > gameObject.GetComponent<ManageTime>().timer)
            {
                currentScore = gameObject.GetComponent<ManageTime>().timer;
                buttonOk.SetActive(true);
                inputField.GetComponent<TMP_InputField>().text = "";
                inputField.SetActive(true);
            }

            scoreChecked = true;
        }
        else
        {
            highscoreText.SetActive(false);
            buttonOk.SetActive(false);

            scoreChecked = false;

            player.transform.position = player.GetComponent<PlayerModification>().initialPos;
            cake.transform.position = initialPosCake;

            GetComponent<ManageTime>().timer = 0.0f;
            GetComponent<ManageTime>().isTimer = true;

        }
       

    }

    public void ConfirmName()
    {
        currentName = inputField.GetComponent<TMP_InputField>().text;
        inputField.SetActive(false);
        highscoreText.SetActive(true);

        int minutes = Mathf.FloorToInt(currentScore / 60.0f);
        int seconds = Mathf.FloorToInt(currentScore - minutes * 60);

        highscoreText.GetComponent<TextMeshProUGUI>().text = "Current Highscore: \n " + currentName.ToString() + " " + string.Format("{0:00}:{1:00}", minutes, seconds);

        SaveFile();
    }

    public void SaveFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        Score score = new Score(currentScore, currentName);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, score);
        file.Close();
    }

    public void LoadFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found.");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        Score score = (Score)bf.Deserialize(file);
        file.Close();

        currentScore = score.score;
        currentName = score.name;


        
    }

   
}
