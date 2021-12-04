using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AminaPool : MonoBehaviour
{
    [Tooltip("The max amount of amina this pool can have")]
    public float maxAmina = 100;

    /// <summary>
    /// The current amount of amina
    /// </summary>
    public float Amina
    {
        get { return amina; }
        private set
        {
            float prevAmina = amina;
            //Set amina
            amina = Mathf.Clamp(value, 0, maxAmina);
            //Delegates
            if (prevAmina != amina)
            {
                onAminaChanged?.Invoke(amina);
            }
            if (amina == maxAmina)
            {
                onAminaFull?.Invoke(amina);
            }
            if (amina == 0)
            {
                onAminaEmpty?.Invoke(amina);
            }
        }
    }
    private float amina;//basically mana
    public delegate void AminaEvent(float amina);
    public event AminaEvent onAminaFull;
    public event AminaEvent onAminaEmpty;
    public event AminaEvent onAminaChanged;

    /// <summary>
    /// The current amount of reserved amina,
    /// Amina reserved for a specific ability that requires charge time
    /// </summary>
    public float ReservedAmina
    {
        get => reservedAmina;
        private set => reservedAmina = Mathf.Clamp(value, 0, maxAmina);
    }
    private float reservedAmina;

    private void Start()
    {
        Amina = maxAmina;
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

    public float requestAminaPerSecond(float amount, bool acceptPartialAmount = true)
    {
        amount = amount * Time.deltaTime;
        return requestAmina(amount, acceptPartialAmount);
    }

    public void reserveAmina(float amount)
    {
        ReservedAmina += requestAmina(amount);
    }

    public float collectReservedAmina()
    {
        float reserves = ReservedAmina;
        ReservedAmina = 0;
        //Call Amina delegates again
        Amina = Amina;
        //Return reserves
        return reserves;
    }

    public void cancelReservedAmina()
    {
        float reservedAmina = ReservedAmina;
        ReservedAmina = 0;
        rechargeAmina(reservedAmina);
    }

    public void rechargeAmina(float amount)
    {
        Amina += amount;
    }

    public void drainAmina(float amount)
    {
        Amina -= amount;
    }
}
