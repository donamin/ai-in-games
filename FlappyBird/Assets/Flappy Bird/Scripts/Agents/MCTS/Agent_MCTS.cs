using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_MCTS : Agent
{
    public static Agent_MCTS _;

    public int numIterations = 50;
    public int rollOutDepth = 15;
    public float lambda = 0.95f;

    MCTS_TreeNode mctsRootNode;

    private void Start()
    {
        _ = this;
    }

    public override Action GetAction()
    {
        mctsRootNode = new MCTS_TreeNode();
        mctsRootNode.mctsSearch(numIterations);
        int actionIdx = mctsRootNode.bestAction();
        return (Action)(actionIdx);
    }
}