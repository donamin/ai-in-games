using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematics : MonoBehaviour
{
    BaseSteeringBehavior steeringBehavior;

    public Vector3 velocity;
    public float rotation;

    // Start is called before the first frame update
    void Start()
    {
        steeringBehavior = GetComponent<BaseSteeringBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}