using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeDestruction : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        StartCoroutine("DestroyKnife");
    }

    IEnumerator DestroyKnife()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(this.gameObject);
    }
}
