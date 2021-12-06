using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=JjfPaY57dDM

    private PhotonView PV;
    public GameObject myAvatar;

    public static PhotonPlayer localPlayer;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            localPlayer = this;
            StartCoroutine(spawn());
        }
    }

    IEnumerator spawn()
    {
        Transform spawn = GameSetup.instance.spawnPoint;
        myAvatar = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", "PlayerAvatar"),
            spawn.position,
            Quaternion.identity
            );
        TeamToken teamToken = TeamToken.getTeamToken(myAvatar);
        teamToken.assignTeam();
        while (teamToken.teamCaptain == null)
        {
            yield return null;
        }
        spawn = teamToken.teamCaptain.getNextSpawnPoint();
        Rigidbody2D rb2d = myAvatar.GetComponent<Rigidbody2D>();
        rb2d.isKinematic = true;
        while (myAvatar.transform.position != spawn.position)
        {
            myAvatar.transform.position = spawn.position;
            yield return null;
        }
        rb2d.isKinematic = false;
        yield return null;
    }
}
