using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SteeringOutput
{
    public Vector3 linear;
    public float angular;
}

public class BaseSteeringBehavior : MonoBehaviour
{
    public Kinematics character;

    public void Start()
    {
        character = gameObject.GetComponent<Kinematics>();
    }

    public virtual SteeringOutput GetSteering()
    {
        SteeringOutput steering;
        steering.linear = Vector3.zero;
        steering.angular = 0;
        return steering;
    }
}