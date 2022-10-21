using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesFunctions : MonoBehaviour
{
    public GameObject winText;
    public GameObject gameOverText;
    public GameObject retryButton;
    public void ReloadScene(string sceneName)
    {
        print("hola");
        GameObject.Find("GameManager").GetComponent<ManageTime>().timer = 0.0f;
        winText.SetActive(false);
        gameOverText.SetActive(false);
        retryButton.SetActive(false);
        //poner posicion inicial
        //poner todos los npcs en patrulla
        SceneManager.LoadScene(sceneName);
    }
}
