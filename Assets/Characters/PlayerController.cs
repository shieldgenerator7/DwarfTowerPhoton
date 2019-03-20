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
        private set { amina = value; }
    }

    public List<PlayerAbility> abilities = new List<PlayerAbility>();

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
        foreach (PlayerAbility ability in abilities)
        {
            if (Input.GetButton(ability.buttonName))
            {
                if (Input.GetButtonDown(ability.buttonName))
                {
                    ability.OnButtonDown();
                }
                else if (Input.GetButtonUp(ability.buttonName))
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
}
