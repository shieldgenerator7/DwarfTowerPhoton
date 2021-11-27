using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//TODO: Get rid of this class (probably)
//and then have each object spawning class spawn multiple shots

/// <summary>
/// Spawns its objects as soon as it can and then destroys itself
/// </summary>
public class ObjectAutoSpawner : MonoBehaviour
{
    public string folderName;
    public List<string> objectNames;
    public bool waitForTeamToken = false;

    public Color PlayerColor { get; set; } = Color.white;

    private TeamToken teamToken;
    private PhotonView PV;

    private void Start()
    {
        teamToken = TeamToken.getTeamToken(gameObject);
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (PV.IsMine)
        {
            if (teamToken.teamCaptain != null || !waitForTeamToken)
            {
                foreach (string objectName in objectNames)
                {
                    GameObject go = PhotonNetwork.Instantiate(
                        Path.Combine("PhotonPrefabs", folderName, objectName),
                        transform.position,
                        transform.rotation
                        );
                    //Color
                    ShotController sc = go.FindComponent<ShotController>();
                    if (sc)
                    {
                        sc.setColor(PlayerColor);
                    }
                    //Delegate
                    if (onObjectSpawned != null)
                    {
                        onObjectSpawned(go);
                    }

                    TeamToken.seeRecruiter(go, teamToken.owner, true);
                }
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    public delegate void OnObjectSpawned(GameObject go);
    public OnObjectSpawned onObjectSpawned;

}
