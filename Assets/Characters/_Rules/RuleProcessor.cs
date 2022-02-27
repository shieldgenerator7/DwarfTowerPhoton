using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ComponentContext))]
public class RuleProcessor : MonoBehaviour
{
    public List<RuleSet> ruleSets;

    public RuleContext initialContext;
    private ComponentContext componentContext;

    private HashSet<RuleSet> activeRuleSets = new HashSet<RuleSet>();

    private RuleProcessor controller;

    private void Awake()
    {
        InitializeContext();
        RegisterDelegates();
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
        componentContext.InitializeComponents();
        //Initialize context
        initialContext.deltaTime = 1;
        initialContext.componentContext = componentContext;
        initialContext.statMultiplier = 1;
        initialContext.ruleSetActions = new Dictionary<RuleSet, RuleContext.RuleSetAction>();
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
        StatKeeper statKeeper = componentContext.statKeeper;
        if (statKeeper)
        {
            statKeeper.selfStats.onStatChanged += (stat) => ProcessRules(RuleTrigger.OnStatsChanged);
        }
        StatusKeeper statusKeeper = componentContext.statusKeeper;
        if (statusKeeper)
        {
            statusKeeper.onStatusChanged += (status) => ProcessRules(RuleTrigger.OnStatusChanged);
        }
        //Controller
        componentContext.teamToken.onControllerGainedControl += setController;
        setController(componentContext.teamToken.controller);
    }

    private void setController(TeamToken ttController)
    {
        //Defaults
        ttController = ttController ?? componentContext.teamToken;
        //Unregister prev delegates
        if (controller)
        {
            if (controller.componentContext.playerInput)
            {
                controller.componentContext.playerInput.onInputChanged -= OnControllerInputChanged;
            }
        }
        controller = ttController.gameObject.FindComponent<RuleProcessor>();
        //Register new delegates
        if (controller)
        {
            if (controller.componentContext.playerInput)
            {
                controller.componentContext.playerInput.onInputChanged += OnControllerInputChanged;
            }
        }
    }
    #endregion

    #region Rule Triggers
    // Start is called before the first frame update
    void Start()
    {
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

    private void OnControllerInputChanged(InputState input)
    {
        RuleContext context = new RuleContext(initialContext)
        {
            inputState = input,
        };
        ProcessRules(RuleTrigger.OnControllerInputChanged, context);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RuleContext context = new RuleContext(initialContext)
        {
            target = collision.gameObject.FindComponent<ComponentContext>(),
            isCollision = true,
        };
        if (!context.target)
        {
            throw new UnityException(
                $"GameObject {collision.gameObject.name} does not have a ComponentContext!"
                );
        }
        ProcessRules(RuleTrigger.OnHit, context);
    }

    private void OnTriggerEnter2D(Collider2D coll2d)
    {
        RuleContext context = new RuleContext(initialContext)
        {
            target = coll2d.gameObject.FindComponent<ComponentContext>(),
            isTrigger = true,
        };
        if (!context.target)
        {
            throw new UnityException(
                $"GameObject {coll2d.gameObject.name} does not have a ComponentContext!"
                );
        }
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
        activeRuleSets.ToList()
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
            rule.actionEnums.ForEach(action => TakeAction(action, settings, ref context));
            rule.actions.ForEach(action => action.TakeAction(settings, ref context));
        }
        UpdateRuleSets(ref context);
    }

    private void TakeAction(RuleActionEnum action, RuleSettings settings, ref RuleContext context)
    {
        ComponentContext compContext = context.componentContext;
        switch (action)
        {
            default:
                throw new System.ArgumentException($"Unknown action: {action}");
        }
    }

    private void UpdateRuleSets(ref RuleContext context)
    {
        foreach (var action in context.ruleSetActions)
        {
            RuleSet ruleSet = action.Key;
            RuleContext.RuleSetAction ruleSetAction = action.Value;
            switch (ruleSetAction)
            {
                case RuleContext.RuleSetAction.ACTIVATE:
                    ValidateRuleSet(ruleSet);
                    activeRuleSets.Add(ruleSet);
                    if (context.lastDeactivatedRuleSet == ruleSet)
                    {
                        context.lastDeactivatedRuleSet = null;
                    }
                    break;
                case RuleContext.RuleSetAction.DEACTIVATE:
                    ValidateRuleSet(ruleSet);
                    activeRuleSets.Remove(ruleSet);
                    context.lastDeactivatedRuleSet = ruleSet;
                    break;
                case RuleContext.RuleSetAction.ADD:
                    activeRuleSets.Add(ruleSet);
                    break;
                case RuleContext.RuleSetAction.REMOVE:
                    activeRuleSets.Remove(ruleSet);
                    break;
                default:
                    throw new System.ArgumentException($"RuleSetAction not known: {action.Value}");
            }
        }
        context.ruleSetActions.Clear();
        initialContext.lastDeactivatedRuleSet = context.lastDeactivatedRuleSet;
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
