using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Color playerColor = Color.white;
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

    public enum InputState
    {
        NONE,
        DOWN,
        HELD,
        UP
    }
    public Dictionary<string, InputState> inputs = new Dictionary<string, InputState>();

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

    public Vector2 SpawnCenter => (Vector2)transform.position + (Vector2.up * 0.5f);
    /// <summary>
    /// The looking direction of the player. Includes magnitude, NOT a unit vector
    /// </summary>
    public Vector2 LookDirection => (Vector2)Utility.MouseWorldPos - SpawnCenter;

    public Stunnable stunnable { get; private set; }
    protected AminaPool aminaPool;
    public PlayerMovement playerMovement { get; private set; }
    protected StatKeeper statKeeper;
    private ObjectSpawner objectSpawner;

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            //Hook up Stunnable with HealthPool
            stunnable = gameObject.FindComponent<Stunnable>();
            HealthPool healthPool = gameObject.FindComponent<HealthPool>();
            healthPool.onDied += () => { stunnable.triggerStun(); };
            stunnable.onStunned += (stunned) =>
            {
                if (stunned)
                {
                    cancelAbilities();
                }
                else
                {
                    //Restore health after unstunned
                    healthPool.Health = healthPool.MaxHealth;
                }
            };
            //StatKeeper
            statKeeper = gameObject.FindComponent<StatKeeper>();
            statKeeper.selfStats.onStatChanged += (stats) =>
            {
                healthPool.MaxHealth = stats.maxHits;
            };
            statKeeper.triggerEvents();
            //Amina
            aminaPool = gameObject.FindComponent<AminaPool>();
            FindObjectOfType<AminaMeterController>().FocusAminaPool = aminaPool;
            //Auto-Reloading
            if (aminaReloader)
            {
                aminaPool.onAminaEmpty += (amina) =>
                {
                    if (amina == 0 && aminaPool.ReservedAmina == 0 && !aminaReloader.Reloading)
                    {
                        aminaReloader.reload();
                    }
                };
            }
            //PlayerMovement
            playerMovement = gameObject.FindComponent<PlayerMovement>();
        }
        //ObjectSpawner and Color
        objectSpawner = gameObject.FindComponent<ObjectSpawner>();
        objectSpawner.PlayerColor = playerColor;
        gameObject.FindComponents<SpriteRenderer>()
            .ForEach(sr => sr.color = playerColor);
        //Inputs
        foreach (string input in new string[] { "Ability1", "Ability2", "Ability3", "Reload" })
        {
            inputs.Add(input, InputState.NONE);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
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
            bool buttonUp = Input.GetButtonUp(ability.buttonName);
            if (Input.GetButton(ability.buttonName) || buttonUp)
            {
                if (Input.GetButtonDown(ability.buttonName))
                {
                    ability.OnButtonDown();
                }
                else if (buttonUp)
                {
                    ability.OnButtonUp();
                }
                else
                {
                    ability.OnButtonHeld();
                }
                if (ability.hidesOtherInputs)
                {
                    //Don't process other abilities
                    break;
                }
            }
        }
    }

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
    }

    public void cancelAbilities()
    {
        //Cancel abilities
        foreach (PlayerAbility ability in abilityContext.abilities)
        {
            bool buttonUp = Input.GetButtonUp(ability.buttonName);
            if (Input.GetButton(ability.buttonName) || buttonUp)
            {
                ability.OnButtonCanceled();
            }
        }
        //Stop processing ongoing abilities
        processingAbilities.Clear();
    }
}
