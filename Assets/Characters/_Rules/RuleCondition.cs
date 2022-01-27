using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuleCondition
{
    public virtual bool Check() => true;
}
