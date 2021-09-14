using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCTS_TreeNode
{
    const float sqrt2 = 1.41421f;
    const int numActions = (int)Agent.Action.Count;

    MCTS_TreeNode parent;
    MCTS_TreeNode[] children;
    float qValue;
    int nVisits;
    int depth;
    
    int index;

    static WorldState currentState;
    static List<Vector3> birdPosList;

    float[] bounds = new float[] { float.MaxValue, -float.MaxValue };

    public MCTS_TreeNode(MCTS_TreeNode parent = null, int index = -1)
    {
        children = new MCTS_TreeNode[numActions];
        qValue = 0;
        nVisits = 0;
        depth = 0;
        this.parent = parent;
        this.index = index;
        if (parent != null)
            depth = parent.depth + 1;
        if (currentState == null)
            currentState = new WorldState();
        if (birdPosList == null)
            birdPosList = new List<Vector3>();
    }
    
    public void mctsSearch(int numIterations)
    {
        MyLineRenderer.Init();
        for(int i = 0; i < numIterations; i++)
        {
            currentState.Save();
            birdPosList = new List<Vector3>();
            birdPosList.Add(currentState.birdPos);
            MCTS_TreeNode selected = treePolicy();
            float reward = selected.rollOut();
            backUp(selected, reward);
            MyLineRenderer.lineStrips.Add(birdPosList);
        }
    }

    MCTS_TreeNode treePolicy()
    {
        MCTS_TreeNode cur = this;
        while (!finishRollout(cur.depth))
        {
            if (cur.notFullyExpanded())
                return cur.expand();
            else
                cur = cur.uct();
        }
        return cur;
    }

    bool notFullyExpanded()
    {
        foreach (MCTS_TreeNode child in children)
            if (child == null)
                return true;
        return false;
    }

    MCTS_TreeNode expand()
    {
        int action = 0;
        float bestRand = -1;
        for (int i = 0; i < numActions; i++)
        {
            float x = Random.Range(0.0f, 1.0f);
            if (x > bestRand && children[i] == null)
            {
                bestRand = x;
                action = i;
            }
        }
        
        currentState.SimulateForward((Agent.Action)(action));
        birdPosList.Add(currentState.birdPos);

        MCTS_TreeNode newChild = new MCTS_TreeNode(this, action);
        children[action] = newChild;
        
        return newChild;
    }

    MCTS_TreeNode uct()
    {
        MCTS_TreeNode selected = null;
        //Choose best action using UCT
        float bestValue = -float.MaxValue;
        foreach (MCTS_TreeNode child in children)
        {
            float uctValue = child.qValue / child.nVisits;
            uctValue = Normalize(uctValue, bounds[0], bounds[1]);
            uctValue = uctValue + sqrt2 * Mathf.Sqrt(Mathf.Log(nVisits + 1) / child.nVisits);
            uctValue = AddNoise(uctValue);
            if (uctValue > bestValue)
            {
                selected = child;
                bestValue = uctValue;
            }
        }
        if (selected == null)
        {
            Debug.Log("Warning! returning null...");
        }

        //Roll the state:
        currentState.SimulateForward((Agent.Action)(selected.index));
        birdPosList.Add(currentState.birdPos);

        return selected;
    }

    float rollOut()
    {
        int thisDepth = depth;
        while (!finishRollout(thisDepth))
        {
            int action = Random.Range(0, numActions);
            action = Random.Range(0, 4) > 0 ? 0 : 1;
            currentState.SimulateForward((Agent.Action)(action));
            birdPosList.Add(currentState.birdPos);
            thisDepth++;
        }
        float delta = value(thisDepth);
        if (delta < bounds[0])
            bounds[0] = delta;
        if (delta > bounds[1])
            bounds[1] = delta;
        return delta;
    }

    bool finishRollout(int rollOutDepth)
    {
        return rollOutDepth >= Agent_MCTS._.rollOutDepth || currentState.gameOver;
    }

    float value(int rollOutDepth)
    {
        float value = currentState.score * 100 + rollOutDepth * 5;
        value -= 1 * Mathf.Pow(currentState.birdPos.y - 1.5f, 2); //Try to stay in the middle of the screen!
        if (currentState.gameOver)
            value -= 100000.0f;
        return value;
    }

    void backUp(MCTS_TreeNode node, float reward)
    {
        while (node != null)
        {
            node.nVisits++;
            node.qValue += reward;
            if (reward < node.bounds[0])
            {
                node.bounds[0] = reward;
            }
            if (reward > node.bounds[1])
            {
                node.bounds[1] = reward;
            }
            node = node.parent;
            reward *= Agent_MCTS._.lambda; //The effect of reward decreases exponentially as we get closer to the root
        }
    }

    static float Normalize(float value, float min, float max)
    {
        if(min < max)
            return (value - min) / (max - min);
        return value;
    }

    static float AddNoise(float input)
    {
        return input + Random.Range(-0.02f, 0.02f);
    }

    public int mostVisitedAction()
    {
        int selected = -1;
        float bestValue = -float.MaxValue;
        bool allEqual = true;
        float first = -1;

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != null)
            {
                if (first == -1)
                    first = children[i].nVisits;
                else if (first != children[i].nVisits)
                    allEqual = false;

                float childValue = children[i].nVisits;
                childValue = AddNoise(childValue);
                if (childValue > bestValue)
                {
                    bestValue = childValue;
                    selected = i;
                }
            }
        }

        if (allEqual)
        {
            //If all are equal, we opt to choose for the one with the best Q.
            selected = bestAction();
        }
        if (selected == -1)
        {
            Debug.Log("Unexpected selection!");
            selected = 0;
        }
        return selected;
    }

    public int bestAction()
    {
        int selected = -1;
        float bestValue = -float.MaxValue;

        string msg = "";
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != null)
            {
                float childValue = children[i].qValue / children[i].nVisits;
                childValue = AddNoise(childValue);
                msg += ", Action " + i + " value = " + childValue;
                if (childValue > bestValue)
                {
                    bestValue = childValue;
                    selected = i;
                }
            }
        }
        msg += " -> Selected " + selected;
        //Debug.Log(msg);
        if (selected == -1)
        {
            Debug.Log("Unexpected selection!");
            selected = 0;
        }
        return selected;
    }
}