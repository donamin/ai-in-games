using UnityEngine;
using UnityEngine.SceneManagement;

public class Trigger_LoadScene : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnTriggerEnter(Collider user)
	{
		if (user.tag == "Player")
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}