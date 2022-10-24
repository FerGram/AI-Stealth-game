using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerGlobal : MonoBehaviour
{
    public static float globalTime = 0;
    public static bool timerActivado = false;

    public void FixedUpdate()
    {
        if (timerActivado)
        {
             globalTime += Time.deltaTime;
             Debug.LogWarning(globalTime);
        }

    }

}
