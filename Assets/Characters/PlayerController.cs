﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    public Color playerColor { get; set; } = Color.white;
    public AminaReloader aminaReloader;//ability called by default when the player runs out of amina
    [SerializeField]
    private AbilityContext abilityContext;
    public AbilityContext AbilityContext
    {
        get { return abilityContext; }
        set
        {
            abilityContext = value;
            onAbilityContextChanged?.Invoke(abilityContext);
        }
    }
    public delegate void OnAbilityContextChanged(AbilityContext context);
    public event OnAbilityContextChanged onAbilityContextChanged;

    private List<PlayerAbility> processingAbilities = new List<PlayerAbility>();//used for abilities that have lasting effects

    public InputState inputState { get; private set; }

    private PhotonView photonView;
    public PhotonView PV
    {
        get
        {
            if (photonView == null)
            {
                photonView = GetComponentInParent<PhotonView>();
            }
            return photonView;
        }
        private set { photonView = value; }
    }

    /// <summary>
    /// The center position for use in object spawn location calculations (use this instead of transform.position)
    /// </summary>
    public Vector2 SpawnCenter => (Vector2)transform.position + (Vector2.up * (transform.localScale.y / 2));
    /// <summary>
    /// The looking direction of the player. Includes magnitude, NOT a unit vector
    /// </summary>
    public Vector2 LookDirection => (Vector2)Utility.MouseWorldPos - SpawnCenter;

    public Stunnable stunnable { get; private set; }
    protected AminaPool aminaPool { get; private set; }
    protected HealthPool healthPool { get; private set; }
    protected Damager damager { get; private set; }
    private PlayerInput playerInput;
    public PlayerMovement playerMovement { get; private set; }
    protected StatKeeper statKeeper { get; private set; }
    protected StatusKeeper statusKeeper { get; private set; }
    private ObjectSpawner objectSpawner;
    protected SpriteRenderer sr;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Initialize components
        InitializeComponents();
        if (PV.IsMine)
        {
            //UI Hookup
            //TODO: Move this out of here
            FindObjectOfType<HitMarker>().Player = this;
            FindObjectOfType<AminaMeterController>().FocusAminaPool = aminaPool;
            //Hook up Stunnable with HealthPool
            healthPool.Start();
            healthPool.onMaxHealthChanged += (hp) => { damager.damage = hp; };
            damager.damage = healthPool.MaxHealth;
            healthPool.onDied += (hp) => { stunnable.triggerStun(); };
            stunnable.onStunned += (stunned) =>
            {
                this.enabled = !stunned;
                if (stunned)
                {
                    cancelAbilities();
                    //Damage other players while stunned
                    damager.damagableTypes.Add(EntityType.PLAYER);
                    damager.damageFriendlies = true;
                }
                else
                {
                    //Restore health after unstunned
                    healthPool.Health = healthPool.MaxHealth;
                    //Stop damaging other players upon recovering
                    damager.damagableTypes.Remove(EntityType.PLAYER);
                    damager.damageFriendlies = false;
                }
            };
            //Auto-Reloading
            if (aminaReloader)
            {
                aminaPool.onAminaEmpty += onAminaEmpty;
            }
            //PlayerMovement
            playerMovement.Start();
            //PlayerInput
            playerInput.onInputChanged += (inputState) =>
            {
                this.inputState = inputState;
            };
            //StatusKeeper
            statusKeeper.onStatusChanged += (status) =>
            {
                //Stunned
                //TODO: sync status effects through network
                //so you can refactor Stunnable into StatusKeeper
                //Stealthed
                sr.color = sr.color.setAlpha((status.stealthed) ? 0.1f : 1);
                //Rooted
                playerMovement.forceMovement(Vector2.zero, status.rooted);
            };
            //StatKeeper
            statKeeper.selfStats.onStatChanged += (stats) =>
            {
                playerMovement.MovementSpeed = stats.moveSpeed;
                healthPool.MaxHealth = stats.maxHits;
                damager.damage = stats.damage;
                transform.localScale = Vector3.one * stats.size;
                //Update status stealthed
                StatusLayer status = statusKeeper.AllowedStatus;
                status.stealthed = stats.size <= 1;
                statusKeeper.AllowedStatus = status;
            };
            statKeeper.triggerEvents();
        }
        //ObjectSpawner and Color
        objectSpawner.PlayerColor = playerColor;
        gameObject.FindComponents<SpriteRenderer>()
            .ForEach(sr => sr.color = playerColor);
        //Register with spawned damagers
        if (PV.IsMine)
        {
            objectSpawner.onObjectSpawned += (go, pos, dir) =>
            {
                Damager damager = go.FindComponent<Damager>();
                if (damager)
                {
                    damager.onDealtDamage += PlayerDealtDamage;
                }
            };
        }
    }
    private void InitializeComponents()
    {
        playerInput = gameObject.FindComponent<PlayerInput>();
        playerMovement = gameObject.FindComponent<PlayerMovement>();
        sr = gameObject.FindComponent<SpriteRenderer>();
        damager = gameObject.FindComponent<Damager>();
        stunnable = gameObject.FindComponent<Stunnable>();
        healthPool = gameObject.FindComponent<HealthPool>();
        aminaPool = gameObject.FindComponent<AminaPool>();
        statKeeper = gameObject.FindComponent<StatKeeper>();
        statusKeeper = gameObject.FindComponent<StatusKeeper>();
        objectSpawner = gameObject.FindComponent<ObjectSpawner>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        //Process Movement
        playerMovement.BasicMovement(inputState);
        //Processing Abilities
        bool processingHidesInputs = false;
        for (int i = processingAbilities.Count - 1; i >= 0; i--)
        {
            PlayerAbility ability = processingAbilities[i];
            ability.OnContinuedProcessing();
            if (ability.hidesOtherInputs)
            {
                processingHidesInputs = true;
            }
        }
        if (processingHidesInputs)
        {
            //don't do process the player inputs
            return;
        }
        //Ability Inputs
        foreach (PlayerAbility ability in abilityContext.abilities)
        {
            ButtonState button = inputState.Button(ability.abilitySlot);
            if (button.Bool())
            {
                switch (button)
                {
                    case ButtonState.DOWN: ability.OnButtonDown(); break;
                    case ButtonState.HELD: ability.OnButtonHeld(); break;
                    case ButtonState.UP: ability.OnButtonUp(); break;
                }
                if (ability.hidesOtherInputs)
                {
                    //Don't process other abilities
                    break;
                }
            }
        }
    }

    protected virtual void onAminaEmpty(float amina)
    {
        if (amina == 0 && aminaPool.ReservedAmina == 0 && !aminaReloader.Reloading)
        {
            aminaReloader.reload();
        }
    }

    public void PlayerDealtDamage(float damage, EntityType type)
    {
        if (type == EntityType.PLAYER)
        {
            onDamagedPlayer?.Invoke();
        }
    }
    public delegate void OnDamagedPlayer();
    public event OnDamagedPlayer onDamagedPlayer;

    public void processAbility(PlayerAbility ability, bool process = true)
    {
        if (process)
        {
            processingAbilities.Add(ability);
        }
        else
        {
            processingAbilities.Remove(ability);
        }
        if (processingAbilities.Count == 0)
        {
            onProcessingFinished?.Invoke();
        }
    }
    public delegate void OnProcessingFinished();
    public event OnProcessingFinished onProcessingFinished;

    public void cancelAbilities()
    {
        //Cancel abilities
        foreach (PlayerAbility ability in abilityContext.abilities)
        {
            ButtonState button = inputState.Button(ability.abilitySlot);
            if (button.Bool())
            {
                ability.OnButtonCanceled();
            }
        }
        //Stop processing ongoing abilities
        processingAbilities.Clear();
    }
}
