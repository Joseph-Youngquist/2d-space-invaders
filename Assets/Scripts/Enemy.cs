using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /*
     * 
     *   Row 1 Invader--5 points
     *   Row 2 Invader--10 points
     *   Row 3 Invader--15 points
     *   Row 4 Invader--20 points
     *   Row 5 Invader--25 points
     *   Row 6 Invader--30 points
     *   Command Alien Ship--200 points, or 100 points in certain game variations
     *   Other player's laser cannon hit--200 points awarded to other player on certain competitive game variations
     *   https://spaceinvaders.fandom.com/wiki/Space_Invaders_(Atari_2600)
     *   
     *   The UFO (aka Flying Saucer or Mystery Ship) is a ship that will randomly pop up. 
     *   If a player hits it, they will receive a random number of either 50, 100, 150, 200, or 300 points. 
     *   It is present in almost every Space Invaders game, official or otherwise.
     *   
     */
    private int _pointValue = 0;
    [SerializeField] public Sprite sprite1;
    [SerializeField] public Sprite sprite2;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetPointValue(int pointValue)
    {
        _pointValue = pointValue;
    }

    public int GetPointValue()
    {
        return _pointValue;
    }
}
