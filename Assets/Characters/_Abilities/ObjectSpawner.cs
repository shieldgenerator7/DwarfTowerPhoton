using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Stores a list of objects to spawn when commanded to
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [Tooltip("The folder to look for objects in (in Assets/Resources/PhotonPrefabs")]
    public string folderName;
    [Tooltip("Info for what objects can be spawned")]
    public List<ObjectSpawnInfo> objectSpawnInfoList;

    private TeamToken teamToken;
    private PhotonView PV;

    private void Start()
    {
        teamToken = TeamToken.getTeamToken(gameObject);
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            //Subfoldername
            if (string.IsNullOrEmpty(folderName))
            {
                string name = transform.parent.gameObject.name;
                name = name.Replace("(Clone)", "").Trim();
                folderName = "Shots/" + name;
            }
        }
    }

    public GameObject spawnObject(int index, Vector2 pos, Vector2 dir)
    {
        if (PV.IsMine)
        {
            ObjectSpawnInfo osi = objectSpawnInfoList[index];
            string pathName = Path.Combine("PhotonPrefabs", folderName, osi.objectName);
            Vector2 position = pos + (dir * osi.spawnBuffer);
            Quaternion rotation = (osi.rotateShot)
                ? Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, dir))
                : Quaternion.Euler(0, 0, 0);
            GameObject go = PhotonNetwork.Instantiate(pathName, position, rotation);
            if (osi.inheritTeamToken)
            {
                TeamToken.seeRecruiter(go, teamToken.owner, true);
            }
            onObjectSpawned?.Invoke(go);
            return go;
        }
        return null;
    }
    public delegate void OnObjectSpawned(GameObject go);
    public event OnObjectSpawned onObjectSpawned;

}
