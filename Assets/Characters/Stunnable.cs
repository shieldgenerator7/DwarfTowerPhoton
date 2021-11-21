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

    private void Start()
    {
        rb2d = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Stunned)
        {
            stunDuration -= Time.deltaTime;
            if (stunDuration <= 0)
            {
                if (PV.IsMine)
                {
                    rb2d.velocity = Vector2.zero;
                    enableScripts(true);
                }
                onStunned?.Invoke(false);
            }
        }
    }
    public delegate void OnStunned(bool stunned);
    public event OnStunned onStunned;

    public bool Stunned => stunDuration > 0;

    public void stun()
    {
        stunDuration = duration;
        if (PV.IsMine)
        {
            float knockbackPerSecond = knockbackDistance / stunDuration;
            rb2d.velocity =
                (transform.position - CaravanController.Caravan.transform.position).normalized
                * knockbackPerSecond;
            enableScripts(false);
        }
        onStunned?.Invoke(true);
    }

    public void triggerStun()
    {
        if (!Stunned)
        {
            if (PV.IsMine)
            {
                PV.RPC("RPC_Stun", RpcTarget.All);
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
