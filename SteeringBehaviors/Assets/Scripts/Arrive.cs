using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : BaseSteeringBehavior
{
    public GameObject target;
    
    public float maxAcceleration = 2.5f;
    public float maxSpeed = 10.0f;

    public float targetRadius = 3.0f;
    public float slowRadius = 10.0f;
    public float timeToTarget = 0.1f;

    public override SteeringOutput GetSteering()
    {
        SteeringOutput steering;
        steering.linear = Vector3.zero;
        steering.angular = 0;

        return steering;
    }
}