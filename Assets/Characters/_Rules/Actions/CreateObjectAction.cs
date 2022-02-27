using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObjectAction : RuleAction
{
    public enum Action
    {
        [Tooltip("Create object (using the SPAWN_INDEX setting and ObjectSpawner component).")]
        CREATE_OBJECT,
    }
    public Action action;

    public override void TakeAction(RuleSettings settings, ref RuleContext context)
    {
        ComponentContext compContext = context.componentContext;
        switch (action)
        {
            case Action.CREATE_OBJECT:
                Vector2 spawnCenter = compContext.playerController?.SpawnCenter ?? compContext.transform.position;
                Vector2 targetPos = Utility.MouseWorldPos;
                Vector2 targetDir = (targetPos - spawnCenter).normalized;
                ComponentContext newObj = compContext.objectSpawner
                    .spawnObject<ComponentContext>(
                        settings.Get(RuleSetting.Option.SPAWN_INDEX),
                        spawnCenter,
                        targetDir
                    );
                newObj.ruleProcessor.Init(targetDir, targetPos);
                context.lastCreatedObject = newObj;
                break;
            default:
                throw new System.ArgumentException($"Action enum not implemented: {action}");
        }
    }
}
