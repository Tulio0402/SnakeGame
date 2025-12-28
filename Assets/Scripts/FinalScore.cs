using UnityEngine;
using TMPro;

public class FinalScore : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text rankText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        int finalScore = ScoreManager.score; 
        string user_id = MySQLManager.UserID;

        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + finalScore.ToString();
        }
        else
        {
            scoreText.text = "SCORE: 0";
        }

        // 執行 MySQLManager 中的上傳方法
        bool success = await MySQLManager.UpdateHighScore(user_id, finalScore.ToString());

        if (success)
        {
            print("Successfully updated high score for User: " + user_id + ", score: " + finalScore.ToString());
        }
        else
        {
            print("Fail to update high score!");
        }

        string rank = await MySQLManager.GetCurrentGameRank(finalScore);
        
        // 3. 更新 UI
        if (rank != "Error") {
            rankText.text = "RANK: #" + rank;
        } else {
            rankText.text = "RANK: -";
        }
    }
}