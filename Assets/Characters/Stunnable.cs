using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunnable : MonoBehaviour
{
    public float duration = 2;
    public float knockbackDistance = 5;
    //the scripts to be turned off while stunned
    public List<MonoBehaviour> stunnableScripts = new List<MonoBehaviour>();

    private float stunDuration = 0;

    private Rigidbody2D rb2d;
    protected PhotonView photonView;
    public PhotonView PV
    {
        get
        {
            if (photonView == null)
            {
                photonView = GetComponent<PhotonView>();
            }
            if (photonView == null)
            {
                photonView = GetComponentInParent<PhotonView>();
            }
            return photonView;
        }
    }
    private BlinkEffect blinkEffect;

    private void Start()
    {
        rb2d = GetComponentInParent<Rigidbody2D>();
        blinkEffect = GetComponentInParent<BlinkEffect>();
    }

    private void Update()
    {
        if (Stunned)
        {
            stunDuration -= Time.deltaTime;
            if (stunDuration <= 0)
            {
                rb2d.velocity = Vector2.zero;
                enableScripts(true);
                blinkEffect.Blinking = false;
                onStunEnded?.Invoke();
            }
        }
    }
    public delegate void OnStunEnded();
    public event OnStunEnded onStunEnded;

    public bool Stunned
    {
        get
        {
            if (PV.IsMine)
            {
                return stunDuration > 0;
            }
            else
            {
                return blinkEffect.Blinking;
            }
        }
    }

    public void stun()
    {
        stunDuration = duration;
        float knockbackPerSecond = knockbackDistance / stunDuration;
        rb2d.velocity =
            (transform.position - CaravanController.Caravan.transform.position).normalized
            * knockbackPerSecond;
        enableScripts(false);

        blinkEffect.Blinking = true;
    }

    public void triggerStun()
    {
        if (!Stunned)
        {
            if (PV.IsMine)
            {
                PV.RPC("RPC_Stun", RpcTarget.All);
                stun();
            }
        }
    }

    //Moved to AvatarSetup because it's on the object with the PhotonView
    //[PunRPC]
    //void RPC_Stun()
    //{
    //    stun();
    //}

    private void enableScripts(bool enable)
    {
        foreach (MonoBehaviour mb in stunnableScripts)
        {
            mb.enabled = enable;
        }
    }
}
