using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.up;
    // 提供給 Food 腳本讀取當前方向，用來預判下一個格子
    public Vector2 CurrentDirection => _direction;

    private Queue<Vector2> _inputQueue = new Queue<Vector2>();
    private bool _shouldGrow = false;

    [Header("Prefabs")]
    public Transform straightPrefab;
    public Transform turnUpRightPrefab;
    public Transform turnUpLeftPrefab;
    public Transform turnDownRightPrefab;
    public Transform turnDownLeftPrefab;
    public Transform tailPrefab;
    
    public int initialSize = 3;

    private List<Transform> _segments;
    // 提供給 Food 腳本讀取全身座標，防止蘋果生成在蛇身上
    public List<Transform> Segments => _segments;

    private enum TurnType { UpLeft, UpRight, DownLeft, DownRight, LeftUp, LeftDown, RightUp, RightDown }
    private Dictionary<Vector3, TurnType> _turnPoints;

    private void Start()
    {
        _segments = new List<Transform>();
        _segments.Add(this.transform);
        _turnPoints = new Dictionary<Vector3, TurnType>();

        for (int i = 1; i < this.initialSize; i++)
        {
            Transform segment = Instantiate(this.straightPrefab);
            segment.position = new Vector3(0, -i, 0);
            _segments.Add(segment);
        }
        UpdateBodyVisuals();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) EnqueueInput(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.S)) EnqueueInput(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.A)) EnqueueInput(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.D)) EnqueueInput(Vector2.right);
    }

    private void EnqueueInput(Vector2 newDir)
    {
        if (_inputQueue.Count >= 2) return;
        Vector2 lastQueuedDir = (_inputQueue.Count > 0) ? GetLastInQueue() : _direction;
        if (newDir != -lastQueuedDir && newDir != lastQueuedDir)
        {
            _inputQueue.Enqueue(newDir);
        }
    }

    private Vector2 GetLastInQueue()
    {
        Vector2[] array = _inputQueue.ToArray();
        return array[array.Length - 1];
    }

    private void FixedUpdate()
    {
        Vector2 prevDirection = _direction;
        
        if (_inputQueue.Count > 0)
        {
            _direction = _inputQueue.Dequeue();
            UpdateHeadRotation();

            Vector3 turnPosition = GetRoundedPosition(this.transform.position);
            TurnType newTurn = GetTurnType(prevDirection, _direction);
            
            if (!_turnPoints.ContainsKey(turnPosition)) {
                _turnPoints.Add(turnPosition, newTurn);
            }
        }

        Vector3 oldTailPos = _segments[_segments.Count - 1].position;

        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        // 移動蛇頭
        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0.0f
        );

        // --- 核心改進：主動檢查碰撞，解決「穿過蘋果」問題 ---
        CheckForFoodManually();

        if (_shouldGrow)
        {
            Transform newSegment = Instantiate(straightPrefab);
            newSegment.position = oldTailPos; 
            _segments.Add(newSegment);
            _shouldGrow = false;
        }

        UpdateBodyVisuals();

        Vector3 currentTailPos = GetRoundedPosition(_segments[_segments.Count - 1].position);
        if (_turnPoints.ContainsKey(currentTailPos))
        {
            _turnPoints.Remove(currentTailPos);
        }
    }

    private void CheckForFoodManually()
    {
        // 檢查蛇頭所在位置是否有標記為 Food 的物件
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f);
        if (hit != null && hit.CompareTag("Food"))
        {
            Grow();
            // 找到 Food 組件並強制它立刻換位置
            Food food = hit.GetComponent<Food>();
            if (food != null) food.RandomizePosition();
        }
    }

    private void UpdateHeadRotation()
    {
        float angle = (_direction == Vector2.up) ? 0 : (_direction == Vector2.down) ? 180 : (_direction == Vector2.left) ? 90 : -90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateBodyVisuals()
    {
        for (int i = 1; i < _segments.Count; i++)
        {
            Transform segment = _segments[i];
            Vector3 currentPos = GetRoundedPosition(segment.position);
            bool isTail = (i == _segments.Count - 1);

            if (!isTail && _turnPoints.ContainsKey(currentPos))
            {
                TurnType turn = _turnPoints[currentPos];
                Transform turnPrefab = GetTurnPrefab(turn);
                if (segment.name != turnPrefab.name + "(Clone)") ReplaceSegment(i, turnPrefab, currentPos);
            }
            else
            {
                Transform targetPrefab = isTail ? tailPrefab : straightPrefab;
                if (segment.name != targetPrefab.name + "(Clone)") ReplaceSegment(i, targetPrefab, currentPos);
                UpdateSegmentRotation(i);
            }
        }
    }

    private void ReplaceSegment(int index, Transform newPrefab, Vector3 position)
    {
        Destroy(_segments[index].gameObject);
        Transform newSegment = Instantiate(newPrefab);
        newSegment.position = position;
        _segments[index] = newSegment;
    }

    private void UpdateSegmentRotation(int index)
    {
        Transform segment = _segments[index];
        Vector3 currentPos = GetRoundedPosition(segment.position);
        Vector3 prevPos = GetRoundedPosition(_segments[index - 1].position);
        Vector3 diff = prevPos - currentPos;

        if (diff.x > 0.5f) segment.rotation = Quaternion.Euler(0, 0, -90);
        else if (diff.x < -0.5f) segment.rotation = Quaternion.Euler(0, 0, 90);
        else if (diff.y > 0.5f) segment.rotation = Quaternion.Euler(0, 0, 0);
        else if (diff.y < -0.5f) segment.rotation = Quaternion.Euler(0, 0, 180);
    }

    private Vector3 GetRoundedPosition(Vector3 position) => new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), 0);

    private TurnType GetTurnType(Vector2 prev, Vector2 next)
    {
        if (prev == Vector2.up) return next == Vector2.left ? TurnType.UpLeft : TurnType.UpRight;
        if (prev == Vector2.down) return next == Vector2.left ? TurnType.DownLeft : TurnType.DownRight;
        if (prev == Vector2.left) return next == Vector2.up ? TurnType.LeftUp : TurnType.LeftDown;
        return next == Vector2.up ? TurnType.RightUp : TurnType.RightDown;
    }

    private Transform GetTurnPrefab(TurnType turn)
    {
        switch (turn) {
            case TurnType.UpRight: case TurnType.LeftDown: return turnUpRightPrefab;
            case TurnType.UpLeft: case TurnType.RightDown: return turnUpLeftPrefab;
            case TurnType.DownRight: case TurnType.LeftUp: return turnDownRightPrefab;
            case TurnType.DownLeft: case TurnType.RightUp: return turnDownLeftPrefab;
            default: return turnUpRightPrefab;
        }
    }

    public void Grow()
    {
        _shouldGrow = true;
        if (ScoreManager.instance != null) ScoreManager.instance.AddPoint();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food")) Grow();
        else if (other.CompareTag("Wall") || other.CompareTag("Body"))
        {
            SceneManager.LoadScene("GameOverMenu");
        }
    }
}