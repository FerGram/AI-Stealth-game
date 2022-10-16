using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemarkableObject : MonoBehaviour
{
    private const float DISTANCE_TO_PLAYER_MAX = 5.0f;
    private GameObject playerObject;
    private GameObject gameManager;
    private Outline outline;
    private float distance;
    public bool objectSelected = false;

    private void Start()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineWidth = 0f;
    }    
    private void Update()
    {
        if (objectSelected)
        {
            outline.OutlineWidth = 5.0f;
        }
        else
        {
            if(outline.OutlineWidth != 0f)
            {
                outline.OutlineWidth = 0f;
            }
        }
    }
}
