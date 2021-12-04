using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunnable : MonoBehaviour
{
    public float stunDuration = 2;
    public float knockbackDistance = 5;

    private float stunTimeLeft = 0;

    private Rigidbody2D rb2d;
    protected PhotonView PV;

    private void Start()
    {
        rb2d = gameObject.FindComponent<Rigidbody2D>();
        PV = gameObject.FindComponent<PhotonView>();
    }

    private void Update()
    {
        if (Stunned)
        {
            stunTimeLeft -= Time.deltaTime;
            if (stunTimeLeft <= 0)
            {
                if (PV.IsMine)
                {
                    rb2d.velocity = Vector2.zero;
                }
                onStunned?.Invoke(false);
            }
        }
    }
    public delegate void OnStunned(bool stunned);
    public event OnStunned onStunned;

    public bool Stunned => stunTimeLeft > 0;

    public void stun()
    {
        stunTimeLeft = stunDuration;
        if (PV.IsMine)
        {
            float knockbackPerSecond = knockbackDistance / stunTimeLeft;
            rb2d.velocity =
                (transform.position - CaravanController.Caravan.transform.position).normalized
                * knockbackPerSecond;
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
}
