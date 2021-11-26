using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AminaMeterController : MonoBehaviour
{
    [Tooltip("How many seconds after spending amina it waits before starting the drain animation")]
    public float aminaDrainAminDelay = 0.5f;
    [Tooltip("How many amina ticks per second it drains during the animation")]
    public float aminaDrainAnimRate = 5;
    [Range(0,1)]
    [Tooltip("How much to increase color brightness by")]
    public float brightnessAdjust = 0.1f;

    //public float maxAnimaPerRing = 100;

    [SerializeField]
    private AminaPool focusAminaPool;
    public AminaPool FocusAminaPool
    {
        get => focusAminaPool;
        set
        {
            if (focusAminaPool)
            {
                focusAminaPool.onAminaChanged -= onAminaChanged;
            }
            focusAminaPool = value;
            if (focusAminaPool)
            {
                focusAminaPool.onAminaChanged -= onAminaChanged;
                focusAminaPool.onAminaChanged += onAminaChanged;
                prevAmina = focusAminaPool.Amina;
                onAminaChanged(prevAmina);
                //Meter color
                updateMeterColor(focusAminaPool.gameObject.FindComponent<SpriteRenderer>().color);                
            }
        }
    }

    public Image aminaMeter;
    public Image aminaReserveMeter;

    private float prevAmina = 0;
    private float goalAmina = 0;

    private float aminaDrainAnimStartTime = -1;

    private void Start()
    {
        if (!focusAminaPool)
        {
            updateMeter(0, 1, 0);
        }
    }

    void updateMeterColor(Color meterColor)
    {
        aminaMeter.color = meterColor;
        Color c = meterColor;
        c.r += brightnessAdjust;
        c.g += brightnessAdjust;
        c.b += brightnessAdjust;
        c.a = aminaReserveMeter.color.a;
        aminaReserveMeter.color = c;
    }

    void onAminaChanged(float amina)
    {
        float reservedAmina = focusAminaPool.ReservedAmina;
        if (reservedAmina > 0)
        {
            prevAmina = amina + reservedAmina;
            updateMeter(amina, focusAminaPool.maxAmina, reservedAmina);
            aminaDrainAnimStartTime = Time.time + aminaDrainAminDelay;
            goalAmina = amina;
        }
        else
        {
            if (aminaDrainAnimStartTime != -1)
            {
                prevAmina = goalAmina;
            }
            float delta = prevAmina - amina;
            updateMeter(amina, focusAminaPool.maxAmina, delta);
            aminaDrainAnimStartTime = Time.time + aminaDrainAminDelay;
            goalAmina = amina;
        }
        if (prevAmina < amina)
        {
            prevAmina = amina;
        }
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
        //Amina Drain Animation
        if (aminaDrainAnimStartTime != -1 && Time.time >= aminaDrainAnimStartTime)
        {
            prevAmina -= aminaDrainAnimRate * Time.deltaTime;
            float delta = prevAmina - goalAmina;
            updateMeter(goalAmina, focusAminaPool.maxAmina, delta);
            if (prevAmina < goalAmina)
            {
                prevAmina = goalAmina;
                aminaDrainAnimStartTime = -1;
            }
        }
    }

    void updateMeter(float amina, float maxAmina, float reservedAmina)
    {
        aminaMeter.fillAmount = amina / maxAmina;
        aminaReserveMeter.fillAmount = (amina + reservedAmina) / maxAmina;
    }
}
