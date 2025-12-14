using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        ScoreManager.score = 0;
        SceneManager.LoadSceneAsync("GameScene");
    }
}
