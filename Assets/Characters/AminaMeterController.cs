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

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (focusPC)
        {
            updateMeter(focusPC.Amina, focusPC.maxAmina);
        }
        else
        {
            updateMeter(0, 1);
        }
        transform.position = Input.mousePosition;
    }

    void updateMeter(float amina, float maxAnima)
    {
        image.fillAmount = amina / maxAnima;
    }
}
