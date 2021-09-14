using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MyGameManager : MonoBehaviour
{
    public static MyGameManager _;
    
    public Text scoreText;
    public GameObject gameOverText;

    public static int score = 0;
    public static bool gameOver = false;
    public float scrollSpeed = 2.5f;
    public float scrollAcceleration = 0.1f;

    void Awake()
    {
        _ = this;
    }

    void Update()
    {
        scrollSpeed += Time.deltaTime * scrollAcceleration;
    }
    
    public void Scored()
    {
        if (gameOver)
            return;
        score++;
        scoreText.text = "Score: " + score.ToString();
    }

    public void GameOver()
    {
        gameOverText.SetActive(true);
        gameOver = true;
    }

    static float sum = 0;
    static int count = 0;
    static int max = 0;
    public static void ResetGame()
    {
        if (count == 20)
            return;
        count++;
        sum += score;
        if (score > max)
            max = score;
        print("Scores after " + count + " rounds:\n\tCurrent: " + score + "\tMax: " + max + "\tAverage: " + sum / count);
        score = 0;
        gameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}