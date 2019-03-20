using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AminaMeterController : MonoBehaviour
{
    //public float maxAnimaPerRing = 100;

    [SerializeField]
    private PlayerController focusPC;
    public PlayerController FocusPlayerController
    {
        get { return focusPC; }
        set { focusPC = value; }
    }

    public Image aminaMeter;
    public Image aminaReserveMeter;
    
    private void Update()
    {
        if (focusPC)
        {
            updateMeter(focusPC.Amina, focusPC.maxAmina, focusPC.ReservedAmina);
        }
        else
        {
            updateMeter(0, 1, 0);
        }
        transform.position = Input.mousePosition;
    }

    void updateMeter(float amina, float maxAmina, float reservedAmina)
    {
        aminaMeter.fillAmount = amina / maxAmina;
        aminaReserveMeter.fillAmount = (amina + reservedAmina) / maxAmina;
    }
}
