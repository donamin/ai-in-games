using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statics : MonoBehaviour
{
    public static float worldScreenWidth, worldScreenHeight;
    public static float worldScreenLeft, worldScreenRight, worldScreenTop, worldScreenBottom;

    void Awake()
    {
        worldScreenHeight = Mathf.Abs(Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)).y - Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y);

        Vector3 topLeft = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        Vector3 bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        
        worldScreenRight = topLeft.x;
        worldScreenTop = topLeft.y;
        worldScreenLeft = bottomRight.x;
        worldScreenBottom = bottomRight.y;

        worldScreenHeight = Mathf.Abs(topLeft.y - bottomRight.y);
        worldScreenWidth = Mathf.Abs(bottomRight.x - topLeft.x);
    }
}