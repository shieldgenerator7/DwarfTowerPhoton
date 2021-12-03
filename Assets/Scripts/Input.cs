

using UnityEngine;

public struct Input
{
    public Vector2 movement;
    public InputState ability0;//reload
    public InputState ability1;//LMB
    public InputState ability2;//RMB
    public InputState ability3;//space

    public override bool Equals(object obj)
    {
        if (!(obj is Input))
        {
            return false;
        }
        return this == ((Input)obj);
    }

    public override int GetHashCode()
    {
        //2021-12-03: copied from https://stackoverflow.com/a/1646913/2336212
        int hash = 17;
        hash = hash * 31 + movement.x.GetHashCode();
        hash = hash * 31 + movement.y.GetHashCode();
        hash = hash * 31 + ability0.GetHashCode();
        hash = hash * 31 + ability1.GetHashCode();
        hash = hash * 31 + ability2.GetHashCode();
        hash = hash * 31 + ability3.GetHashCode();
        return hash;
    }

    public static bool operator ==(Input a, Input b)
    {
        return a.movement == b.movement
            && a.ability0 == b.ability0
            && a.ability1 == b.ability1
            && a.ability2 == b.ability2
            && a.ability3 == b.ability3;
    }
    public static bool operator !=(Input a, Input b)
    {
        return a.movement != b.movement
            || a.ability0 != b.ability0
            || a.ability1 != b.ability1
            || a.ability2 != b.ability2
            || a.ability3 != b.ability3;
    }
}
