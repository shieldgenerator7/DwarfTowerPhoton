using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthPool))]
public class OnDieDestroy : MonoBehaviour
{
    private PhotonView PV;
    private HealthPool healthPool;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        healthPool = GetComponent<HealthPool>();
        healthPool.onDied += (hp) =>
        {
            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
            else
            {
                GetComponent<Collider2D>().enabled = false;
            }
        };
    }
}
