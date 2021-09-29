using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //file that contains the player data
    public int health;
    public int level;
    public float[] position; //float[] is basically vector3, since vector3 is 3 float values but vector 3 cannot be formatted into binary
    MobileHealthController2D mobileHealthController2D;

    public PlayerData (MobileHealthController2D mobileHealthController2D)
    {
        level = mobileHealthController2D.level;
        health = mobileHealthController2D.health;

        position = new float[3];
        position[0] = mobileHealthController2D.transform.position.x;
        position[0] = mobileHealthController2D.transform.position.y;
        position[0] = mobileHealthController2D.transform.position.z;

    }
}
