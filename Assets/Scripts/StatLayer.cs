using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatLayer
{
    public float speedChargeMultiplier;
    public float initialDamageChargeMultiplier;
    public float damagePerSecondChargeMultiplier;
    public float stunDurationChargeMultiplier;
    public float knockbackDistanceChargeMultiplier;
    public float maxHealthChargeMultiplier;

    public StatLayer(float defaultValue = -1)
    {
        //-1 is an invalid value
        speedChargeMultiplier = defaultValue;
        initialDamageChargeMultiplier = defaultValue;
        damagePerSecondChargeMultiplier = defaultValue;
        stunDurationChargeMultiplier = defaultValue;
        knockbackDistanceChargeMultiplier = defaultValue;
        maxHealthChargeMultiplier = defaultValue;
    }
}
