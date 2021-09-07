using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    public GameObject[] waypoints;

    public float speed = 5.0f;
    public float minDistance = 1.0f;
    public float rotationSpeed = 10.0f;

    int currentWaypointIndex;
    GameObject currentWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentWaypoint(0);
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentWaypoint.transform.position);
        if(distanceToTarget < minDistance)
        {
            //0 -> 1
            //1 -> 2
            //2 -> 3
            //3 -> 0
            SetCurrentWaypoint((currentWaypointIndex + 1) % waypoints.Length);
        }

        Vector3 diff2Target = currentWaypoint.transform.position - transform.position;
        diff2Target.y = 0;

        Vector3 updatedPosition = Vector3.MoveTowards(transform.position, currentWaypoint.transform.position, speed * Time.deltaTime);
        //Vector3 updatedPosition = Vector3.Lerp(transform.position, currentWaypoint.transform.position, speed * Time.deltaTime);
        //Vector3 updatedPosition = transform.position + diff2Target.normalized * speed * Time.deltaTime;
        updatedPosition.y = transform.position.y;
        transform.position = updatedPosition;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(diff2Target), rotationSpeed * Time.deltaTime);
        //transform.forward = diff2Target;
    }

    void SetCurrentWaypoint(int index)
    {
        currentWaypointIndex = index;
        currentWaypoint = waypoints[index];
    }
}