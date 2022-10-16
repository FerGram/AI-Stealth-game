using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InField : MonoBehaviour
{
    
    public Transform target;//objeto

    Vector3 v = Vector3.zero;
    float distance = 0.0f;
    
    public float radius = 1.0f;//grandaria del gizmo

    public float dot = 0.0f;

    public float fov = 50.0f; //Angulo de vision donde localiza al objeto
    public float dotFov = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos(){
        Gizmos.color=Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);


        v= target.position - transform.position;//calcula la distancia
        distance = v.sqrMagnitude;

        v.Normalize();

        dotFov = Mathf.Cos(fov * 0.5f* Mathf.Deg2Rad);

        dot = Vector3.Dot(transform.forward, v);

        //Debug.Log(dot);

        //si esta dentro del radio y del angulo de vision fov rojo y sino verde
        if ((distance<=radius*radius) && (dot >=dotFov))
        {
            Gizmos.color = Color.red;
            Debug.Log ("dentro");
        }
        else{
            Gizmos.color = new Color(0f, 0.7f, 0f, 1f);
            //Debug.Log ("fuera");
            
        }
        Gizmos.DrawLine(transform.position, target.position);//linea que identifica el objeto

        //Funcion para visualizar el cono de vision de manera rudimentaria
        DrawVisionField();
    }


    

    void DrawVisionField()
    {
        Gizmos.color = Color. gray;
        float amp = radius * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        Vector3 r= Vector3.zero;
        Vector3 l= Vector3.zero;
        Vector3 u = Vector3.zero;
        Vector3 d = Vector3.zero;
        Vector3 advance = Vector3.zero;
        advance = transform.localPosition + transform.forward * radius;
        r = transform.right * amp;
        l = -transform.right * amp;

        u = transform.up * amp;
        d = -transform.up * amp;

        Gizmos.DrawLine(transform.localPosition, advance + r);
        Gizmos.DrawLine(transform.localPosition, advance + l);
        Gizmos.DrawLine(transform.localPosition, advance + u);
        Gizmos.DrawLine(transform.localPosition, advance + d);

        Gizmos.DrawLine(advance + l + u, advance + r + u);
        Gizmos.DrawLine(advance + l + u, advance + l + d);

        Gizmos.DrawLine(advance + l + d, advance + r + d);
        Gizmos.DrawLine(advance + r + u, advance + r + d);

        Gizmos.DrawWireSphere(transform.localPosition + advance, amp);
    }


    //        void OnTriggerStay(Collider other){
// 
//            if (other.gameObject.Tag == "Player")
//            {
//                Debug.Log ("Success!");
//            }
//       }
}
