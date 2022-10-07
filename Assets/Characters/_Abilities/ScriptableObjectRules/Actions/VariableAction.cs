using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "VariableAction", menuName = "Rule/Action/VariableAction", order = 0)]
public abstract class VariableAction : RuleAction
{
    //protected abstract System.Type type { get; }

    public sealed override void TakeAction(RuleSettings settings, ref RuleContext context)
    {
        object source = GetSource(settings, context);
        SetVariable(source, settings, ref context);
    }

    protected abstract object GetSource(RuleSettings settings, RuleContext context);

    protected abstract void SetVariable(object source, RuleSettings settings, ref RuleContext context);
}
