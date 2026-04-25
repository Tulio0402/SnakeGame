using UnityEngine;
using System.Collections.Generic;

public class Food : MonoBehaviour
{
    public BoxCollider2D gridArea;
    private Snake snakeScript;

    private void Start()
    {
        // 尋找蛇的組件
        snakeScript = FindFirstObjectByType<Snake>();
        RandomizePosition();
    }

    // 將此改為 public，讓 Snake 腳本在手動碰撞時可以呼叫
    public void RandomizePosition()
    {
        Bounds bounds = this.gridArea.bounds;
        Vector3 newPosition;
        bool isInvalid;
        int safetyNet = 0;

        do
        {
            isInvalid = false;
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            newPosition = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);

            if (snakeScript != null)
            {
                // 1. 檢查是否在蛇身上
                foreach (Transform segment in snakeScript.Segments)
                {
                    if (Vector3.Distance(segment.position, newPosition) < 0.1f)
                    {
                        isInvalid = true;
                        break;
                    }
                }

                // 2. 核心改進：檢查是否在蛇頭「下一步」的位置，防止蘋果瞬間出現在正前方導致穿過
                Vector3 nextHeadPos = snakeScript.transform.position + (Vector3)snakeScript.CurrentDirection;
                if (Vector3.Distance(nextHeadPos, newPosition) < 0.1f)
                {
                    isInvalid = true;
                }
            }

            safetyNet++;
            // 安全機制，防止死循環
            if (safetyNet > 100) break;

        } while (isInvalid);

        this.transform.position = newPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RandomizePosition();
        }
    }
}