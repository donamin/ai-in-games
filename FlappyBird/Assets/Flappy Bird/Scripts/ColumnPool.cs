using UnityEngine;
using System.Collections;

public class ColumnPool : MonoBehaviour
{
    public static ColumnPool _;
    public GameObject[] columns;
    [HideInInspector]
    public Transform[] columnsTransforms;

    public float distBetweenColumns = 7.5f;
    public float columnMin = -1f;
	public float columnMax = 3.5f;

    [HideInInspector]
    public int currentColumn = 0;
    float spawnXOffset = 12f;
    [HideInInspector]
    public float distSinceLastSpawned;

    [HideInInspector]
    public Vector3 scoreRectOffset, scoreRectSize;
    [HideInInspector]
    public Vector3[] columnRectOffsets, columnRectSizes;

    private void Awake()
    {
        _ = this;
        distSinceLastSpawned = distBetweenColumns * 0.9f;
        
        columnsTransforms = new Transform[columns.Length];
        for (int i = 0; i < columns.Length; i++)
        {
            columnsTransforms[i] = columns[i].transform;
        }

        Bounds bounds = columns[0].GetComponent<BoxCollider2D>().bounds;
        scoreRectOffset = bounds.min - columnsTransforms[0].position;
        scoreRectSize = bounds.max - bounds.min;

        columnRectOffsets = new Vector3[2];
        columnRectSizes = new Vector3[2];
        for (int i = 0; i < 2; i++)
        {
            bounds = columns[0].transform.GetChild(i).GetComponent<BoxCollider2D>().bounds;
            columnRectOffsets[i] = bounds.min - columnsTransforms[0].position;
            columnRectSizes[i] = bounds.max - bounds.min;
        }
    }

    private void FixedUpdate()
    {
        if (MyGameManager.gameOver)
            return;
        distSinceLastSpawned += Time.fixedDeltaTime * MyGameManager._.scrollSpeed;
		if (distSinceLastSpawned >= distBetweenColumns) 
		{
            distSinceLastSpawned = 0f;
			columns[currentColumn].transform.position = NewSpawnPos(Bird._.transform.position.x);
			currentColumn = (currentColumn + 1) % columns.Length;
        }
    }

    public Vector2 NewSpawnPos(float birdX)
    {
        return new Vector2(birdX + spawnXOffset, Random.Range(columnMin, columnMax));
    }
}