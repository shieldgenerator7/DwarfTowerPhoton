using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementAction", menuName = "Rule/Action/MovementAction", order = 0)]
public class MovementAction : TargetedAction
{
    public enum Action
    {
        [Tooltip("Move this entity in the target direction.")]
        MOVE_IN_TARGET_DIR,
        [Tooltip("Move this entity towards the target position.")]
        MOVE_TOWARDS_TARGET_POS,
    }
    public Action action;

    protected override void TakeAction(ComponentContext target, RuleSettings settings, ref RuleContext context)
    {
        Rigidbody2D rb2d = target.rb2d;
        StatLayer stats = target.statKeeper?.selfStats.Stats ?? new StatLayer(-1);
        float moveSpeed = stats.moveSpeed;
        switch (action)
        {
            case Action.MOVE_IN_TARGET_DIR:
                rb2d.velocity = context.targetDir * moveSpeed;
                break;
            case Action.MOVE_TOWARDS_TARGET_POS:
                rb2d.velocity = (context.targetPos - (Vector2)rb2d.transform.position).normalized * moveSpeed;
                break;
            default:
                throw new System.ArgumentException($"Action enum not implemented: {action}");
        }
    }
}
