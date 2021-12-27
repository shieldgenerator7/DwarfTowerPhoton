using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatusLayer
{
    public bool stunned;
    public bool stealthed;
    public bool rooted;

    public StatusLayer stackOr(StatusLayer status)
    {
        StatusLayer layer = new StatusLayer();
        layer.stunned = this.stunned || status.stunned;
        layer.stealthed = this.stealthed || status.stealthed;
        layer.rooted = this.rooted || status.rooted;
        return layer;
    }

    public StatusLayer stackAnd(StatusLayer status)
    {
        StatusLayer layer = new StatusLayer();
        layer.stunned = this.stunned && status.stunned;
        layer.stealthed = this.stealthed && status.stealthed;
        layer.rooted = this.rooted && status.rooted;
        return layer;
    }

    #region Photon methods
    //2021-12-19: copied from StatLayer
    //2021-12-17: copied from https://doc.photonengine.com/en-us/pun/current/reference/serialization-in-photon
    public static void RegisterWithPhoton()
    {
        PhotonPeer.RegisterType(typeof(StatusLayer), (byte)'T', SerializeStatusLayer, DeserializeStatusLayer);
    }


    public bool[] BoolList
    {
        get=> new bool[] {
                stunned,
                stealthed,
                rooted,
            };
        set
        {
            stunned = value[0];
            stealthed = value[1];
            rooted = value[2];
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
