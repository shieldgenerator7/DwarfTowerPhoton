using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=JjfPaY57dDM

    private PhotonView PV;
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            int spawnPicker = Random.Range(0, GameSetup.instance.spawnPoints.Length);
            myAvatar = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                GameSetup.instance.spawnPoints[spawnPicker].position,
                GameSetup.instance.spawnPoints[spawnPicker].rotation,
                0
                );
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
