using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Score
{
    public float score;
    public string name;

    public Score(float score, string name)
    {
        this.score = score;
        this.name = name;
    }
}
