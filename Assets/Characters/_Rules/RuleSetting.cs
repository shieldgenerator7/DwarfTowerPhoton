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
        //Activation delay
        ACTIVATE_DELAY,
    }
    public Option setting;
    public float value;
}
