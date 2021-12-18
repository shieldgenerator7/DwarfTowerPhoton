using ExitGames.Client.Photon;
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

    public StatLayer Add(float addend)
    {
        if (addend == 0)
        {
            throw new ArgumentException($"addend is 0! {addend}");
        }
        StatLayer layer = new StatLayer();
        layer.moveSpeed = AddStat(this.moveSpeed, addend);
        layer.maxHits = AddStat(this.maxHits, addend);
        layer.fireRate = AddStat(this.fireRate, addend);
        layer.damage = AddStat(this.damage, addend);
        layer.size = AddStat(this.size, addend);
        return layer;
    }

    public StatLayer Add(StatLayer addend)
    {
        StatLayer layer = new StatLayer();
        layer.moveSpeed = AddStat(this.moveSpeed, addend.moveSpeed);
        layer.maxHits = AddStat(this.maxHits, addend.maxHits);
        layer.fireRate = AddStat(this.fireRate, addend.fireRate);
        layer.damage = AddStat(this.damage, addend.damage);
        layer.size = AddStat(this.size, addend.size);
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="minLayer"></param>
    /// <param name="maxLayer"></param>
    /// <param name="percentage">A value between 0 and 1. 0: minLayer, 0.5: in between, 1: maxLayer</param>
    /// <returns></returns>
    public static StatLayer LerpAdd(StatLayer minLayer, StatLayer maxLayer, float percentage)
    {
        StatLayer layer = new StatLayer();
        layer.moveSpeed = LerpStatAdd(minLayer.moveSpeed, maxLayer.moveSpeed, percentage);
        layer.maxHits = LerpStatAdd(minLayer.maxHits, maxLayer.maxHits, percentage);
        layer.fireRate = LerpStatAdd(minLayer.fireRate, maxLayer.fireRate, percentage);
        layer.damage = LerpStatAdd(minLayer.damage, maxLayer.damage, percentage);
        layer.size = LerpStatAdd(minLayer.size, maxLayer.size, percentage);
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
    private static float AddStat(float stat, float addend)
    {
        if (stat >= 0)
        {
            stat += addend;
            //addends can't subtract to below zero
            stat = Mathf.Max(0, stat);
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
    private static float LerpStatAdd(float min, float max, float percentage)
        => Mathf.Lerp(min, max, percentage);

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

    #region Photon methods
    //2021-12-17: copied from https://doc.photonengine.com/en-us/pun/current/reference/serialization-in-photon

    public static void RegisterWithPhoton()
    {
        PhotonPeer.RegisterType(typeof(StatLayer), (byte) 'S', SerializeStatLayer, DeserializeStatLayer);
    }

    public static readonly byte[] memStatLayer = new byte[5 * 4];
    private static short SerializeStatLayer(StreamBuffer outStream, object customobject)
    {
        StatLayer sl = (StatLayer)customobject;
        lock (memStatLayer)
        {
            byte[] bytes = memStatLayer;
            int index = 0;
            Protocol.Serialize(sl.moveSpeed, bytes, ref index);
            Protocol.Serialize(sl.maxHits, bytes, ref index);
            Protocol.Serialize(sl.fireRate, bytes, ref index);
            Protocol.Serialize(sl.damage, bytes, ref index);
            Protocol.Serialize(sl.size, bytes, ref index);
            outStream.Write(bytes, 0, 5 * 4);
        }
        return 5 * 4;
    }

    private static object DeserializeStatLayer(StreamBuffer inStream, short length)
    {
        StatLayer sl = new StatLayer();
        lock (memStatLayer)
        {
            inStream.Read(memStatLayer, 0, length);// 5 * 4);
            int index = 0;
            Protocol.Deserialize(out sl.moveSpeed, memStatLayer, ref index);
            Protocol.Deserialize(out sl.maxHits, memStatLayer, ref index);
            Protocol.Deserialize(out sl.fireRate, memStatLayer, ref index);
            Protocol.Deserialize(out sl.damage, memStatLayer, ref index);
            Protocol.Deserialize(out sl.size, memStatLayer, ref index);
        }
        return sl;
    }
    #endregion
}
