using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "RuleAction", menuName = "Rule/RuleAction", order = 0)]
public abstract class RuleAction : ScriptableObject
{
    public abstract void TakeAction(RuleSettings settings, ref RuleContext context);
}
