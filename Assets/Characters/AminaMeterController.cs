using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AminaMeterController : MonoBehaviour
{
    //public float maxAnimaPerRing = 100;

    [SerializeField]
    private AminaPool focusAminaPool;
    public AminaPool FocusAminaPool
    {
        get { return focusAminaPool; }
        set { focusAminaPool = value; }
    }

    public Image aminaMeter;
    public Image aminaReserveMeter;

    private void Start()
    {
        if (focusAminaPool)
        {
            focusAminaPool.onAminaChanged += onAminaChanged;
            onAminaChanged(focusAminaPool.Amina);
        }
        else
        {
            updateMeter(0, 1, 0);
        }
    }

    void onAminaChanged(float amina)
    {
        updateMeter(amina, focusAminaPool.maxAmina, focusAminaPool.ReservedAmina);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    void updateMeter(float amina, float maxAmina, float reservedAmina)
    {
        aminaMeter.fillAmount = amina / maxAmina;
        aminaReserveMeter.fillAmount = (amina + reservedAmina) / maxAmina;
    }
}
