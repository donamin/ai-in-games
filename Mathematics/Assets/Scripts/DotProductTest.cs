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

        float dot_manual = a.x * b.x + a.y * b.y + a.z * b.z;
        //print(dot_manual);
        float dot_unity = Vector3.Dot(a, b);
        //print(dot_unity);

        float alpha = Mathf.Acos(dot_manual / (a.magnitude * b.magnitude)) * Mathf.Rad2Deg;
        if (Vector3.Dot(b, transform.right) > 0)
            alpha = -alpha;
        //print(alpha);

        Vector3 p = b;
        Vector3 q = transform.right;
        Vector3 proj = Vector3.Dot(p, q) / q.sqrMagnitude * q;
        Debug.DrawLine(transform.position, transform.position + proj, Color.green);
    }
}