using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxAmina = 100;
    [SerializeField]
    private float amina;//basically mana
    public float Amina
    {
        get { return amina; }
        set { amina = Mathf.Clamp(value, 0, maxAmina); }
    }
    [SerializeField]
    private float reservedAmina;//amina reserved for a specific ability that requires charge time
    public float ReservedAmina
    {
        get { return reservedAmina; }
        private set { reservedAmina = Mathf.Clamp(value, 0, maxAmina); }
    }

    public AminaReloader aminaReloader;//ability called by default when the player runs out of amina
    public List<PlayerAbility> abilities = new List<PlayerAbility>();

    private List<PlayerAbility> processingAbilities = new List<PlayerAbility>();//used for abilities that have lasting effects

    private PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponentInParent<PhotonView>();
        if (PV.IsMine)
        {
            Amina = maxAmina;
            FindObjectOfType<AminaMeterController>().FocusPlayerController = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        //Auto-Reloading
        if (Amina == 0 && !aminaReloader.Reloading)
        {
            aminaReloader.reload();
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
        foreach (PlayerAbility ability in abilities)
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

    public bool hasAmina(float amount, bool acceptPartialAmount = true)
    {
        return Amina >= amount
            || (acceptPartialAmount && Amina > 0);
    }

    public float requestAmina(float amount, bool acceptPartialAmount = true)
    {
        if (Amina - amount >= 0)
        {
            Amina -= amount;
            return amount;
        }
        else if (acceptPartialAmount)
        {
            amount = Amina;
            Amina = 0;
            return amount;
        }
        return 0;
    }

    public void reserveAmina(float amount)
    {
        ReservedAmina += requestAmina(amount);
    }

    public float collectReservedAmina()
    {
        float reserves = ReservedAmina;
        ReservedAmina = 0;
        return reserves;
    }

    public void cancelReservedAmina()
    {
        rechargeAmina(ReservedAmina);
        ReservedAmina = 0;
    }

    public void rechargeAmina(float amount)
    {
        Amina += amount;
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
