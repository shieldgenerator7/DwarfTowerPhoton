using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitGiveAmina : MonoBehaviour
{
    public float aminaToGive = 10;

    private List<AminaPool> receipients = new List<AminaPool>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AminaPool aminaPool = collision.gameObject.FindComponent<AminaPool>();
        if (aminaPool)
        {
            if (!receipients.Contains(aminaPool))
            {
                aminaPool.rechargeAmina(aminaToGive);
                receipients.Add(aminaPool);
            }
        }
    }
}
