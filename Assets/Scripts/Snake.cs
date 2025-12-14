using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    // Initial Snake direcion
    private Vector2 _direction = Vector2.up;

    // Snake List
    private List<Transform> _segments;
    // SnakeBody Types
    public Transform straightPrefab;
    public Transform turnUpRightPrefab;
    public Transform turnUpLeftPrefab;
    public Transform turnDownRightPrefab;
    public Transform turnDownLeftPrefab;
    public Transform tailPrefab;
    public int initailSize = 3;

    // 8 TurnTypes
    private enum TurnType { UpLeft, UpRight, DownLeft, DownRight, LeftUp, LeftDown, RightUp, RightDown }
    // Record turnPosition
    private Dictionary<Vector3, TurnType> _turnPoints;

    private void Start()
    {
        _segments = new List<Transform>();
        _segments.Add(this.transform);
        _turnPoints = new Dictionary<Vector3, TurnType>();

        // Initail size
        for(int i=1 ; i<this.initailSize ; i++)
        {
            _segments.Add(Instantiate(this.straightPrefab));
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        // Record current direction
        Vector2 prevDirection = _direction;

        // Update direction if Player press a movement key(W, A, S, D)
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
        {
            _direction = Vector2.up;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
        {
            _direction = Vector2.down;
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
        {
            _direction = Vector2.left;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
        {
            _direction = Vector2.right;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }

        // If the Snake makes a turn
        if (_direction != prevDirection)
        {
            Vector3 turnPosition = GetRoundedPosition(this.transform.position);
            TurnType newTurn = GetTurnType(prevDirection, _direction);

            // Check if this turnPosistion is already in the Dictionary
            if (!_turnPoints.ContainsKey(turnPosition))
            {
                _turnPoints.Add(turnPosition, newTurn);
            }
        }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void FixedUpdate()
    {
        // Record SnakeTail position to clear turnPoints
        Vector3 tailPosition = GetRoundedPosition(_segments[_segments.Count - 1].position);

        // Making Snake's head, body and tail connected
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        // SnakeHead movement
        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0.0f
        );

        UpdateBodyPrefabs();

        // Clear the turnPoint if it is at tailPosition
        if (_turnPoints.ContainsKey(tailPosition))
        {
            _turnPoints.Remove(tailPosition);
        }
    }

    private void UpdateBodyPrefabs()
    {
        // Updating from the first SnakeBody
        for (int i = 1; i < _segments.Count ; i++)
        {
            Transform segment = _segments[i];
            Vector3 currentPos = GetRoundedPosition(segment.position);

            // At turnPoint but not at tail
            if (_turnPoints.ContainsKey(currentPos) && currentPos != _segments[_segments.Count - 1].position)
            {
                // Using the currentPos to get the TurnType from the Dictionary
                TurnType turn = _turnPoints[currentPos];
                // Using the TurnType to get the correct turnPrefab
                Transform turnPrefab = GetTurnPrefab(turn);

                // Check if the name of the current segment is turn...Prefab
                if (segment.name != turnPrefab.name + "(Clone)") // p.s. Instantiate Prefab will add "(Clone)" behind its name
                {
                    ReplaceSegment(i, turnPrefab, currentPos);
                }
            }
            // At straight or at tail
            else
            {   
                // Check if the currentPos is tailPos
                if (currentPos == _segments[_segments.Count - 1].position)
                {
                    ReplaceSegment(i, tailPrefab, currentPos);
                    segment = _segments[i];
                }
                // Check if the name of the current segment is straightPrefab
                else if (segment.name != straightPrefab.name + "(Clone)")
                {
                    ReplaceSegment(i, straightPrefab, currentPos);
                    segment = _segments[i];
                }
                
                // Direction is determined by comparing the currentPos with the prevPos
                Vector3 prevPos = GetRoundedPosition(_segments[i - 1].position);

                // Going right side
                if (currentPos.x - prevPos.x < 0)
                {
                    segment.rotation = Quaternion.Euler(0, 0, -90);
                }
                // Going left side
                else if (currentPos.x - prevPos.x > 0)
                {
                    segment.rotation = Quaternion.Euler(0, 0, 90);
                }
                // Going upward
                else if (currentPos.y - prevPos.y < 0)
                {
                    segment.rotation = Quaternion.Euler(0, 0, 0);
                }
                // Going downward
                else if (currentPos.y - prevPos.y > 0)
                {
                    segment.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
        }
    }

    // Round the turnPosistion(x, y, z)
    private Vector3 GetRoundedPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x),
            Mathf.Round(position.y),
            position.z
        );
    }

    // Decide the TurnType
    private TurnType GetTurnType(Vector2 prevDirection, Vector2 nextDirection)
    {
        if (prevDirection == Vector2.up)
        {
            if (nextDirection == Vector2.left) return TurnType.UpLeft;
            if (nextDirection == Vector2.right) return TurnType.UpRight;
        }
        else if (prevDirection == Vector2.down)
        {
            if (nextDirection == Vector2.left) return TurnType.DownLeft;
            if (nextDirection == Vector2.right) return TurnType.DownRight;
        }
        else if (prevDirection == Vector2.left)
        {
            if (nextDirection == Vector2.up) return TurnType.LeftUp;
            if (nextDirection == Vector2.down) return TurnType.LeftDown;
        }
        else if (prevDirection == Vector2.right)
        {
            if (nextDirection == Vector2.up) return TurnType.RightUp;
            if (nextDirection == Vector2.down) return TurnType.RightDown;
        }
        // Default return
        return TurnType.UpRight;
    }

    // Get the corresponding turnPrefab based on TurnType
    private Transform GetTurnPrefab(TurnType turn)
    {
        if (turn == TurnType.UpRight || turn == TurnType.LeftDown)
        {
            return turnUpRightPrefab;
        }
        else if (turn == TurnType.UpLeft || turn == TurnType.RightDown)
        {
            return turnUpLeftPrefab;
        }
        else if (turn == TurnType.DownRight || turn == TurnType.LeftUp)
        {
            return turnDownRightPrefab;
        }
        else if (turn == TurnType.DownLeft || turn == TurnType.RightUp)
        {
            return turnDownLeftPrefab;
        }
        else
        {
            return turnUpRightPrefab;
        }
    }


    // Replace old Prefab with new one, and update _segments list
    private void ReplaceSegment(int index, Transform newPrefab, Vector3 position)
    {
        // Destroy old Prefab
        Destroy(_segments[index].gameObject);

        // Instantiate new Prefab
        Transform segment = Instantiate(newPrefab);
        segment.position = position;
        segment.rotation = Quaternion.identity; // Quaternion.identity == rotation(0,0,0)
        
        // Update the _segments list
        _segments[index] = segment;
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.straightPrefab);
        segment.position = _segments[_segments.Count - 1].position;
        segment.rotation = _segments[_segments.Count - 1].rotation;

        _segments.Add(segment);

        // Add a POINT when player eats an apple
        ScoreManager.instance.AddPoint();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
        }
        else if(other.tag == "Wall")
        {
            SceneManager.LoadSceneAsync("GameOverMenu");
        }
    }
}
