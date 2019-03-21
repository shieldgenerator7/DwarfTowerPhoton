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
            TeamToken teamCaptain = TeamToken.getTeamWithFewestPlayers();
            List<GameObject> teamSpawns = new List<GameObject>();
            foreach(Transform t in teamCaptain.transform)
            {
                if (t.CompareTag("Respawn"))
                {
                    teamSpawns.Add(t.gameObject);
                }
            }
            int spawnPickIndex = Random.Range(0, teamSpawns.Count);
            GameObject spawn = teamSpawns[spawnPickIndex];
            myAvatar = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                spawn.transform.position,
                spawn.transform.rotation,
                0
                );
            myAvatar.GetComponent<TeamToken>().seeRecruiter(teamCaptain);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
