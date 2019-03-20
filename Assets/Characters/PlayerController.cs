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

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponentInParent<PhotonView>().IsMine)
        {
            Amina = maxAmina;
            FindObjectOfType<AminaMeterController>().FocusPlayerController = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
}
