using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceObstacle : MonoBehaviour
{
    public StatLayer slipLayer;

    private PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = gameObject.FindComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ComponentContext compContext = collision.gameObject.FindComponent<ComponentContext>();
        if (compContext.playerMovement && compContext.PV.IsMine)
        {
            slipPlayer(compContext, true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ComponentContext compContext = collision.gameObject.FindComponent<ComponentContext>();
        if (compContext.playerMovement && compContext.PV.IsMine)
        {
            slipPlayer(compContext, true);
            if (Mathf.Approximately(0, compContext.rb2d.velocity.magnitude))
            {
                slipPlayer(compContext, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ComponentContext compContext = collision.gameObject.FindComponent<ComponentContext>();
        if (compContext.playerMovement && compContext.PV.IsMine)
        {
            slipPlayer(compContext, false);
        }
    }

    void slipPlayer(ComponentContext compContext, bool slip)
    {
        PlayerMovement playerMovement = compContext.playerMovement;
        StatKeeper statKeeper = compContext.statKeeper;
        if (slip)
        {
            playerMovement.forceMovement(playerMovement.LastMoveDirection);
            statKeeper.selfStats.addLayer(PV.ViewID, slipLayer);
        }
        else
        {
            playerMovement.forceMovement(false);
            statKeeper.selfStats.removeLayer(PV.ViewID);
        }
    }
}
