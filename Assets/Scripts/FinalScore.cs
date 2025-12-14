using UnityEngine;
using TMPro;

public class FinalScore : MonoBehaviour
{
    public TMP_Text scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int finalScore = ScoreManager.score; 

        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + finalScore.ToString();
        }
        else
        {
            scoreText.text = "SCORE: 0";
        }
    }
}