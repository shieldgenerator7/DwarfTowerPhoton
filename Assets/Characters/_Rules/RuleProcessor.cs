using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ComponentContext))]
public class RuleProcessor : MonoBehaviour
{
    public List<RuleSet> ruleSets;

    public RuleContext ruleContext;
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
        ruleContext = new RuleContext()
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
        ruleContext.deltaTime = 1;
        ruleContext.componentContext = componentContext;
        ruleContext.statMultiplier = 1;
        ruleContext.ruleSetActions = new Dictionary<RuleSet, RuleContext.RuleSetAction>();
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
        if (componentContext.teamToken)
        {
            componentContext.teamToken.onControllerGainedControl += setController;
            setController(componentContext.teamToken.controller);
        }
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
        ProcessRules(RuleTrigger.OnStart);
    }

    // Update is called once per frame
    void Update()
    {
        ruleContext.deltaTime = Time.deltaTime;
        ProcessRules(RuleTrigger.OnUpdate);
    }

    private void OnInputChanged(InputState input)
    {
        ruleContext.inputState = input;
        ProcessRules(RuleTrigger.OnInputChanged);
    }

    private void OnControllerInputChanged(InputState input)
    {
        ruleContext.inputState = input;
        ProcessRules(RuleTrigger.OnControllerInputChanged);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ruleContext.target = collision.gameObject.FindComponent<ComponentContext>();
        ruleContext.isCollision = true;
        if (!ruleContext.target)
        {
            throw new UnityException(
                $"GameObject {collision.gameObject.name} does not have a ComponentContext!"
                );
        }
        ProcessRules(RuleTrigger.OnHit);
    }

    private void OnTriggerEnter2D(Collider2D coll2d)
    {
        ruleContext.target = coll2d.gameObject.FindComponent<ComponentContext>();
        ruleContext.isTrigger = true;
        if (!ruleContext.target)
        {
            throw new UnityException(
                $"GameObject {coll2d.gameObject.name} does not have a ComponentContext!"
                );
        }
        ProcessRules(RuleTrigger.OnHit);
    }

    private void OnAminaEmpty(float amina)
    {
        ProcessRules(RuleTrigger.OnAminaEmpty);
    }
    private void OnAminaFull(float amina)
    {
        ProcessRules(RuleTrigger.OnAminaFull);
    }
    #endregion

    #region Rule Processing
    private void ProcessRules(RuleTrigger trigger)
    {
        activeRuleSets.ToList()
            .ForEach(ruleSet =>
        {
            ruleContext.currentRuleSet = ruleSet;
            ruleSet.rules
                .FindAll(rule => rule.trigger == trigger)
                .ForEach(rule => ProcessRule(rule));
        });
    }

    private void ProcessRule(Rule rule)
    {
        bool canProcess = rule.condition?.Check(rule.settings, ruleContext) ?? true;
        if (canProcess)
        {
            rule.actions.ForEach(
                action => action.TakeAction(rule.settings, ref ruleContext)
                );
        }
        UpdateRuleSets();
    }

    private void UpdateRuleSets()
    {
        foreach (var action in ruleContext.ruleSetActions)
        {
            RuleSet ruleSet = action.Key;
            RuleContext.RuleSetAction ruleSetAction = action.Value;
            switch (ruleSetAction)
            {
                case RuleContext.RuleSetAction.ACTIVATE:
                    ValidateRuleSet(ruleSet);
                    activeRuleSets.Add(ruleSet);
                    if (ruleContext.lastDeactivatedRuleSet == ruleSet)
                    {
                        ruleContext.lastDeactivatedRuleSet = null;
                    }
                    break;
                case RuleContext.RuleSetAction.DEACTIVATE:
                    ValidateRuleSet(ruleSet);
                    activeRuleSets.Remove(ruleSet);
                    ruleContext.lastDeactivatedRuleSet = ruleSet;
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
        ruleContext.ruleSetActions.Clear();
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
