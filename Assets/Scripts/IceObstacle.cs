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

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.gameObject.FindComponent<PlayerMovement>();
        if (playerMovement && playerMovement.isPhotonViewMine())
        {
            slipPlayer(playerMovement, true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.gameObject.FindComponent<PlayerMovement>();
        if (playerMovement && playerMovement.isPhotonViewMine())
        {
            slipPlayer(playerMovement, true);
            if (Mathf.Approximately(0, playerMovement.rb2d.velocity.magnitude))
            {
                slipPlayer(playerMovement, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.gameObject.FindComponent<PlayerMovement>();
        if (playerMovement && playerMovement.isPhotonViewMine())
        {
            slipPlayer(playerMovement, false);
        }
    }

    void slipPlayer(PlayerMovement playerMovement, bool slip)
    {
        StatKeeper statKeeper = playerMovement.gameObject.FindComponent<StatKeeper>();
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
