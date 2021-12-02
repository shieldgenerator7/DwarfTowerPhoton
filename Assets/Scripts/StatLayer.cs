using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatLayer
{
    public const float STAT_IGNORE = -1;

    public float moveSpeed;//how fast it travels (units/sec)
    public float maxHits;//how many hits before being stunned or destroyed
    public float fireRate;//how many seconds between shots
    public float damage;//damage dealt upon initial contact
    public float size;//percentage of base size

    public StatLayer(float defaultValue = STAT_IGNORE)
    {
        //-1 is an invalid value
        //if a stat has an invalid value, it doesn't get processed
        moveSpeed = defaultValue;
        maxHits = defaultValue;
        fireRate = defaultValue;
        damage = defaultValue;
        size = defaultValue;
    }

    public StatLayer Multiply(float multiplier)
    {
        if (multiplier < 0)
        {
            throw new ArgumentException($"multiplier is less than 0! {multiplier}");
        }
        StatLayer layer = new StatLayer();
        layer.moveSpeed = MultiplyStat(this.moveSpeed, multiplier);
        layer.maxHits = MultiplyStat(this.maxHits, multiplier);
        layer.fireRate = MultiplyStat(this.fireRate, multiplier);
        layer.damage = MultiplyStat(this.damage, multiplier);
        layer.size = MultiplyStat(this.size, multiplier);
        return layer;
    }

    public StatLayer Multiply(StatLayer multiplier)
    {
        StatLayer layer = new StatLayer();
        layer.moveSpeed = MultiplyStat(this.moveSpeed, multiplier.moveSpeed);
        layer.maxHits = MultiplyStat(this.maxHits, multiplier.maxHits);
        layer.fireRate = MultiplyStat(this.fireRate, multiplier.fireRate);
        layer.damage = MultiplyStat(this.damage, multiplier.damage);
        layer.size = MultiplyStat(this.size, multiplier.size);
        return layer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="minLayer"></param>
    /// <param name="maxLayer"></param>
    /// <param name="percentage">A value between 0 and 1. 0: minLayer, 0.5: in between, 1: maxLayer</param>
    /// <returns></returns>
    public static StatLayer Lerp(StatLayer minLayer, StatLayer maxLayer, float percentage)
    {
        StatLayer layer = new StatLayer();
        layer.moveSpeed = LerpStat(minLayer.moveSpeed, maxLayer.moveSpeed, percentage);
        layer.maxHits = LerpStat(minLayer.maxHits, maxLayer.maxHits, percentage);
        layer.fireRate = LerpStat(minLayer.fireRate, maxLayer.fireRate, percentage);
        layer.damage = LerpStat(minLayer.damage, maxLayer.damage, percentage);
        layer.size = LerpStat(minLayer.size, maxLayer.size, percentage);
        return layer;
    }

    //TODO: Potentially remove this method and refactor the thing that uses it
    public StatLayer Charge(float multiplier, StatLayer percentage)
    {
        StatLayer layer = new StatLayer();
        layer.moveSpeed = ChargeStat(this.moveSpeed, multiplier, percentage.moveSpeed);
        layer.maxHits = ChargeStat(this.maxHits, multiplier, percentage.maxHits);
        layer.fireRate = ChargeStat(this.fireRate, multiplier, percentage.fireRate);
        layer.damage = ChargeStat(this.damage, multiplier, percentage.damage);
        layer.size = ChargeStat(this.size, multiplier, percentage.size);
        return layer;
    }

    #region Individual Stat Methods
    private static float MultiplyStat(float stat, float multiplier)
    {
        if (stat >= 0 && multiplier >= 0)
        {
            stat *= multiplier;
        }
        return stat;
    }
    private static float LerpStat(float min, float max, float percentage)
    {
        if (min >= 0 && max >= 0)
        {
            return Mathf.Lerp(min, max, percentage);
        }
        else if (min >= 0)
        {
            return min;
        }
        else if (max >= 0)
        {
            return max;
        }
        else
        {
            return STAT_IGNORE;
        }
    }

    /// <summary>
    /// Returns the charged value of the given stat with the given multiplier and given percentage
    /// Percentage is the percentage of extra amount kept
    /// Examples:
    /// 2, 1, 1 -> 2
    /// 2, 2, 1 -> 4
    /// 2, 0.5, 1 -> 1
    /// 2, 1, 2 -> 2
    /// 2, 2, 2 -> 6
    /// 2, 0.5, 2 -> 0
    /// 2, 1, 0.5 -> 2
    /// 2, 2, 0.5 -> 3
    /// 2, 0.5, 0.5 -> 1.5
    /// </summary>
    /// <param name="multiplier"></param>
    /// <param name="stat"></param>
    /// <param name="percentage">Percentage of extra amount kept</param>
    private static float ChargeStat(float stat, float multiplier, float percentage)
    {
        if (stat == 0 && percentage > 0)
        {
            stat = 1;
        }
        float newStat = stat * multiplier;
        float diff = (newStat - stat);
        float keptDiff = diff * percentage;
        return Mathf.Max(0, stat + keptDiff);
    }
    #endregion
}
