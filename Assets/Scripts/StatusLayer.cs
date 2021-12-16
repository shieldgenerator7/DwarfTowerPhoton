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
}
