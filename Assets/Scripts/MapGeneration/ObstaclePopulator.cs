using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstaclePopulator : MonoBehaviour
{

    public Transform folder;

    public MapPathGenerator mapPathGenerator;

    public List<Transform> avoidPosList;

    public ObjectSpawner objectSpawner;

    public PhotonView PV;

    private Vector2 min;
    private Vector2 max;

    private MapProfile mapProfile;

    public void populateObstacles(MapProfile mapProfile)
    {
        this.mapProfile = mapProfile;
        min = mapProfile.GeneratableBounds.min;
        max = mapProfile.GeneratableBounds.max;
        //ObjectSpawner
        objectSpawner.SpawnMaster = true;
        //Populate
        foreach (ObstacleInfo obstacle in mapProfile.obstacleList)
        {
            populate(obstacle);
        }
    }

    void populate(ObstacleInfo obstacle)
    {
        for (int i = 0; i < obstacle.spawnCount; i++)
        {
            GameObject obst = objectSpawner.spawnObject(
                obstacle.spawnInfo,
                getRandomPosition(obstacle, mapPathGenerator.mapPath),
                Vector2.up
                );
            if (obstacle.visualEffects)
            {
                PV.RPC(
                    "RPC_AddVisualEffect",
                    RpcTarget.All,
                    PhotonView.Get(obst).ViewID,
                    mapProfile.obstacleList.IndexOf(obstacle)
                    );
            }
            obst.transform.parent = folder;
            obst.GetComponentsInChildren<SpriteRenderer>().ToList()
                .FindAll(sr => sr.sortingLayerName == "Default")
                .ForEach(sr => sr.updateSortingOrder());
        }
    }

    Vector2 getRandomPosition(ObstacleInfo obstacle)
    {
        int safetyEject = 100;
        Vector2 pos = Vector2.zero;
        do
        {
            pos.x = Random.Range(min.x, max.x);
            pos.y = Random.Range(min.y, max.y);
            safetyEject--;
            if (safetyEject == 0)
            {
                Debug.Log($"Safety eject! obstacle: {obstacle}");
                break;
            }
        }
        while (avoidPosList.Any(
            t => Vector2.Distance(t.position, pos) <= obstacle.avoidRadius
            ));
        return pos;
    }

    Vector2 getRandomPosition(ObstacleInfo obstacle, MapPath pathToAvoid)
    {
        int safetyEject = 100;
        Vector2 pos;
        do
        {
            pos = getRandomPosition(obstacle);
            safetyEject--;
            if (safetyEject == 0)
            {
                Debug.Log($"Safety eject! obstacle: {obstacle}");
                break;
            }
        }
        while (pathToAvoid.distanceFromPath(pos, obstacle.pathAvoidRadius) <= obstacle.pathAvoidRadius);
        return pos;
    }

    [PunRPC]
    void RPC_AddVisualEffect(int viewId, int obstacleIndex)
    {
        GameObject obstacle = PhotonView.Find(viewId).gameObject;
        GameObject visualEffect = Instantiate(
            mapProfile.obstacleList[obstacleIndex].visualEffects
            );
        visualEffect.transform.parent = obstacle.transform;
        visualEffect.transform.localPosition = Vector2.zero;
    }
}
