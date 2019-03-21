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
            int spawnPickIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1 % GameSetup.instance.spawnPoints.Length;
            GameObject spawn = GameSetup.instance.spawnPoints[spawnPickIndex];
            myAvatar = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                spawn.transform.position,
                spawn.transform.rotation,
                0
                );
            myAvatar.GetComponent<TeamToken>().seeRecruiter(TeamToken.getTeamToken(spawn));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
