

using UnityEngine;

public struct InputState
{
    public Vector2 movement;//movement
    public ButtonState ability1;//LMB
    public ButtonState ability2;//RMB
    public ButtonState ability3;//space
    public ButtonState reload;//reload
    public ButtonState moveTowardsCursor;//move towards cursor

    public ButtonState Button(AbilitySlot slot)
    {
        switch (slot)
        {
            case AbilitySlot.Ability1: return ability1;
            case AbilitySlot.Ability2: return ability2;
            case AbilitySlot.Ability3: return ability3;
            case AbilitySlot.Reload: return reload;
        }
        throw new System.ArgumentException($"AbilitySlot not recognized! slot: {slot}");
    }

    public override bool Equals(object obj)
    {
        if (!(obj is InputState))
        {
            return false;
        }
        return this == ((InputState)obj);
    }

    public override int GetHashCode()
    {
        //2021-12-03: copied from https://stackoverflow.com/a/1646913/2336212
        int hash = 17;
        hash = hash * 31 + movement.x.GetHashCode();
        hash = hash * 31 + movement.y.GetHashCode();
        hash = hash * 31 + ability1.GetHashCode();
        hash = hash * 31 + ability2.GetHashCode();
        hash = hash * 31 + ability3.GetHashCode();
        hash = hash * 31 + reload.GetHashCode();
        hash = hash * 31 + moveTowardsCursor.GetHashCode();
        return hash;
    }

    public static bool operator ==(InputState a, InputState b)
    {
        return a.movement == b.movement
            && a.ability1 == b.ability1
            && a.ability2 == b.ability2
            && a.ability3 == b.ability3
            && a.reload == b.reload
            && a.moveTowardsCursor == b.moveTowardsCursor
            ;
    }
    public static bool operator !=(InputState a, InputState b)
    {
        return a.movement != b.movement
            || a.ability1 != b.ability1
            || a.ability2 != b.ability2
            || a.ability3 != b.ability3
            || a.reload != b.reload
            || a.moveTowardsCursor != b.moveTowardsCursor
            ;
    }
}
