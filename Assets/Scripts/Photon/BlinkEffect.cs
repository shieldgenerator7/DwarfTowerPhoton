using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField]
    private float blinkSpeed = 0.1f;//time between blinks
    private float lastBlinkTime;

    public bool Blinking
    {
        get { return lastBlinkTime > 0; }
        set
        {
            if (PV.IsMine)
            {
                bool prevValue = Blinking;
                if (prevValue != value)
                {
                    PV.RPC("RPC_Blink", RpcTarget.AllViaServer, value);
                }
            }
        }
    }

    List<SpriteRenderer> srs;

    private PhotonView PV;
    private void Start()
    {
        PV = GetComponent<PhotonView>();
        srs = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
    }

    private void Update()
    {
        if (Blinking)
        {
            if (Time.time > lastBlinkTime + blinkSpeed)
            {
                lastBlinkTime += blinkSpeed;
                blinkAll();
            }
        }
    }

    [PunRPC]
    void RPC_Blink(bool blink)
    {
        if (blink)
        {
            lastBlinkTime = Time.time;
            blinkAll();
        }
        else
        {
            lastBlinkTime = 0;
            blinkAll(true);
        }
    }

    private void blinkAll(bool forceOpaque = false)
    {
        foreach (SpriteRenderer sr in srs)
        {
            blink(sr, forceOpaque);
        }
    }

    private void blink(SpriteRenderer sr, bool forceOpaque = false)
    {
        Color c = sr.color;
        c.a = (forceOpaque) ? 1 : 1 - c.a;
        sr.color = c;
    }
}
