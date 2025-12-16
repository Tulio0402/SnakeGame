using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

static class MySQLManager
{
    readonly static string SERVER_URL = "localhost:80/SnakeGame_user_db";

    public static async Task<bool> RegisterUser(string email, string user_id, string password)
    {
        string REGISTER_USESR_URL = $"{SERVER_URL}/signup.php";

        return (await SendPostRequest(REGISTER_USESR_URL, new Dictionary<string, string>()
            {
                {"email", email},
                {"user_id", user_id},
                {"password", password}
            }))
            .success;
    }

    public static async Task<(bool success, string user_id)> LoginUser(string email, string password)
    {
        string LOGIN_USESR_URL = $"{SERVER_URL}/login.php";

        return await SendPostRequest(LOGIN_USESR_URL, new Dictionary<string, string>()
            {
                {"email", email},
                {"password", password}
            });
    }

    static async Task<(bool success, string returnMessage)> SendPostRequest(string url, Dictionary<string, string> data)
    {
        using(UnityWebRequest req = UnityWebRequest.Post(url, data))
        {
            await req.SendWebRequest();

            while(!req.isDone) await Task.Delay(100);

            // When the Task is done
            if(req.error != null
                || !string.IsNullOrWhiteSpace(req.error)
                || HasErrorMessage(req.downloadHandler.text))
                return (false, req.downloadHandler.text);

            // On Success
            return (true, req.downloadHandler.text);
        }
    }

    static bool HasErrorMessage(string msg) => int.TryParse(msg, out var res);
}
