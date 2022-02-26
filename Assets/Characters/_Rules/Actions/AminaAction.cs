using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AminaAction", menuName = "Rule/Action/AminaAction", order = 0)]
public class AminaAction : TargetedAction
{
    public enum Action
    {
        [Tooltip("Use amina (setting: AMINA_COST).")]
        USE_AMINA,
        [Tooltip("Use amina over time (setting: AMINA_COST_PER_SECOND).")]
        USE_AMINA_PER_SECOND,
        [Tooltip("Recharge amina (setting: AMINA_COST).")]
        RECHARGE_AMINA,
        [Tooltip("Recharge amina over time (setting: AMINA_COST_PER_SECOND).")]
        RECHARGE_AMINA_PER_SECOND,
        RESERVE_AMINA_PER_SECOND,
        USE_RESERVED_AMINA,
        CANCEL_RESERVED_AMINA,
    }
    public Action action;

    protected override void TakeAction(ComponentContext target, RuleSettings settings, ref RuleContext context)
    {
        //Take the selected action
        switch (action)
        {
            case Action.USE_AMINA:
                bool acceptPartialAmount = settings
                    .Try(RuleSetting.Option.ACCEPT_PARTIAL_AMOUNT)
                    ?? true;
                float amina = target.aminaPool.requestAmina(
                    settings.Get(RuleSetting.Option.AMINA_COST),
                    acceptPartialAmount
                    );
                break;
            case Action.USE_AMINA_PER_SECOND:
                bool acceptPartialAmountPerSecond = settings
                    .Try(RuleSetting.Option.ACCEPT_PARTIAL_AMOUNT)
                    ?? true;
                float aminaPerSecond = target.aminaPool.requestAmina(
                    settings.Get(RuleSetting.Option.AMINA_COST_PER_SECOND) * context.deltaTime,
                    acceptPartialAmountPerSecond
                    );
                break;
            case Action.RECHARGE_AMINA:
                target.aminaPool.rechargeAmina(
                    settings.Get(RuleSetting.Option.AMINA_COST)
                    );
                break;
            case Action.RECHARGE_AMINA_PER_SECOND:
                target.aminaPool.rechargeAmina(
                    settings.Get(RuleSetting.Option.AMINA_COST_PER_SECOND) * context.deltaTime
                    );
                break;
            case Action.RESERVE_AMINA_PER_SECOND:
                target.aminaPool.reserveAmina(
                    settings.Get(RuleSetting.Option.AMINA_COST_PER_SECOND) * context.deltaTime
                    );
                break;
            case Action.USE_RESERVED_AMINA:
                float collectedAmina = target.aminaPool.collectReservedAmina();
                break;
            case Action.CANCEL_RESERVED_AMINA:
                target.aminaPool.cancelReservedAmina();
                break;
        }
    }
}
