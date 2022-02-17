using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ComponentContext))]
public class RuleProcessor : MonoBehaviour
{
    public List<RuleSet> ruleSets;

    public RuleContext initialContext;
    public ComponentContext componentContext;

    private HashSet<RuleSet> activeRuleSets = new HashSet<RuleSet>();
    private RuleSet lastDeactivatedRuleSet;

    private void Awake()
    {
        InitializeContext();
    }

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
        //Component context
        componentContext = gameObject.FindComponent<ComponentContext>();
        //Initialize context
        initialContext.deltaTime = 1;
        initialContext.componentContext = componentContext;
    }

    private bool _registeredDelegates = false;
    private void RegisterDelegates()
    {
        //Only allow being called once
        if (_registeredDelegates)
        {
            throw new UnityException($"RuleProcessor.registerDelegates called more than once! gameObject: {gameObject.name}");
        }
        _registeredDelegates = true;
        //Register delegates
        PlayerInput playerInput = componentContext.playerInput;
        if (playerInput)
        {
            playerInput.onInputChanged += OnInputChanged;
        }
        HealthPool healthPool = componentContext.healthPool;
        if (healthPool)
        {
            //healthPool.onChanged += (hp) => ProcessRules(RuleTrigger.onHealthChanged);
            healthPool.onHealed += (hp) => ProcessRules(RuleTrigger.OnHealed);
            healthPool.onHealedFull += (hp) => ProcessRules(RuleTrigger.OnHealthFull);
            healthPool.onDamaged += (hp) => ProcessRules(RuleTrigger.OnDamaged);
            healthPool.onDied += (hp) => ProcessRules(RuleTrigger.OnDied);
        }
        AminaPool aminaPool = componentContext.aminaPool;
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
        initialContext.inputState = input;
        RuleContext context = new RuleContext(initialContext)
        {
        };
        ProcessRules(RuleTrigger.OnInputChanged, context);
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
    private void ProcessRules(RuleTrigger trigger)
    {
        ProcessRules(trigger, initialContext);
    }
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
        ComponentContext compContext = context.componentContext;
        StatLayer stats = compContext.statKeeper?.selfStats.Stats ?? new StatLayer(-1);
        switch (action)
        {
            case RuleAction.MOVE_IN_TARGET_DIR:
                compContext.rb2d.velocity = context.targetDir * stats.moveSpeed;
                break;
            case RuleAction.MOVE_TOWARDS_TARGET_POS:
                compContext.rb2d.velocity = (context.targetPos - (Vector2)compContext.transform.position).normalized * stats.moveSpeed;
                break;
            case RuleAction.DAMAGE:
                {
                    //TODO: make delegate: onDamageDealt
                    HealthPool hp = context.target.FindComponent<HealthPool>();
                    hp.Health += -stats.damage * context.deltaTime;
                }
                break;
            case RuleAction.DAMAGE_SELF:
                //TODO: make damage over time amount a variable
                compContext.healthPool.Health += -1 * context.deltaTime;
                break;
            case RuleAction.CREATE_OBJECT:
                Vector2 spawnCenter = compContext.playerController.SpawnCenter;
                Vector2 targetPos = Utility.MouseWorldPos;
                Vector2 targetDir = (targetPos - spawnCenter).normalized;
                RuleProcessor newObj = compContext.objectSpawner
                    .spawnObject<RuleProcessor>(
                        settings.Get(RuleSetting.Option.SPAWN_INDEX),
                        spawnCenter,
                        targetDir
                    );
                newObj.Init(targetDir, targetPos);
                break;
            case RuleAction.SWITCH_RULESET:
                RuleSet currentRuleSet = context.currentRuleSet;
                RuleSet targetRuleSet = settings.targetRuleSet
                    ?? lastDeactivatedRuleSet;
                ValidateRuleSet(currentRuleSet);
                ValidateRuleSet(targetRuleSet);
                activeRuleSets.Remove(currentRuleSet);
                activeRuleSets.Add(targetRuleSet);
                lastDeactivatedRuleSet = currentRuleSet;
                break;
            case RuleAction.USE_AMINA:
                bool acceptPartialAmount = settings
                    .Try(RuleSetting.Option.ACCEPT_PARTIAL_AMOUNT)
                    ?? true;
                float amina = compContext.aminaPool.requestAmina(
                    settings.Get(RuleSetting.Option.AMINA_COST),
                    acceptPartialAmount
                    );
                break;
            case RuleAction.USE_AMINA_PER_SECOND:
                bool acceptPartialAmountPerSecond = settings
                    .Try(RuleSetting.Option.ACCEPT_PARTIAL_AMOUNT)
                    ?? true;
                float aminaPerSecond = compContext.aminaPool.requestAmina(
                    settings.Get(RuleSetting.Option.AMINA_COST_PER_SECOND) * context.deltaTime,
                    acceptPartialAmountPerSecond
                    );
                break;
            case RuleAction.RECHARGE_AMINA:
                compContext.aminaPool.rechargeAmina(
                    settings.Get(RuleSetting.Option.AMINA_COST)
                    );
                break;
            case RuleAction.RECHARGE_AMINA_PER_SECOND:
                compContext.aminaPool.rechargeAmina(
                    settings.Get(RuleSetting.Option.AMINA_COST_PER_SECOND) * context.deltaTime
                    );
                break;
            default:
                throw new System.ArgumentException($"Unknown action: {action}");
        }
    }
    #endregion

    #region Validation
    private void ValidateRuleSet(RuleSet ruleSet)
    {
        //Check to make sure the ruleset is not null
        if (!ruleSet)
        {
            Debug.LogError($"RuleSet null: {ruleSet}");
        }
        //Check to make sure the ruleset is registered
        if (!ruleSets.Contains(ruleSet))
        {
            Debug.LogError($"RuleSet not registered: {ruleSet}");
        }
    }
    #endregion
}
