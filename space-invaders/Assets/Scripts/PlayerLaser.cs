using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private float _speed = 15f;
    [SerializeField] private Sprite _laserExplosion;
    [SerializeField] private float _deathPauseLength = 1.25f;
    [SerializeField] private float _screenTop = 4f;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player is null)
        {
            Debug.LogError("PlayerLaser.Start() - Player is null");
        }
        if (_laserExplosion is null)
        {
            Debug.LogError("PlayerLaser.Start() - Laser explosion sprite is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > _screenTop)
        {
            ScreenTopExplosion();
        }
    }

    public void OnBunkerHit()
    {
        _player.OnLaserDestroyed();
        Destroy(this.gameObject);
    }
    private void ScreenTopExplosion()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        _speed = 0f;
        _player.OnLaserDestroyed();
        //transform.GetComponent<SpriteRenderer>().sprite = 
        Destroy(this.gameObject, _deathPauseLength);
        
    }
}
