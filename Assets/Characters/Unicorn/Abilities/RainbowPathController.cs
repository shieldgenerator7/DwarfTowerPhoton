using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowPathController : MonoBehaviour
{
    public StatLayer teamMultiplier;
    public StatLayer enemyMultiplier;
    public float duration = 10;
    public float fadeSpeed = 1;
    [Tooltip("How much amina to regen each second while active")]
    public float aminaRegenRate = 10;

    /// <summary>
    /// The ability id to use when granting speed boosts
    /// </summary>
    public int abilityID { get; set; }

    private Vector2 _startPos;
    public Vector2 startPos
    {
        get => _startPos;
        set
        {
            _startPos = value;
            reposition();
            PV.RPC("RPC_Reposition", RpcTarget.Others, _startPos, _endPos);
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
            PV.RPC("RPC_Reposition", RpcTarget.Others, _startPos, _endPos);
        }
    }

    private float startTime = -1;
    private bool prevDestroyed = false;
    private PhotonView PV;

    public void Start()
    {
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
            if (prevDestroyed && startTime != -1
                && Time.time >= startTime + duration
                )
            {
                Vector2 dir = (endPos - startPos);
                float step = fadeSpeed * Time.deltaTime;
                if (dir.magnitude > step)
                {
                    startPos += dir.normalized * step;
                }
                else
                {
                    onDestroy?.Invoke(this);
                    PhotonNetwork.Destroy(this.gameObject);
                }
            }
        }
    }
    public delegate void OnDestroy(RainbowPathController rpc);
    public event OnDestroy onDestroy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        checkAddLayer(collision.gameObject);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        checkAddLayer(collision.gameObject);
        //Add amina if it's moving
        AminaPool aminaPool = collision.gameObject.FindComponent<AminaPool>();
        if (aminaPool)
        {
            bool onSameTeam = TeamToken.onSameTeam(gameObject, collision.gameObject);
            if (onSameTeam)
            {
                Rigidbody2D rb2d = collision.gameObject.FindComponent<Rigidbody2D>();
                if (rb2d && rb2d.velocity.magnitude > 0)
                {

                    aminaPool.rechargeAmina(aminaRegenRate * Time.deltaTime);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        checkAddLayer(collision.gameObject, false);
    }
    private void checkAddLayer(GameObject go, bool add = true)
    {
        StatKeeper sk = go.GetComponent<StatKeeper>();
        if (sk)
        {
            if (add)
            {
                bool onSameTeam = TeamToken.onSameTeam(gameObject, go);
                if (onSameTeam)
                {
                    sk.selfStats.addLayer(abilityID, teamMultiplier);
                }
                else
                {
                    sk.selfStats.addLayer(abilityID, enemyMultiplier);
                }
            }
            else
            {
                sk.selfStats.removeLayer(abilityID);
            }
        }
    }

    public void finish(RainbowPathController previous)
    {
        //Start the timer for fading
        startTime = Time.time;
        //Register previous delegate
        if (previous)
        {
            previous.onDestroy += (rpc) => { prevDestroyed = true; };
        }
        else
        {
            prevDestroyed = true;
        }
    }

    [PunRPC]
    void RPC_Reposition(Vector2 start, Vector2 end)
    {
        _startPos = start;
        _endPos = end;
        reposition();
    }
}
