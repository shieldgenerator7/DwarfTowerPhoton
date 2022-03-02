using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildVariableAction", menuName = "Rule/Action/BuildVariableAction", order = 0)]
public class BuildVariableAction : VariableAction
{
    public enum Variable
    {
        BUILD_POS,
        BUILD_TARGET,
        BUILD_ACTION,
    }
    public List<Variable> variables;

    public enum Source
    {
        BUILD_TARGET,
        TARGET_POS,
        CURSOR_POS,
    }
    public Source source;

    public List<EntityType> entityTypesToDetect;

    public float tooCloseRange = 1;
    public float upgradeRange = 2;
    public float destroyRange = 1;

    //TODO: improve the information used to identify objects to upgrade/destroy:
    //use an object ID? this would require all spawnable objects to have a unique object type ID,
    //much like each character has a unique character ID
    public List<string> upgradeNames;
    public List<string> destroyNames;

    protected override object GetSource(RuleSettings settings, RuleContext context)
    {
        Vector2 buildPos;
        switch (source)
        {
            case Source.BUILD_TARGET:
                buildPos = context.target.transform.position;
                break;
            case Source.TARGET_POS:
                buildPos = context.targetPos;
                break;
            case Source.CURSOR_POS:
                buildPos = Utility.MouseWorldPos;
                break;
            default: throw new System.ArgumentException($"Unknown source enum: {source}");
        }
        var sourceRet = getBuildActionState(buildPos);
        return sourceRet;
    }
    protected override void SetVariable(object source, RuleSettings settings, ref RuleContext context)
    {
        (Vector2, GameObject, BuildAction) sourceObj = ((Vector2, GameObject, BuildAction))source;
        if (variables.Contains(Variable.BUILD_POS))
        {
            context.targetPos = sourceObj.Item1;
        }
        if (variables.Contains(Variable.BUILD_TARGET))
        {
            context.target = sourceObj.Item2?.FindComponent<ComponentContext>() ?? null;
        }
        if (variables.Contains(Variable.BUILD_ACTION))
        {
            context.buildAction = sourceObj.Item3;
        }
    }

    private (Vector2, GameObject, BuildAction) getBuildActionState(Vector2 position)
    {
        GameObject targetObject = null;
        BuildAction buildAction;
        //Check to see if it should upgrade or destroy a nearby object
        GameObject goUpgrade = getClosestObject(
            position,
            upgradeRange,
            (go) => upgradeNames.Any(name => go.name.Contains(name))
            );
        GameObject goDestroy = getClosestObject(
            position,
            destroyRange,
            //TODO: require owned by same player in order to destroy
            (go) => destroyNames.Any(name => go.name.Contains(name))
            );
        if (goUpgrade || goDestroy)
        {
            if (goUpgrade && goDestroy)
            {
                //If goUpgrade is closer
                if (closestGameObject(position, goUpgrade, goDestroy) == goUpgrade)
                {
                    targetObject = goUpgrade;
                    buildAction = BuildAction.UPGRADE;
                }
                else
                {
                    targetObject = goDestroy;
                    buildAction = BuildAction.DESTROY;
                }
            }
            else if (goUpgrade)
            {
                targetObject = goUpgrade;
                buildAction = BuildAction.UPGRADE;
            }
            else if (goDestroy)
            {
                targetObject = goDestroy;
                buildAction = BuildAction.DESTROY;
            }
            else
            {
                throw new UnityException("This branch shouldn't be possible!");
            }
        }
        //Check for objects in the way
        else
        {
            GameObject goObstacle = getClosestObject(
                position,
                tooCloseRange,
                (go) => true
                );
            if (goObstacle)
            {
                targetObject = goObstacle;
                buildAction = BuildAction.NONE;
            }
            //Free to build new object
            else
            {
                targetObject = null;
                buildAction = BuildAction.CREATE;
            }
        }

        return (targetObject?.transform.position ?? position, targetObject, buildAction);
    }

    private GameObject getClosestObject(Vector2 position, float range, Predicate<GameObject> searchFunc)
    {
        RaycastHit2D[] rch2ds = Physics2D.CircleCastAll(
                position,
                range,
                Vector2.zero
                );
        List<RaycastHit2D> rch2dList = rch2ds.ToList();
        rch2dList.RemoveAll(rch2d => !rch2d || rch2d.collider.isTrigger);
        List<GameObject> goList = rch2dList
            .ConvertAll(rch2d => rch2d.collider.gameObject);
        goList.RemoveAll(go =>
        {
            HealthPool hp = go.FindComponent<HealthPool>();
            return hp && !entityTypesToDetect.Contains(hp.entityType);
        });
        goList.RemoveAll(go => !searchFunc(go));
        if (goList.Count > 0)
        {
            return goList.Aggregate(
                (closest, current) => closestGameObject(position, closest, current)
                );
        }
        return null;
    }

    private GameObject closestGameObject(Vector2 position, GameObject go1, GameObject go2)
    {
        if (Vector2.Distance(position, go1.transform.position)
            < Vector2.Distance(position, go2.transform.position))
        {
            return go1;
        }
        else
        {
            return go2;
        }
    }
}
