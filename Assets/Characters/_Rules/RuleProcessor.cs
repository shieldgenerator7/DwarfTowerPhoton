using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuleProcessor : MonoBehaviour
{
    public List<RuleSet> ruleSets;

    private RuleContext initialContext;

    public void Init(Vector2 dir, Vector2 pos)
    {
        initialContext = new RuleContext()
        {
            targetDir = dir,
            targetPos = pos,
        };
    }

    #region Rule Triggers
    // Start is called before the first frame update
    void Start()
    {
        ProcessRules(RuleTrigger.OnStart, initialContext);
    }

    // Update is called once per frame
    void Update()
    {
        ProcessRules(RuleTrigger.OnUpdate, initialContext);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RuleContext context = new RuleContext(initialContext)
        {
            target = collision.gameObject,
            isCollision = true,
        };
        ProcessRules(RuleTrigger.OnHit, context);
    }

    private void OnTriggerEnter2D(Collider2D coll2d)
    {
        RuleContext context = new RuleContext(initialContext)
        {
            target = coll2d.gameObject,
            isTrigger = true,
        };
        ProcessRules(RuleTrigger.OnHit, context);
    }
    #endregion

    #region Rule Processing
    private void ProcessRules(RuleTrigger trigger, RuleContext context)
    {
        ruleSets.ForEach(ruleSet =>
        {
            ruleSet.rules
                .FindAll(rule => rule.trigger == trigger)
                .ForEach(rule => ProcessRule(rule, context));
        });
    }

    private void ProcessRule(Rule rule, RuleContext context)
    {
        bool canProcess = rule.conditions.Count == 0
            || rule.conditions.Any(cond => CheckCondition(cond, context));
        if (canProcess)
        {
            rule.actions.ForEach(action => TakeAction(action, context));
        }
    }

    private bool CheckCondition(RuleCondition condition, RuleContext context)
    {
        bool onSameTeam = TeamToken.onSameTeam(gameObject, context.target);
        if (condition.onAlly && onSameTeam || condition.onEnemy && !onSameTeam)
        {
            if (condition.onTrigger && context.isTrigger || condition.onCollision && context.isCollision)
            {
                HealthPool hp = context.target.FindComponent<HealthPool>();
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

    private void TakeAction(RuleAction action, RuleContext context)
    {
        Rigidbody2D rb2d = gameObject.FindComponent<Rigidbody2D>();
        StatKeeper statKeeper = gameObject.FindComponent<StatKeeper>();
        StatLayer stats = statKeeper.selfStats.Stats;
        switch (action)
        {
            case RuleAction.MOVE_IN_TARGET_DIR:
                rb2d.velocity = context.targetDir.normalized * stats.moveSpeed;
                break;
            case RuleAction.MOVE_TOWARDS_TARGET_POS:
                rb2d.velocity = (context.targetPos - (Vector2)gameObject.transform.position).normalized * stats.moveSpeed;
                break;
            case RuleAction.DAMAGE:
                {
                    //TODO: make delegate: onDamageDealt
                    HealthPool hp = context.target.FindComponent<HealthPool>();
                    hp.Health += -stats.damage;
                }
                break;
            case RuleAction.DAMAGE_SELF:
                {
                    //TODO: make damage over time amount a variable
                    //TODO: put deltaTime in RuleContext
                    HealthPool hp = gameObject.FindComponent<HealthPool>();
                    hp.Health += -1 * Time.deltaTime;
                }
                break;
            default:
                throw new System.ArgumentException($"Unknown action: {action}");
        }
    }
    #endregion
}
