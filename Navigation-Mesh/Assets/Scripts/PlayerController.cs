using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    NavMeshPath path;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo))
            {
                //TODO: Your code here (Q1): Move towards hitInfo.point only if that point is reachable.
                //Hint: Use: NavMeshAgent.CalculatePath() function and the NavMeshPath variable defined in Line 9.
                agent.SetDestination(hitInfo.point);
            }
        }
        //TODO: Your code here (Q2): Stop the character when the player right clicks.
        
        //TODO: Your code here (Q3): Use Debug.DrawLine() function to draw the path calculated using the NavMeshAgent.CalculatePath() function above.
        
    }
}