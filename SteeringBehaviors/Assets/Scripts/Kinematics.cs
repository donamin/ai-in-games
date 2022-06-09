using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematics : MonoBehaviour
{
    BaseSteeringBehavior steeringBehavior;

    public float maxVelocity = 5;

    public float orientation = 0;

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
        if (steeringBehavior == null || !steeringBehavior.isActiveAndEnabled)
            return;

        transform.position += velocity * Time.deltaTime;

        if (steeringBehavior.ignoreRotation)
        {
            if (velocity.sqrMagnitude > 0)
                transform.forward = velocity;
        }
        else
        {
            orientation += rotation * Time.deltaTime;

            //Make sure that the orientation is always in [0, 360]
            if (orientation < 0)
                orientation += 360;
            if (orientation > 360)
                orientation -= 360;

            transform.forward = new Vector3(Mathf.Sin(orientation * Mathf.Deg2Rad), 0, Mathf.Cos(orientation * Mathf.Deg2Rad));
        }

        SteeringOutput steeringOutput = steeringBehavior.GetSteering();
        velocity += steeringOutput.linear * Time.deltaTime;
        rotation += steeringOutput.angular * Time.deltaTime;

        if(velocity.magnitude > maxVelocity)
        {
            velocity = velocity.normalized * maxVelocity;
        }

        //Return the object to the middle of the screen if it gets too far!
        if (transform.position.magnitude > 40)
        {
            transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f));
            transform.Rotate(Vector3.up, Random.Range(0.0f, 360.0f));

            velocity = Vector3.zero;
            rotation = 0;
        }
    }
}