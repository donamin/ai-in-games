using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour
{
    public static Bird _;

    public bool controlledByPlayer = true;
    Agent.Action aIAction = Agent.Action.UNKNOWN;

    Agent agent;

    Animator anim;
    public float tapSpeed = 3.75f;
    [HideInInspector]
    public float verticalSpeed = 0;

    [HideInInspector]
    public Vector2 colliderSize;

    float cameraOffsetX;

    void Awake()
    {
        _ = this;
        if (!controlledByPlayer)
        {
            agent = GetComponent<Agent>();
        }
        anim = GetComponent<Animator>();
        colliderSize = GetComponent<BoxCollider2D>().size;
        cameraOffsetX = Camera.main.transform.position.x - transform.position.x;
    }

    private void FixedUpdate()
    {
        if (controlledByPlayer)
            return;
        //AI decision making is managed here so we make sure that AI makes 30 decisions per second.
        if (MyGameManager.gameOver)
            aIAction = Agent.Action.LeftClick; //We should click to restart game
        else
            aIAction = agent.GetAction();
    }

    private void Update()
    {
        if (MyGameManager.gameOver)
        {
            if (Clicked())
                MyGameManager.ResetGame();
            return;
        }
        
        if (Clicked())
        {
            anim.SetTrigger("Flap");
            verticalSpeed = tapSpeed;
        }
        else
        {
            verticalSpeed += Physics2D.gravity.y * Time.deltaTime;
        }
        transform.Translate((Vector3.right * MyGameManager._.scrollSpeed + Vector3.up * verticalSpeed) * Time.deltaTime);
        Camera.main.transform.position = new Vector3(transform.position.x + cameraOffsetX, 0, Camera.main.transform.position.z);
        aIAction = Agent.Action.UNKNOWN; //We make it UNKNOWN to make sure that one AI action doesn't execute more than once between 2 fixed updates
    }

    bool Clicked()
    {
        if (controlledByPlayer)
            return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
        return aIAction == Agent.Action.LeftClick;
    }

    public void Died()
    {
        anim.SetTrigger("Die");
        MyGameManager._.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string t = collision.gameObject.tag;
        if (t == "Score")
        {
            MyGameManager._.Scored();
        }
        else if(t == "Roof")
        {
            verticalSpeed = 0;
            transform.position = new Vector3(transform.position.x, 4.8f, transform.position.z);
        }
        else
        {
            Died();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string t = collision.gameObject.tag;
        if (t == "Score")
        {
        }
        else if (t == "Roof")
        {
            verticalSpeed = 0;
            transform.position = new Vector3(transform.position.x, 4.8f, transform.position.z);
        }
        else
        {
            Died();
        }
    }
}