using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private Sprite _explosionSprite;

    [SerializeField]
    private Sprite _shipSprite;

    [SerializeField]
    private float _startX = -6.5f;
    [SerializeField]
    private float _startY = -4.5f;

    [SerializeField]
    private float _speed = 8f;

    [SerializeField]
    private int _maxLasers = 1;

    [SerializeField]
    private int _activeLaserCount = 0;

    private int _score = 0;
    private int _lives = 3;

    [SerializeField]
    private float _rightEdge = 7.25f;

    // Start is called before the first frame update
    void Start()
    {
        if (_laserPrefab is null)
        {
            Debug.LogError("Player.Start() - Laser Prefab is null");
        }

        if (_explosionSprite is null)
        {
            Debug.LogError("Player.Start() - Player explosion sprite is null");
        }

        if (_shipSprite is null)
        {
            Debug.LogError("Player.Start() - Player ship sprite is null");
        }

        transform.position = new Vector3(_startX, _startY, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _activeLaserCount < _maxLasers)
        {
            _activeLaserCount++;
            Fire();
        }

        CalculateMovement();
    }

    private void CalculateMovement()
    {
        float inputDirection = Input.GetAxis("Horizontal");
        Vector3 newDirection = new Vector3(inputDirection, 0f, 0f);

        transform.Translate(newDirection * _speed * Time.deltaTime);

        float currentX = transform.position.x;
        float newPositionX = Mathf.Clamp(currentX, -_rightEdge, _rightEdge);
        transform.position = new Vector3(newPositionX, _startY, 0f);
    }

    private void Fire()
    {
        Instantiate(_laserPrefab, transform.position, Quaternion.identity);
    }

    public void OnLaserDestroyed()
    {
        _activeLaserCount--;

        if (_activeLaserCount < 0)
        {
            _activeLaserCount = 0;
        }
    }

}
