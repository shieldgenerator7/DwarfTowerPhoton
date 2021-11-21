using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField]
    private float blinkSpeed = 0.1f;//time between blinks
    private float lastBlinkTime = 0;

    public bool Blinking
    {
        get { return lastBlinkTime > 0; }
        set
        {
            bool blinking = value;
            setBlink(blinking);
        }
    }

    List<SpriteRenderer> srs;

    protected PhotonView photonView;
    public PhotonView PV
    {
        get
        {
            if (photonView == null)
            {
                photonView = GetComponent<PhotonView>();
            }
            return photonView;
        }
    }
    private void Start()
    {
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
        setBlink(blink);
    }

    public void setBlink(bool blink)
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
