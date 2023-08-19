using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunker : MonoBehaviour
{
    [SerializeField]
    private int _destroyRadius = 3;

    private SpriteRenderer _spriteRenderer;
    private Texture2D _bunkerTexture;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _bunkerTexture = Instantiate(_spriteRenderer.sprite.texture);

        if(_spriteRenderer is null)
        {
            Debug.LogError("Bunker.Start() - SpriteRenderer is null");
        }

        if (_bunkerTexture is null)
        {
            Debug.LogError("Bunker.Start() - Bunker Texture is null");
        }

        _spriteRenderer.sprite = Sprite.Create(_bunkerTexture, _spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), _spriteRenderer.sprite.pixelsPerUnit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerLaser")
        {
            Vector3 hitPoint = other.gameObject.transform.position;
            PlayerLaser _playerLaser = other.gameObject.GetComponent<PlayerLaser>();

            if (_playerLaser is null)
            {
                Debug.Log("PlayerLaser is gone already in bunker collider");
                return;
            }
            _playerLaser.OnBunkerHit();

            // Convert world position to pixel position
            Vector2 pixelPos = WorldToPixel(hitPoint);

            // Loop over a square of pixels centered on the bullet hit and set their color to transparent
            for (int i = (int)pixelPos.x - _destroyRadius; i <= (int)pixelPos.x + _destroyRadius; i++)
            {
                for (int j = (int)pixelPos.y - _destroyRadius; j <= (int)pixelPos.y + _destroyRadius; j++)
                {
                    if (i >= 0 && i < _bunkerTexture.width && j >= 0 && j < _bunkerTexture.height)
                    {
                        _bunkerTexture.SetPixel(i, j, Color.clear);
                    }
                }
            }

            // Apply the changes made with SetPixel
            _bunkerTexture.Apply();

            // Destroy the bunker if it's mostly destroyed
            if (MostlyDestroyed())
            {
                Destroy(gameObject);
            }
        }
    }

    private Vector2 WorldToPixel(Vector3 worldPos)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);

        return new Vector2((localPos.x + 0.5f) * _bunkerTexture.width, (localPos.y + 0.5f) * _bunkerTexture.height);
    }

    private bool MostlyDestroyed()
    {
        // Count the number of non-transparent pixels and destroy the bunker if there's less than 10% left
        int pixelCount = 0;
        for (int i = 0; i < _bunkerTexture.width; i++)
        {
            for (int j = 0; j < _bunkerTexture.height; j++)
            {
                if (_bunkerTexture.GetPixel(i, j).a > 0)
                {
                    pixelCount++;
                }
            }
        }
        return pixelCount < _bunkerTexture.width * _bunkerTexture.height * 0.1;

    }
}
