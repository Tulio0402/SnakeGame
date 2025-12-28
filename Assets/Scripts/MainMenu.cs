using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        ScoreManager.score = 0;
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void HighScore()
    {
        SceneManager.LoadSceneAsync("HighScore");
    }

    public void Leaderboard()
    {
        SceneManager.LoadSceneAsync("Leaderboard");
    }

    public void LogOut()
    {
        SceneManager.LoadSceneAsync("Login");
    }

    public void Menu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
