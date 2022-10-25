using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFunctions : MonoBehaviour
{
    public void ReloadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
