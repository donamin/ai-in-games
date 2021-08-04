using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingObject : MonoBehaviour
{
    public float v0_x, v0_z;
    public float g = -10;

    public LineRenderer lineRenderer;

    bool throwing = false, throwFinished;

    Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {   
    }

    // Update is called once per frame
    void Update()
    {
        if (!throwing && Input.GetKeyDown(KeyCode.Space))
        {
            //Start throwing the ball if space is pressed
            throwing = true;
            throwFinished = false;

            //Initialize the velocity vector
            velocity.x = v0_x;
            velocity.z = v0_z;
        }

        if(throwing && !throwFinished)
        {
            //Update the position if the throw is not finished yet.
            transform.position += velocity * Time.deltaTime;

            //Stop throwing if z becomes zero or negative
            if (transform.position.z <= 0)
                throwFinished = true;

            //Update the z component of the velocity (the x component does not need to updated since its acceleration is zero)
            //ToDo: velocity.z += ;
        }

        float t_end = 0;
        //ToDo: t_end = ; //The time after which the throwing is finished

        //Draw the X & Z axes.
        int nPoints = 200;
        lineRenderer.positionCount = nPoints + 5;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, new Vector3(100, 0, 0));
        lineRenderer.SetPosition(2, Vector3.zero);
        lineRenderer.SetPosition(3, new Vector3(0, 0, 100));
        lineRenderer.SetPosition(4, Vector3.zero);

        //Draw the throwing curve based on the initial velocity and g values.
        for (int i = 0; i < nPoints; i++)
        {
            float t = Mathf.Lerp(0, t_end, (float)(i) / nPoints);
            float x = 0, z = 0;
            //ToDo: x = ; //The X component at time t
            //ToDo: z = ; //The Z component at time t
            lineRenderer.SetPosition(i + 5, new Vector3(x, 0, z));
        }
    }
}