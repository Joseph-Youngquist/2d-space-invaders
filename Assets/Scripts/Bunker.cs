using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunker : MonoBehaviour
{
    [SerializeField]
    private int _destroyRadius = 4;

    private SpriteRenderer _spriteRenderer;
    private Texture2D _bunkerTexture;

    [SerializeField]
    private bool[] _bunkerHoles;

    private int _bunkerMaxHealth;
    private int _bunkerCurrentHealth;

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

        // initialize the _bunkerHoles array and set health
        _bunkerHoles = new bool[(int)_bunkerTexture.height];
        _bunkerMaxHealth = _bunkerTexture.width * _bunkerTexture.height - 1240; // subtracting the already blank areas of the bunker.
        _bunkerCurrentHealth = _bunkerMaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool projectileHit = other.gameObject.tag == "PlayerLaser" || other.gameObject.tag == "EnemyProjectile";

        /*
         * When we get a hit we need to see if the pixels above or below the bullet are transparent or not
         * in order to know where we need to set the y position for the start of the loop.
         * 
         * For the enemy lasers we need to go down the length of the bunker
         *  loop through the hit's x +- destroyRadius to the length of the destroy length
         *  if the pixels to the bottom of the bunker are all transparent then we need to
         *   ignore the collider since this bunker is destoryed through this column
         *  if there's a non-transparent pixel
         *   then we need to set that pixel's y as the y to start from and not the y of the hit
         *   and jump out of the loop
         *    
         */

        if (other.gameObject.tag == "PlayerLaser")
        {
            RegisterPlayerDamage(other);
        } else if (other.gameObject.tag == "EnemyProjectile")
        {
            RegisterEnemyDamage(other);
        }

        if (projectileHit)
        {
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
        Debug.Log($"WorldToPixel: {localPos}");
        return new Vector2((localPos.x + 0.5f) * _bunkerTexture.width, (localPos.y + 0.5f) * _bunkerTexture.height);
        
    }

    private (Vector2, bool) CheckBunkerIntegrity(Vector2 hit, string hitBy)
    {
        Debug.Log($"CheckBunkerIntegrity hit at ({hit})");
        /*
         *  We need to loop across the width of the laser and keep going
         *   down (for enemies) or up (for he player) until we reach:
         *     1) a non-transparent pixel
         *   or
         *     2) reached the edge of the bunker
         *     
         */
        // Loop across the bunker from hit.x - _destroyRadius until hit.x + _destroyRadius

        // Here's a "map" on what we're trying to do for enemy lasers:
        /*
         *        |         here we have the laser hitting at 2,0
         *      *****       we're going to loop through the column at index 2
         *      *****       until we find an active bunker piece (non-transparent pixel)
         *      *****       or we reach the bottom of the bunker
         *      *****       - In this case to the left, we return 2,0 and true
         *
         *
         *        |         here we have the laser hitting at 2,0
         *      **.**       we're going to loop through the column at index 2
         *      **.**       until we find an active bunker piece (non-transparent pixel)
         *      **.**       or we reach the bottom of the bunker
         *      *****       - In this case to the left, we return 2,2 and true
         *      
         *        |         here we have the laser hitting at 2,0
         *      **.**       we're going to loop through the column at index 2
         *      **.**       until we find an active bunker piece (non-transparent pixel)
         *      **.**       or we reach the bottom of the bunker
         *      **.**       - In this case to the left, we return 2,0 and false 
         *                  which will let the laser continue down until it hits the player, city or top
         */

        // Here's a "map" on what we're trying to do for player lasers:
        /*
         *                  here we have the laser hitting at 2,3
         *      *****       we're going to loop through the column at index 2
         *      *****       until we find an active bunker piece (non-transparent pixel)
         *      *****       or we reach the top of the bunker
         *      *****       - In this case to the left, we return 2,3 and true
         *        |
         *
         *
         *                  here we have the laser hitting at 2,3
         *      *****       we're going to loop through the column at index 2
         *      **.**       until we find an active bunker piece (non-transparent pixel)
         *      **.**       or we reach the top of the bunker
         *      **.**       - In this case to the left, we return 2,1 and true
         *        |
         *      
         *                  here we have the laser hitting at 2,3
         *      **.**       we're going to loop through the column at index 2
         *      **.**       until we find an active bunker piece (non-transparent pixel)
         *      *****       or we reach the top of the bunker
         *      **.**       - In this case to the left, we return 2,2 and true 
         *        |         
         */

        // short circut if we can by checking to see if we've already checked this hit.x before
        /*
        int checkIndex = (int)Mathf.Clamp(hit.x, (float)_bunkerTexture.width, (float)_bunkerTexture.height);
        bool hasHole = (bool)_bunkerHoles.GetValue(checkIndex);
        if (hasHole)
        {
            return (hit, false); // there's a hole in this bunker at this x position.
        }
        */
        
        for (int i = (int) hit.x - _destroyRadius; i <= (int) hit.x + _destroyRadius; i++)
        {
            if (hitBy == "Enemy")
            {
                for (int j = (int)hit.y; j <= _bunkerTexture.height; j++)
                {
                    if (i >= 0 && i < _bunkerTexture.width && j >= 0 && j < _bunkerTexture.height)
                    {
                        Color pixelColor = _bunkerTexture.GetPixel(i, j);

                        // if we have an active pixel then return this point at which we found the pixel
                        // and set the bool to true, the bunker is blocking
                        if (pixelColor.a == 1)
                        {
                            return (new Vector2((float)i, (float)j), true);
                        }
                    }
                }
            } else
            {
                for (int j = 0; j <= _bunkerTexture.height; j++)
                {
                    if (i >= 0 && i < _bunkerTexture.width && j >= 0 && j < _bunkerTexture.height)
                    {
                        Color pixelColor = _bunkerTexture.GetPixel(i, j);
                        // if we have an active pixel then return this point at which we found the pixel
                        // and set the bool to true, the bunker is blocking
                        if (pixelColor.a == 1f)
                        {
                            Debug.Log($"Found an active pixel so breaking at pos: {i},{j}");
                            return (new Vector2(i, j), true);
                        }
                    }
                }
            }
        }
        // If we've gotten to here, we didn't find an active pixel to block the laser
        // memorize this column so next time we can short circut the looping
        //_bunkerHoles[(int)hit.x] = true; // there's a hole at this x position

        return (hit, false); // this bunker didn't collide at this x
    }

    private bool MostlyDestroyed()
    {
        // Count the number of non-transparent pixels and destroy the bunker if there's less than 10% left
        /*
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
        */
        return _bunkerCurrentHealth < _bunkerMaxHealth * 0.1;

    }

    private void RegisterPlayerDamage(Collider2D other)
    {
        bool blocked;
        Vector3 hitPoint = other.gameObject.transform.position;
        Debug.Log($"Hit registered at world coords: {hitPoint}");
        // Convert world position to pixel position
        Vector2 pixelPos = WorldToPixel(hitPoint);
        
        // Check to see if the bunker is still able to block this hit
        // we'll get a new pixelPos and whether or not the bullet is blockable
        (pixelPos, blocked) = CheckBunkerIntegrity(pixelPos, "Player");
        Debug.Log($"Got {pixelPos} and {blocked} from integrity check.");
        if (blocked == false)
        {
            // skip this since the bunker is destroyed through this bullet column
            return;
        }

        PlayerLaser _playerLaser = other.gameObject.GetComponent<PlayerLaser>();

        if (_playerLaser is null)
        {
            Debug.Log("PlayerLaser is gone already in bunker collider");
            return;
        }
        _playerLaser.OnBunkerHit();
        int xi = (int)pixelPos.x - _destroyRadius;
        int yj = (int)pixelPos.y; // - _destroyRadius * 2;

        Debug.Log($"Laser hit at: {pixelPos.x},{pixelPos.y}");
        Debug.Log($"Settings i to {xi} and j to {yj}");
        // Loop over a square of pixels centered on the bullet hit and set their color to transparent
        for (int i = (int)pixelPos.x - _destroyRadius; i <= (int)pixelPos.x + _destroyRadius; i++)
        {
            for (int j = yj; j <= (int)(pixelPos.y + _destroyRadius * 2); j++)
            {
                Debug.Log($"destroying hit pixels at: ({i},{j})");
                if (i >= 0 && i < _bunkerTexture.width && j >= 0 && j < _bunkerTexture.height)
                {
                    _bunkerTexture.SetPixel(i, j, Color.clear);
                    _bunkerCurrentHealth--; // subtract off these impacts.
                }
            }
        }

        // Apply the changes made with SetPixel
        _bunkerTexture.Apply();
    }

    private void RegisterEnemyDamage(Collider2D other)
    {
        bool blocked;
        Vector3 hitPoint = other.gameObject.transform.position;
        
        // Convert world position to pixel position
        Vector2 pixelPos = WorldToPixel(hitPoint);

        // Check to see if the bunker is still able to block this hit
        // we'll get a new pixelPos and whether or not the bullet is blockable
        (pixelPos, blocked) = CheckBunkerIntegrity(pixelPos, "Player");
        if (blocked == false)
        {
            // skip this since the bunker is destroyed through this bullet column
            return;
        }
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
    }
}

