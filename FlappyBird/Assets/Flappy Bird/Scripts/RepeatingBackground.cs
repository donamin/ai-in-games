using UnityEngine;
using System.Collections;

public class RepeatingBackground : MonoBehaviour
{
    public static RepeatingBackground _;
    float groundHorizontalLength;

    [HideInInspector]
    public GameObject[] backgrounds;
    [HideInInspector]
    public Transform[] backgroundsTransforms;
    [HideInInspector]
    public Rigidbody2D[] backgroundsRigidbodies;
    [HideInInspector]
    public int leftmostBackground = 0;

    private void Awake ()
	{
        _ = this;
        backgrounds = new GameObject[2];
        backgrounds[0] = transform.GetChild(0).gameObject;
        backgrounds[1] = transform.GetChild(1).gameObject;
        backgroundsTransforms = new Transform[backgrounds.Length];
        backgroundsRigidbodies = new Rigidbody2D[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgroundsTransforms[i] = backgrounds[i].GetComponent<Transform>();
            backgroundsRigidbodies[i] = backgrounds[i].GetComponent<Rigidbody2D>();
        }

        BoxCollider2D groundCollider = backgrounds[0].GetComponent<BoxCollider2D> ();
		groundHorizontalLength = groundCollider.size.x;
    }

    private void FixedUpdate()
    {
        if (backgrounds[leftmostBackground].transform.position.x + groundHorizontalLength / 2.0f < Camera.main.transform.position.x - Statics.worldScreenWidth / 2.0f)
        {
            backgrounds[leftmostBackground].transform.position = backgrounds[1 - leftmostBackground].transform.position + new Vector3(groundHorizontalLength, 0, 0);
            leftmostBackground = 1 - leftmostBackground;
        }
    }
}