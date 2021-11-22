using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Spawns its objects as soon as it can and then destroys itself
/// </summary>
public class ObjectAutoSpawner : MonoBehaviour
{
    public string folderName;
    public List<string> objectNames;
    public bool waitForTeamToken = false;

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
