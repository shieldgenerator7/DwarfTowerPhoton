using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct StatusLayer
{
    [SerializeField]
    private List<StatusEffect> statusList;

    public StatusLayer(params StatusEffect[] statusList)
    {
        this.statusList = statusList.ToHashSet().ToList();
    }
    public StatusLayer(List<StatusEffect> statusList)
    {
        this.statusList = statusList;
    }
    public StatusLayer(HashSet<StatusEffect> statusList)
    {
        this.statusList = statusList.ToList();
    }

    public bool Has(StatusEffect effect)
    {
        return statusList.Contains(effect);
    }

    public void Set(StatusEffect effect, bool setOn = true)
    {
        if (setOn)
        {
            if (!statusList.Contains(effect))
            {
                statusList.Add(effect);
            }
        }
        else
        {
            statusList.Remove(effect);
        }
    }

    public List<StatusEffect> StatusEffects => statusList.ToList();

    public void checkValid()
    {
        if (statusList == null)
        {
            throw new ArgumentException($"statusList cannot be null! statusList: {statusList}");
        }
    }

    public StatusLayer stackOr(StatusLayer status)
    {
        HashSet<StatusEffect> effects = this.statusList.ToHashSet();
        effects.UnionWith(status.statusList);
        StatusLayer layer = new StatusLayer(effects);
        return layer;
    }

    public StatusLayer stackAnd(StatusLayer status)
    {
        HashSet<StatusEffect> effects = new HashSet<StatusEffect>(this.statusList);
        effects.IntersectWith(status.statusList);
        StatusLayer layer = new StatusLayer(effects);
        return layer;
    }

    public static bool operator ==(StatusLayer a, StatusLayer b)
    {
        return a.statusList.Equals(b.statusList);
    }
    public static bool operator !=(StatusLayer a, StatusLayer b)
    {
        return !a.statusList.Equals(b.statusList);
    }
    public override bool Equals(object obj)
    {
        return obj != null
            && obj is StatusLayer
            && this.statusList.Equals(((StatusLayer)obj).statusList);
    }
    public override int GetHashCode()
    {
        return this.statusList.GetHashCode();
    }

    #region Photon methods
    //2021-12-19: copied from StatLayer
    //2021-12-17: copied from https://doc.photonengine.com/en-us/pun/current/reference/serialization-in-photon
    public static void RegisterWithPhoton()
    {
        PhotonPeer.RegisterType(typeof(StatusLayer), (byte)'T', SerializeStatusLayer, DeserializeStatusLayer);
    }


    //TODO: Maybe refactor the Photon network serialization to only serialize the effects that are active
    public bool[] BoolList
    {
        get => new bool[] {
                statusList.Contains(StatusEffect.STUNNED),
                statusList.Contains(StatusEffect.STEALTHED),
                statusList.Contains(StatusEffect.ROOTED),
            };
        set
        {
            statusList.Clear();
            if (value[0]) { statusList.Add(StatusEffect.STUNNED); }
            if (value[1]) { statusList.Add(StatusEffect.STEALTHED); }
            if (value[2]) { statusList.Add(StatusEffect.ROOTED); }
        }
    }

    public const int byteArraySize = 1;
    public static readonly byte[] memStatusLayer = new byte[byteArraySize];
    private static short SerializeStatusLayer(StreamBuffer outStream, object customobject)
    {
        StatusLayer sl = (StatusLayer)customobject;
        lock (memStatusLayer)
        {
            byte[] bytes = memStatusLayer;
            bytes[0] = ConvertBoolArrayToByte(sl.BoolList);
            outStream.Write(bytes, 0, byteArraySize);
        }
        return byteArraySize;
    }

    private static object DeserializeStatusLayer(StreamBuffer inStream, short length)
    {
        StatusLayer sl = new StatusLayer();
        lock (memStatusLayer)
        {
            inStream.Read(memStatusLayer, 0, length);// byteArraySize);
            sl.BoolList = ConvertByteToBoolArray(memStatusLayer[0]);
        }
        return sl;
    }

    //2021-12-19: copied from https://stackoverflow.com/a/24322811/2336212
    private static byte ConvertBoolArrayToByte(bool[] source)
    {
        byte result = 0;
        // This assumes the array never contains more than 8 elements!
        int index = 8 - source.Length;

        // Loop through the array
        foreach (bool b in source)
        {
            // if the element is 'true' set the bit at that position
            if (b)
            {
                result |= (byte)(1 << (7 - index));
            }
            index++;
        }

        return result;
    }
    //2021-12-19: copied from https://stackoverflow.com/a/24322811/2336212
    private static bool[] ConvertByteToBoolArray(byte b)
    {
        // prepare the return result
        bool[] result = new bool[8];

        // check each bit in the byte. if 1 set to true, if 0 set to false
        for (int i = 0; i < 8; i++)
        {
            result[i] = (b & (1 << i)) == 0 ? false : true;
        }

        // reverse the array
        Array.Reverse(result);

        return result;
    }
    #endregion
}
