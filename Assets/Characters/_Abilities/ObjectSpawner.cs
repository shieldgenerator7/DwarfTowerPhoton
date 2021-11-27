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
        PV = gameObject.FindComponent<PhotonView>();
        if (PV.IsMine)
        {
            //Subfoldername
            if (string.IsNullOrEmpty(folderName))
            {
                throw new UnityException("folderName is null! foldername: " + folderName + ", gameObject: " + gameObject.name);
            }
        }
    }

    public T spawnObject<T>(int index, Vector2 pos, Vector2 dir) where T : Component
    {
        return spawnObject(index, pos, dir).FindComponent<T>();
    }

    public GameObject spawnObject(int index, Vector2 pos, Vector2 dir)
    {
        if (PV.IsMine)
        {
            //Make sure dir is a unit vector
            if (!Mathf.Approximately(dir.sqrMagnitude, 1))
            {
                throw new System.ArgumentException(
                    "dir needs to be a unit vector! dir: " + dir + "," +
                    "sqrMagnitude: " + dir.sqrMagnitude
                    );
            }
            //Initialize arguments
            ObjectSpawnInfo osi = objectSpawnInfoList[index];
            string pathName = Path.Combine("PhotonPrefabs", folderName, osi.objectName);
            Vector2 position = pos + (dir * osi.spawnBuffer);
            position += osi.spawnOffset;
            Quaternion rotation = (osi.rotateShot)
                ? Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, dir))
                : Quaternion.Euler(0, 0, 0);
            //Instantiate
            GameObject go = PhotonNetwork.Instantiate(pathName, position, rotation);
            //Team Token
            if (osi.inheritTeamToken)
            {
                TeamToken.seeRecruiter(go, teamToken.owner, true);
            }
            //Delegate
            onObjectSpawned?.Invoke(go, position, dir);
            //Return
            return go;
        }
        return null;
    }
    public delegate void OnObjectSpawned(GameObject go, Vector2 pos, Vector2 dir);
    public event OnObjectSpawned onObjectSpawned;

}
