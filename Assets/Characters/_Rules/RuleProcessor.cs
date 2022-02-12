using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleProcessor : MonoBehaviour
{
    public List<Rule> rules;

    #region Rule Triggers
    // Start is called before the first frame update
    void Start()
    {
        rules.FindAll(rule => rule.trigger == RuleTrigger.OnStart);
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Rule Processing
    private void ProcessRule(Rule rule)
    {

    }

    private bool CheckCondition(RuleCondition condition, GameObject target, bool isTrigger = false, bool isCollision = false)
    {
        bool onSameTeam = TeamToken.onSameTeam(gameObject, target);
        if (condition.onAlly && onSameTeam || condition.onEnemy && !onSameTeam)
        {
            if (condition.onTrigger && isTrigger || condition.onCollision && isCollision)
            {
                HealthPool hp = target.FindComponent<HealthPool>();
                if (hp)
                {
                    if (condition.entityTypes.Contains(hp.entityType))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    #endregion
}
