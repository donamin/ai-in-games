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

        float norm_manual = Mathf.Sqrt(Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.z, 2));
        float norm_unity = transform.position.magnitude;
        print(norm_manual - norm_unity);
        float sqr_norm_unity = transform.position.sqrMagnitude;

        if (norm_unity < 2)
            print("do something!");
        if (sqr_norm_unity < 4)
            print("do something faster!");

        Vector3 position_unit = Vector3.forward;
        if(norm_manual > Mathf.Epsilon)
            position_unit = transform.position / norm_manual;
        //position_unit = transform.position.normalized;
        Debug.DrawLine(Vector3.zero, position_unit, Color.green);
    }
}