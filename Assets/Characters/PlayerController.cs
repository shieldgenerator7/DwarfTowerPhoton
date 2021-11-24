using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public AminaReloader aminaReloader;//ability called by default when the player runs out of amina
    [SerializeField]
    private AbilityContext abilityContext;
    public AbilityContext AbilityContext
    {
        get { return abilityContext; }
        set { abilityContext = value; }
    }

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

    protected AminaPool aminaPool;

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            //Hook up Stunnable with HealthPool
            Stunnable stunnable = gameObject.FindComponent<Stunnable>();
            HealthPool healthPool = gameObject.FindComponent<HealthPool>();
            healthPool.onDied += () => { stunnable.triggerStun(); };
            stunnable.onStunned += (stunned) =>
            {
                if (!stunned)
                {
                    healthPool.Health = healthPool.MaxHealth;
                }
            };
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
        }
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
}
