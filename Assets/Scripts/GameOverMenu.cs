using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void Restart()
    {
        ScoreManager.score = 0;
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void Home()
    {
        ScoreManager.score = 0;
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
