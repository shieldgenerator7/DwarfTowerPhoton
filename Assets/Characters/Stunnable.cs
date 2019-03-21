using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunnable : MonoBehaviour
{
    //the scripts to be turned off while stunned
    public List<MonoBehaviour> stunnableScripts = new List<MonoBehaviour>();
    
    private float stunDuration;

    private Rigidbody2D rb2d;
    public PhotonView PV;

    private void Start()
    {
        rb2d = GetComponentInParent<Rigidbody2D>();
        PV = GetComponentInParent<PhotonView>();
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
            }
        }
    }

    public bool Stunned
    {
        get { return stunDuration > 0; }
    }

    public void stun(float duration, float knockbackDistance)
    {
        stunDuration = duration;
        float knockbackPerSecond = knockbackDistance / stunDuration;
        TeamToken teamCaptain = TeamToken.getTeamToken(gameObject).teamCaptain;
        rb2d.velocity =
            (teamCaptain.transform.position - transform.position).normalized
            * knockbackPerSecond;
        enableScripts(false);
    }

    private void enableScripts(bool enable)
    {
        foreach(MonoBehaviour mb in stunnableScripts)
        {
            mb.enabled = enable;
        }
    }
}
