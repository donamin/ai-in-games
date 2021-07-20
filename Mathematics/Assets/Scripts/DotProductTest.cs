using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotProductTest : MonoBehaviour
{
    public GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 25, Color.blue);
        Debug.DrawLine(transform.position, enemy.transform.position, Color.red);

        Vector3 a = transform.forward;
        Vector3 b = enemy.transform.position - transform.position;

        float dot = a.x * b.x + a.z * b.z; // = Vector3.Dot(a, b);
        //print(dot);

        float alpha = Mathf.Acos(dot / (a.magnitude * b.magnitude)) * Mathf.Rad2Deg;
        if (Vector3.Dot(transform.right, b) > 0) //Enemy is on the right side.
            alpha = alpha * -1; //alpha *= -1;
        print(alpha);

        Vector3 p = b;
        Vector3 q = transform.right; // = Vector3.Cross(transform.forward, transform.up);
        Vector3 proj = Vector3.Dot(p, q) / q.sqrMagnitude * q;
        Debug.DrawLine(transform.position, transform.position + proj, Color.green);
    }
}