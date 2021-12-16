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
            playerMovement.forceMovement(playerMovement.LastMoveDirection);
            StatKeeper statKeeper = playerMovement.gameObject.FindComponent<StatKeeper>();
            statKeeper.selfStats.addLayer(PV.ViewID, slipLayer);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.gameObject.FindComponent<PlayerMovement>();
        if (playerMovement && playerMovement.isPhotonViewMine())
        {
            playerMovement.forceMovement(false);
            StatKeeper statKeeper = playerMovement.gameObject.FindComponent<StatKeeper>();
            statKeeper.selfStats.removeLayer(PV.ViewID);
        }
    }
}
