using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI[] scoreTexts; // 拖入 5 個 Score Text
    [SerializeField] private TextMeshProUGUI[] IDTexts;  // 拖入 5 個 Date Text

    private async void OnEnable() {
        string response = await MySQLManager.GetLeaderboard();

        // 先把 UI 欄位清空
        foreach (var t in scoreTexts) t.text = "-";
        foreach (var t in IDTexts) t.text = "-";

        if (response == "Empty" || response == "Error") return;

        // 1. 先用 | 拆出每一列資料
        string[] rows = response.Split('|');

        for (int i = 0; i < rows.Length && i < 5; i++) {
            // 2. 再用 , 拆出分數與日期
            string[] details = rows[i].Split(',');
            
            if (details.Length == 2) {
                scoreTexts[i].text = details[0]; // 分數
                IDTexts[i].text = details[1];    // 使用者 ID
            }
        }
    }
}