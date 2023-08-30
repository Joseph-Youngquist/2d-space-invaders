using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int _wavesCleared;

    [SerializeField] private AlienController _alienController;
    [SerializeField] private Player _player;
    [SerializeField] private TMP_Text _playerScore;

    // Start is called before the first frame update
    void Start()
    {
        if( _alienController == null )
        {
            Debug.LogError("GameController.Start() - AlienController is null");
        }
        _playerScore = GameObject.Find("PlayerScore").GetComponent<TMP_Text>();
        if(_playerScore == null)
        {
            Debug.LogError("GameController.Start() - Player Score TextMeshPro is null");
        }
        if (_player == null)
        {
            Debug.LogError("GameController.Start() - Player is null");
        }
        _playerScore.text = $"Score: {_player.GetScore()}";
        _wavesCleared = 0;
        StartWave();
    }
    private void StartWave()
    {

        // Start with a fresh enemy setup.
        _alienController.InitializeEnemies(true);
        _alienController.Reset();
    }
    public void WaveCleared()
    {
        _wavesCleared++;
        StartWave();
    }
    public void OnEnemyKilled(int killValue)
    {
        _player.AddScore(killValue);
        _playerScore.text = $"Score: {_player.GetScore()}";
    }
}
