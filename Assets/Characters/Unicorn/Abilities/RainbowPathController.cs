using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowPathController : MonoBehaviour
{
    public float speedMultiplier = 1.5f;
    public float slowMultiplier = 0.75f;
    public float duration = 10;

    private float startTime;
    private PhotonView PV;

    private void Start()
    {
        startTime = Time.time;
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if (Time.time - startTime >= duration)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();
        if (pm)
        {
            bool onSameTeam = TeamToken.onSameTeam(gameObject, collision.gameObject);
            if (onSameTeam)
            {
                pm.movementSpeed *= speedMultiplier;
            }
            else
            {
                pm.movementSpeed *= slowMultiplier;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();
        if (pm)
        {
            bool onSameTeam = TeamToken.onSameTeam(gameObject, collision.gameObject);
            if (onSameTeam)
            {
                pm.movementSpeed /= speedMultiplier;
            }
            else
            {
                pm.movementSpeed /= slowMultiplier;
            }
        }
    }
}
