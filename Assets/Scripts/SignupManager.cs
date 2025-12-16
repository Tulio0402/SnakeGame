using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignupManager : MonoBehaviour
{
    [Header("Register")]
    [SerializeField] TMP_InputField Reg_Email;
    [SerializeField] TMP_InputField Reg_ID;
    [SerializeField] TMP_InputField Reg_Password;

    public async void OnRegisterPressed()
    {
        if (string.IsNullOrWhiteSpace(Reg_Email.text))
        {
            Debug.LogError("Please enter a valid email!");
            return;
        }

        if (string.IsNullOrWhiteSpace(Reg_ID.text) || Reg_ID.text.Length < 7)
        {
            Debug.LogError("Please enter a valid ID!");
            return;
        }

        if (string.IsNullOrWhiteSpace(Reg_Password.text))
        {
            Debug.LogError("Please enter a valid password!");
            return;
        }

        if(await MySQLManager.RegisterUser(Reg_Email.text, Reg_ID.text, Reg_Password.text))
            print("Successfully Registered!");
        else
            print("Failed to Register User!");
    }

    public void GoToLogin()
    {
        SceneManager.LoadSceneAsync("Login");
    }
}