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

        if(target)
        {
            Vector3 direction = target.transform.position - character.transform.position;
            direction.y = 0;
            float distance = direction.magnitude;

            if(distance < targetRadius)
            {
                return steering;
            }

            float targetSpeed = 0;
            if(distance > slowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                //targetSpeed = maxSpeed * distance / slowRadius;
                //distance is between targetRadius and slowRadius
                targetSpeed = maxSpeed * (distance - targetRadius) / (slowRadius - targetRadius);
            }

            Vector3 targetVelocity = direction;
            targetVelocity.Normalize();
            targetVelocity *= targetSpeed;

            steering.linear = targetVelocity - character.velocity;
            steering.linear /= timeToTarget;
            steering.linear.y = 0;

            if(steering.linear.magnitude > maxAcceleration)
            {
                steering.linear.Normalize();
                steering.linear *= maxAcceleration;
            }
        }

        return steering;
    }
}