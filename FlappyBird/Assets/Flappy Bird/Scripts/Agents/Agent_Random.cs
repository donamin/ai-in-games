using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_Random : Agent
{
    public override Action GetAction()
    {
        return Random.Range(0, 2) == 0 ? Action.LeftClick : Action.None;
    }
}