using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatLayer
{
    public float moveSpeed;
    public float maxHits;
    public float fireRate;
    public float damage;
    public float size;

    public StatLayer(float defaultValue = -1)
    {
        //-1 is an invalid value
        //if a stat has an invalid value, it doesn't get processed
        moveSpeed = defaultValue;
        maxHits = defaultValue;
        fireRate = defaultValue;
        damage = defaultValue;
        size = defaultValue;
    }
}
