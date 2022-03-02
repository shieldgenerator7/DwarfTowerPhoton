using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusTrigger : MonoBehaviour
{
    [Tooltip("The status to grant while an entity is in this trigger")]
    public StatusLayer statusToGrant;
    [Tooltip("Does the entity's position have to be in the collider to get the status?")]
    public bool requirePositionInTrigger = true;

    private PhotonView PV;
    private Collider2D coll2d;
    private int viewId;

    private void Start()
    {
        PV = gameObject.FindComponent<PhotonView>();
        viewId = PV.ViewID;
        coll2d = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        checkGrantStatus(collision.gameObject, true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        checkGrantStatus(collision.gameObject, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        checkGrantStatus(collision.gameObject, false);
    }

    void checkGrantStatus(GameObject go, bool grant)
    {
        StatusKeeper statusKeeper = go.FindComponent<StatusKeeper>();
        if (statusKeeper)
        {
            bool posReq = !requirePositionInTrigger
                || !coll2d
                || coll2d.OverlapPoint(go.transform.position);
            if (grant && posReq)
            {
                statusKeeper.addLayer(viewId, statusToGrant.Copy());
            }
            else
            {
                statusKeeper.removeLayer(viewId);
            }
        }
    }
}
