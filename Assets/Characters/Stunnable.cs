﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunnable : MonoBehaviour
{
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
            }
        }
    }

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

    public void stun(float duration, float knockbackDistance)
    {
        stunDuration = duration;
        float knockbackPerSecond = knockbackDistance / stunDuration;
        TeamToken teamCaptain = TeamToken.getTeamToken(gameObject).teamCaptain;
        rb2d.velocity =
            (transform.position - CaravanController.Caravan.transform.position).normalized
            * knockbackPerSecond;
        enableScripts(false);

        blinkEffect.Blinking = true;
        if (PV.IsMine)
        {
            blinkEffect.setBlink(true);
        }
    }

    private void enableScripts(bool enable)
    {
        foreach (MonoBehaviour mb in stunnableScripts)
        {
            mb.enabled = enable;
        }
    }
}
