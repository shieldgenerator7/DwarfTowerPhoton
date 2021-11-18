using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatLayer
{
    public float moveSpeed;
    public float maxHits;
    public float shotFireRate;
    public float shotMoveSpeed;
    public float shotDamage;
    public float shotMaxHits;

    public StatLayer(float defaultValue = -1)
    {
        //-1 is an invalid value
        moveSpeed = defaultValue;
        maxHits = defaultValue;
        shotFireRate = defaultValue;
        shotMoveSpeed = defaultValue;
        shotDamage = defaultValue;
        shotMaxHits = defaultValue;
    }
}
