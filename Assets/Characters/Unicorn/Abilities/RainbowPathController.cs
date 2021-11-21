using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowPathController : MonoBehaviour
{
    public float speedMultiplier = 1.5f;
    public float slowMultiplier = 0.75f;
    public float duration = 10;
    public float fadeSpeed = 1;

    private Vector2 _startPos;
    public Vector2 startPos
    {
        get => _startPos;
        set
        {
            _startPos = value;
            reposition();
        }
    }
    private Vector2 _endPos;
    public Vector2 endPos
    {
        get => _endPos;
        set
        {
            _endPos = value;
            reposition();
        }
    }

    private float startTime;
    private PhotonView PV;

    private void Start()
    {
        startTime = Time.time;
        PV = GetComponent<PhotonView>();
    }

    private void reposition()
    {
        transform.position = startPos;
        Vector2 dir = endPos - startPos;
        transform.up = dir;
        Vector3 scale = transform.localScale;
        scale.y = dir.magnitude;
        transform.localScale = scale;
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if (Time.time - startTime >= duration)
            {
                Vector2 dir = (endPos - startPos);
                float step = fadeSpeed * Time.deltaTime;
                if (dir.magnitude > step)
                {
                    startPos += dir.normalized * step;
                }
                else
                {
                    PhotonNetwork.Destroy(this.gameObject);
                }
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
