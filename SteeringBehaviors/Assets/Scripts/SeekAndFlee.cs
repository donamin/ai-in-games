using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAndFlee : BaseSteeringBehavior
{
    public GameObject target;
    public bool flee;
    public float maxAcceleration = 2.5f;

    public override SteeringOutput GetSteering()
    {
        SteeringOutput steering;
        steering.linear = Vector3.zero;
        steering.angular = 0;

        return steering;
    }
}