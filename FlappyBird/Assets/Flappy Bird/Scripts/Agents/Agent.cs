using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public enum Action { None = 0, LeftClick, Count, UNKNOWN}

    public virtual Action GetAction()
    {
        print("Calling GetAction() from base Agent class. Are you forgetting to override GetAction() in your agent class?");
        return Action.None;
    }
}