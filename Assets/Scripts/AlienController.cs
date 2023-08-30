using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    [SerializeField] private GameController _gameController;
    [SerializeField] private float _baseSpeed = 30f;
    [SerializeField] private float _waitTime;
    [SerializeField] private GameObject[] _alienTypePrefabs;
    [SerializeField] private float _offsetX = 1f;
    [SerializeField] private float _offsetY = 1f;
    [SerializeField] private float _startRowX;
    [SerializeField] private float _startRowY;

    private int _columns = 11;
    private int _rows = 5;

    private GameObject[][] _grid;
    private int _enemiesAlive;
    private bool _toggle = true;
    private bool _moveLeft = false; // default is to move right

    private float _rightEdgeOfScreen = 7.25f;
    private int _directionChangeCount;
    private bool _changedDirection; // we default to not have had a change in directions (left and right)

    // Start is called before the first frame update
    void Start()
    {
        if (_gameController == null)
        {
            Debug.LogError("AlienController::Start() - GameController is null");
        }
    }

    public void InitializeEnemies(bool init = false)
    {
        Vector2 gridSize = new Vector2(_rows, _columns);
        _grid = new GameObject[(int)gridSize.x][];
        // instantiate a grid of 5 rows of 11 aliens
        for (int row = 0; row < _rows; row++)
        {
            _grid[row] = new GameObject[(int)gridSize.y];
            for (int col = 0; col < _columns; col++)
            {
                if (init)
                {
                    Vector3 newAlienPos = new Vector3(_startRowX + (col * _offsetY), _startRowY - (row * _offsetX), 0f);
                    _grid[row][col] = Instantiate(_alienTypePrefabs[row], newAlienPos, Quaternion.identity);
                    _grid[row][col].GetComponent<Enemy>().SetPointValue(row * 5 + 5);
                } else
                {
                    _grid[row][col].SetActive(true);
                }
            }
        }
    }
    public void Reset()
    {
        _enemiesAlive = _rows * _columns;
        _directionChangeCount = 0;
        _changedDirection = false;
        _moveLeft = false;
        _waitTime = 6f;
        StartCoroutine(MoveGrid());
    }
    // Update is called once per frame
    void MoveEnemies()
    {
        float incrementMovementX = 0.2f;
        float incrementMovevmentY = 0.0f;

        if (_moveLeft)
        {
            incrementMovementX = -incrementMovementX; // reverse direction.
        }

        // check to see if we need to advance the enemies down a row this time.
        if (_changedDirection)
        {
            incrementMovevmentY = -1f;
            _changedDirection = false;
        }

        for (int row = 0; row < _rows; ++row)
        {
            for (int col = 0; col < _columns; col++)
            {
                Vector3 pos = _grid[row][col].transform.position;
                _grid[row][col].transform.position = new Vector3(pos.x + incrementMovementX, pos.y + incrementMovevmentY, 0f); ;
                Enemy alien = _grid[row][col].transform.gameObject.GetComponent<Enemy>();
                Sprite sprite = alien.sprite1;
                if (_toggle == false)
                {
                    sprite = alien.sprite2;
                }
                _grid[row][col].transform.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;

                // if any enemy reaches the left or right edge of the screen
                // we need to toggle the direction they all move.
                float alienCurrentPositionX = alien.transform.position.x;
                bool isActive = alien.isActiveAndEnabled;
                if (alienCurrentPositionX >= _rightEdgeOfScreen && isActive && !_changedDirection)
                {
                    _moveLeft = true;
                    _changedDirection = true;
                    _directionChangeCount++;
                } else if (alienCurrentPositionX <= -_rightEdgeOfScreen && isActive && !_changedDirection)
                {
                    _moveLeft = false;
                    _changedDirection = true;
                    _directionChangeCount++;
                }
            }
        }
     
    }

    public void OnEnemyHit(int pointValue)
    {
        _enemiesAlive--;
        StopCoroutine(MoveGrid());
        _gameController.OnEnemyKilled(pointValue);
        StartCoroutine(MoveGrid());
        if (_enemiesAlive == 0)
        {
            // Stop the Enemy Movement
            StopCoroutine(MoveGrid());
            // Need to notify the GameController that this wave is all done!
            _gameController.WaveCleared();
        }
    }
    IEnumerator MoveGrid()
    {
        while (_enemiesAlive > 0)
        {
            _waitTime = Mathf.Clamp(Mathf.Sqrt(0.125f*_enemiesAlive) + 5f, 5f, 6f); // 55 enemies
           
            yield return new WaitForSeconds(_waitTime); // gets faster with less enemies.
            MoveEnemies();
            _toggle = !_toggle;
        }
    }
}
