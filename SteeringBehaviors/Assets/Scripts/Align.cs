using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : BaseSteeringBehavior
{
    public Kinematics target;

    public float maxAngularAcceleration = 90.0f;
    public float maxRotation= 45.0f;

    public float targetRadius = 2.5f;
    public float slowRadius = 15.0f;

    public float timeToTarget = 0.1f;

    public override SteeringOutput GetSteering()
    {
        SteeringOutput steering;
        steering.linear = Vector3.zero;
        steering.angular = 0;

        return steering;
    }
}