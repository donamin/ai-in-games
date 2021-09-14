using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState
{
    //World
    public int score;
    public bool gameOver;
    float scrollSpeed;
    Vector3[] columnsPositions;
    bool[] columnsGotScore;
    int currentColumn;
    float distSinceLastSpawned;

    //Bird
    public Vector3 birdPos;
    float verticalSpeed;

    public WorldState()
    {
        columnsPositions = new Vector3[ColumnPool._.columns.Length];
        columnsGotScore = new bool[ColumnPool._.columns.Length];
    }

    public void Save()
    {
        score = MyGameManager.score;
        gameOver = MyGameManager.gameOver;
        scrollSpeed = MyGameManager._.scrollSpeed;
        for (int i = 0; i < columnsPositions.Length; i++)
        {
            columnsPositions[i] = ColumnPool._.columnsTransforms[i].position;
            columnsGotScore[i] = false;
        }
        currentColumn = ColumnPool._.currentColumn;
        distSinceLastSpawned = ColumnPool._.distSinceLastSpawned;

        birdPos = Bird._.transform.position;
        verticalSpeed = Bird._.verticalSpeed;
    }
    
    public void SimulateForward(Agent.Action action)
    {
        if (gameOver)
            return;
        scrollSpeed += Time.fixedDeltaTime * MyGameManager._.scrollAcceleration;

        if (action == Agent.Action.LeftClick)
        {
            verticalSpeed = Bird._.tapSpeed;
        }
        else
        {
            verticalSpeed += Physics2D.gravity.y * Time.fixedDeltaTime;
        }
        birdPos += (Vector3.right * scrollSpeed + Vector3.up * verticalSpeed) * Time.fixedDeltaTime;
        if (birdPos.y > 4.8f)
            birdPos.y = 4.8f;

        distSinceLastSpawned += Time.fixedDeltaTime * scrollSpeed;
        if (distSinceLastSpawned >= ColumnPool._.distBetweenColumns)
        {
            distSinceLastSpawned = 0f;
            columnsPositions[currentColumn] = ColumnPool._.NewSpawnPos(birdPos.x);
            currentColumn = (currentColumn + 1) % columnsPositions.Length;
        }

        CheckCollisions();
    }

    void CheckCollisions()
    {
        Rect birdRect = new Rect(birdPos.x - Bird._.colliderSize.x / 2.0f, birdPos.y - Bird._.colliderSize.y / 2.0f, Bird._.colliderSize.x, Bird._.colliderSize.y);
        for (int i = 0; i < columnsPositions.Length; i++)
        {
            Rect scoreRect = new Rect(columnsPositions[i] + ColumnPool._.scoreRectOffset, ColumnPool._.scoreRectSize);
            if (birdRect.Overlaps(scoreRect) && !columnsGotScore[i])
            {
                score++;
                columnsGotScore[i] = true;
            }
            for (int c = 0; c < 2; c++)
            {
                Rect colRect = new Rect(columnsPositions[i] + ColumnPool._.columnRectOffsets[c] - new Vector3(0.5f, 0.15f, 0), ColumnPool._.columnRectSizes[c] + new Vector3(1.0f, 0.3f, 0));
                //Rect colRect = new Rect(columnsPositions[i] + ColumnPool._.columnRectOffsets[c], ColumnPool._.columnRectSizes[c]);
                if (birdRect.Overlaps(colRect))
                {
                    gameOver = true;
                    return;
                }
            }
        }
        if(birdPos.y < -1.9f)
        {
            gameOver = true;
            return;
        }
    }
}