using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RuleSetting
{
    public enum Option
    {
        //Amina
        AMINA_COST,
        ACCEPT_PARTIAL_AMOUNT,
        //Activation delay
        ACTIVATE_DELAY,
    }
    public Option setting;
    public float value;

    public static implicit operator bool(RuleSetting setting)
    {
        if (setting.value == 1)
        {
            return true;
        }
        else if (setting.value == 0)
        {
            return false;
        }
        else
        {
            throw new System.ArgumentException(
                $"Cannot convert value to bool: {setting.value}"
                );
        }
    }

    public static implicit operator int(RuleSetting setting)
        => (int)setting.value;

    public static implicit operator float(RuleSetting setting)
        => (float)setting.value;
}