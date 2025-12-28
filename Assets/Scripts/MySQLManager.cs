using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

static class MySQLManager
{
    public static string UserID;

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

    public static async Task<bool> UpdateHighScore(string user_id, string score)
    {
        string REGISTER_USESR_URL = $"{SERVER_URL}/save_scores.php";

        return (await SendPostRequest(REGISTER_USESR_URL, new Dictionary<string, string>()
            {
                {"user_id", user_id},
                {"score", score}
            }))
            .success;
    }

    public static async Task<string> GetHighScores(string user_id) {
        WWWForm form = new WWWForm();
        form.AddField("user_id", user_id);

        using (UnityWebRequest req = UnityWebRequest.Post($"{SERVER_URL}/get_high_scores.php", form)) {
            await req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success) {
                return req.downloadHandler.text; // 拿到 "100,2025-12-17|90,2025-12-16..."
            }
        }
        return "Error";
    }

    public static async Task<string> GetLeaderboard() {
        using (UnityWebRequest req = UnityWebRequest.Get($"{SERVER_URL}/get_leaderboard.php")) {
            await req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success) {
                return req.downloadHandler.text;
            }
        }
        return "Error";
    }

    public static async Task<string> GetCurrentGameRank(int score) {
        WWWForm form = new WWWForm();
        form.AddField("current_score", score);

        using (UnityWebRequest req = UnityWebRequest.Post($"{SERVER_URL}/get_current_rank.php", form)) {
            await req.SendWebRequest();
            
            if (req.result == UnityWebRequest.Result.Success) {
                return req.downloadHandler.text; 
            }
        }
        return "Error";
    }

    static async Task<(bool success, string returnMessage)> SendPostRequest(string url, Dictionary<string, string> data)
    {
        using(UnityWebRequest req = UnityWebRequest.Post(url, data))
        {
            await req.SendWebRequest();

            while(!req.isDone) await Task.Delay(100);

            // When the Task is done
            if(req.error != null || !string.IsNullOrWhiteSpace(req.error))
                return (false, req.downloadHandler.text);

            // On Success
            return (true, req.downloadHandler.text);
        }
    }
}
