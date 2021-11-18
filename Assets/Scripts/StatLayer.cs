using System;
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

    public StatLayer Multiply(float multiplier)
    {
        if (multiplier < 0)
        {
            throw new ArgumentException("multiplier is less than 0! " + multiplier);
        }
        StatLayer layer = new StatLayer();
        if (this.moveSpeed >= 0)
        {
            layer.moveSpeed = this.moveSpeed * multiplier;
        }
        if (this.maxHits >= 0)
        {
            layer.maxHits = this.maxHits * multiplier;
        }
        if (this.fireRate >= 0)
        {
            layer.fireRate = this.fireRate * multiplier;
        }
        if (this.damage >= 0)
        {
            layer.damage = this.damage * multiplier;
        }
        if (this.size >= 0)
        {
            layer.size = this.size * multiplier;
        }
        return layer;
    }

    public StatLayer Multiply(StatLayer multiplier)
    {
        StatLayer layer = new StatLayer();
        if (this.moveSpeed >= 0 && multiplier.moveSpeed >= 0)
        {
            layer.moveSpeed = this.moveSpeed * multiplier.moveSpeed;
        }
        if (this.maxHits >= 0 && multiplier.maxHits >= 0)
        {
            layer.maxHits = this.maxHits * multiplier.maxHits;
        }
        if (this.fireRate >= 0 && multiplier.fireRate >= 0)
        {
            layer.fireRate = this.fireRate * multiplier.fireRate;
        }
        if (this.damage >= 0 && multiplier.damage >= 0)
        {
            layer.damage = this.damage * multiplier.damage;
        }
        if (this.size >= 0 && multiplier.size >= 0)
        {
            layer.size = this.size * multiplier.size;
        }
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
        if (minLayer.moveSpeed >= 0 && maxLayer.moveSpeed >= 0)
        {
            layer.moveSpeed = Mathf.Lerp(minLayer.moveSpeed, maxLayer.moveSpeed, percentage);
        }
        if (minLayer.maxHits >= 0 && maxLayer.maxHits >= 0)
        {
            layer.maxHits = Mathf.Lerp(minLayer.maxHits, maxLayer.maxHits, percentage);
        }
        if (minLayer.fireRate >= 0 && maxLayer.fireRate >= 0)
        {
            layer.fireRate = Mathf.Lerp(minLayer.fireRate, maxLayer.fireRate, percentage);
        }
        if (minLayer.damage >= 0 && maxLayer.damage >= 0)
        {
            layer.damage = Mathf.Lerp(minLayer.damage, maxLayer.damage, percentage);
        }
        if (minLayer.size >= 0 && maxLayer.size >= 0)
        {
            layer.size = Mathf.Lerp(minLayer.size, maxLayer.size, percentage);
        }
        return layer;
    }
}
