using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : BaseSteeringBehavior
{
    public Kinematics target;

    public float maxAngularAcceleration = 90.0f;
    public float maxRotation = 45.0f;

    public float targetRadius = 2.5f;
    public float slowRadius = 15.0f;

    public float timeToTarget = 0.1f;

    public override SteeringOutput GetSteering()
    {
        SteeringOutput steering;
        steering.linear = Vector3.zero;
        steering.angular = 0;

        if (target)
        {
            float rotation = target.orientation - character.orientation;
            if (rotation > 180)
                rotation = rotation - 360;
            else if (rotation < -180)
                rotation = rotation + 360;
            //print(rotation);
            float rotationSize = Mathf.Abs(rotation);

            if (rotationSize < targetRadius)
            {
                return steering;
            }

            float targetRotation = 0;
            if (rotationSize > slowRadius)
            {
                targetRotation = maxRotation;
            }
            else
            {
                //targetRotation = maxRotation * rotationSize / slowRadius;
                targetRotation = maxRotation * (rotationSize - targetRadius) / (slowRadius - targetRadius);
            }

            targetRotation *= Mathf.Sign(rotation);

            steering.angular = targetRotation - character.rotation;
            steering.angular /= timeToTarget;

            float angularAcceleration = Mathf.Abs(steering.angular);
            if (angularAcceleration > maxAngularAcceleration)
            {
                steering.angular /= angularAcceleration;
                steering.angular *= maxAngularAcceleration;
            }
        }

        return steering;
    }
}