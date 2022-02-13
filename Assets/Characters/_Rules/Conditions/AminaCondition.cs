using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AminaCondition", menuName = "Characters/Rule/AminaCondition", order = 0)]
public class AminaCondition : RuleCondition
{
    public float aminaRequirement;
    public bool acceptPartialAmount = true;

    public override bool Check(RuleContext context)
    {
        AminaPool aminaPool = context.self.FindComponent<AminaPool>();
        float amina = aminaPool.requestAmina(
            aminaRequirement * context.deltaTime,
            acceptPartialAmount
            );
        return amina > 0;
        //TODO: make AminaAction
        //return aminaPool.hasAmina(
        //    aminaRequirement * context.deltaTime,
        //    acceptPartialAmount
        //    );
    }
}
