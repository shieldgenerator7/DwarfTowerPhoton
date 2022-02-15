using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuleProcessor : MonoBehaviour
{
    public List<RuleSet> ruleSets;

    public RuleContext initialContext;

    private HashSet<RuleSet> activeRuleSets = new HashSet<RuleSet>();

    public void Init(Vector2 dir, Vector2 pos)
    {
        initialContext = new RuleContext()
        {
            targetDir = dir.normalized,
            targetPos = pos,
        };
        InitializeContext();
    }

    #region Initialization
    private void InitializeContext()
    {
        //Initialize context
        initialContext.deltaTime = 1;
        initialContext.self = gameObject;
    }

    private void RegisterDelegates()
    {
        PlayerInput playerInput = gameObject.FindComponent<PlayerInput>();
        if (playerInput)
        {
            playerInput.onInputChanged -= OnInputChanged;
            playerInput.onInputChanged += OnInputChanged;
        }
        AminaPool aminaPool = gameObject.FindComponent<AminaPool>();
        if (aminaPool)
        {
            aminaPool.onAminaEmpty += OnAminaEmpty;
            aminaPool.onAminaFull += OnAminaFull;
        }
    }
    #endregion

    #region Rule Triggers
    // Start is called before the first frame update
    void Start()
    {
        InitializeContext();
        RegisterDelegates();
        ruleSets.FindAll(rs => rs.activeAtStart)
            .ForEach(rs => activeRuleSets.Add(rs));
        //Process rules
        ProcessRules(RuleTrigger.OnStart, initialContext);
    }

    // Update is called once per frame
    void Update()
    {
        RuleContext context = new RuleContext(initialContext)
        {
            deltaTime = Time.deltaTime,
        };
        ProcessRules(RuleTrigger.OnUpdate, context);
    }

    private void OnInputChanged(InputState input)
    {
        if (input.ability1 == ButtonState.DOWN)
        {
            RuleContext context = new RuleContext(initialContext)
            {
            };
            ProcessRules(RuleTrigger.OnButtonDownLMB, context);
        }
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

    private void OnAminaEmpty(float amina)
    {
        RuleContext context = new RuleContext(initialContext)
        {
        };
        ProcessRules(RuleTrigger.OnAminaEmpty, context);
    }
    private void OnAminaFull(float amina)
    {
        RuleContext context = new RuleContext(initialContext)
        {
        };
        ProcessRules(RuleTrigger.OnAminaFull, context);
    }
    #endregion

    #region Rule Processing
    private void ProcessRules(RuleTrigger trigger, RuleContext context)
    {
        ruleSets
            .FindAll(ruleSet => activeRuleSets.Contains(ruleSet))
            .ForEach(ruleSet =>
        {
            RuleContext currentContext = new RuleContext(context)
            {
                currentRuleSet = ruleSet,
            };
            ruleSet.rules
                .FindAll(rule => rule.trigger == trigger)
                .ForEach(rule => ProcessRule(rule, rule.settings, currentContext));
        });
    }

    private void ProcessRule(Rule rule, RuleSettings settings, RuleContext context)
    {
        bool canProcess = rule.condition?.Check(rule.settings, context) ?? true;
        if (canProcess)
        {
            rule.actions.ForEach(action => TakeAction(action, settings, context));
        }
    }

    private void TakeAction(RuleAction action, RuleSettings settings, RuleContext context)
    {
        Rigidbody2D rb2d = gameObject.FindComponent<Rigidbody2D>();
        StatKeeper statKeeper = gameObject.FindComponent<StatKeeper>();
        StatLayer stats = statKeeper.selfStats.Stats;
        switch (action)
        {
            case RuleAction.MOVE_IN_TARGET_DIR:
                rb2d.velocity = context.targetDir * stats.moveSpeed;
                break;
            case RuleAction.MOVE_TOWARDS_TARGET_POS:
                rb2d.velocity = (context.targetPos - (Vector2)gameObject.transform.position).normalized * stats.moveSpeed;
                break;
            case RuleAction.DAMAGE:
                {
                    //TODO: make delegate: onDamageDealt
                    HealthPool hp = context.target.FindComponent<HealthPool>();
                    hp.Health += -stats.damage * context.deltaTime;
                }
                break;
            case RuleAction.DAMAGE_SELF:
                {
                    //TODO: make damage over time amount a variable
                    HealthPool hp = gameObject.FindComponent<HealthPool>();
                    hp.Health += -1 * context.deltaTime;
                }
                break;
            case RuleAction.CREATE_OBJECT:
                Vector2 spawnCenter = gameObject.FindComponent<PlayerController>().SpawnCenter;
                Vector2 targetPos = Utility.MouseWorldPos;
                Vector2 targetDir = (targetPos - spawnCenter).normalized;
                RuleProcessor newObj = gameObject.FindComponent<ObjectSpawner>()
                    .spawnObject<RuleProcessor>(
                    settings.Get(RuleSetting.Option.SPAWN_INDEX),
                    spawnCenter,
                    targetDir
                    );
                newObj.Init(targetDir, targetPos);
                break;
            case RuleAction.SWITCH_RULESET:
                activeRuleSets.Remove(context.currentRuleSet);
                activeRuleSets.Add(settings.targetRuleSet);
                break;
            default:
                throw new System.ArgumentException($"Unknown action: {action}");
        }
    }
    #endregion
}
