using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] TMP_InputField Log_Email;
    [SerializeField] TMP_InputField Log_Password;

    public async void OnLoginPressed()
    {
        if (string.IsNullOrWhiteSpace(Log_Email.text))
        {
            Debug.LogError("Please enter a valid email!");
            return;
        }

        if (string.IsNullOrWhiteSpace(Log_Password.text))
        {
            Debug.LogError("Please enter a valid password!");
            return;
        }

        (bool success, string user_id) = await MySQLManager.LoginUser(Log_Email.text, Log_Password.text);

        if (success)
        {
            print("Successfully logged in " + user_id + "!");
            await SceneManager.LoadSceneAsync("MainMenu");
        }
        else
            print("Failed to log in User!");
    }

    public void GoToSignup()
    {
        SceneManager.LoadSceneAsync("Signup");
    }
}
