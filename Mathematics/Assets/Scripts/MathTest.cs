using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(Vector3.zero, transform.position, Color.white);

        float norm_manual = Mathf.Sqrt(Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.y, 2) + Mathf.Pow(transform.position.z, 2));
        //print(norm_manual);
        float norm_unity = transform.position.magnitude;
        //print(norm_unity);
        //print(Mathf.Abs(norm_unity - norm_manual));

        float sqr_norm_manual = Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.y, 2) + Mathf.Pow(transform.position.z, 2);
        float sqr_norm_unity = transform.position.sqrMagnitude;

        /*if (norm_unity < 2)
            print("do something");
        if (sqr_norm_unity < 4)
            print("do the same thing faster");*/

        Vector3 pos_unit_manual = transform.position / (norm_manual > 0 ? norm_manual : 1);
        //Debug.DrawLine(Vector3.zero, pos_unit_manual, Color.green);
        Vector3 pos_unit_unity = transform.position.normalized;
        Debug.DrawLine(Vector3.zero, pos_unit_unity, Color.green);
    }
}