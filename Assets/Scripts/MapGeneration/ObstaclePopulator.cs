using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstaclePopulator : MonoBehaviour
{
    
    public Transform folder;

    public PlayArea playArea;
    public MapPathGenerator mapPathGenerator;

    public List<ObstacleInfo> obstaclesToSpawn;

    public List<Transform> avoidPosList;

    private ObjectSpawner objectSpawner;

    private Vector2 min;
    private Vector2 max;

    // Start is called before the first frame update
    void Start()
    {
        //Object Spawner
        objectSpawner = GetComponent<ObjectSpawner>();
        if (objectSpawner.PV.IsMine)
        {
            //Area
            min = new Vector2(-playArea.width / 2 + 1, -playArea.height / 2 + 1);
            max = new Vector2(playArea.width / 2 - 1, playArea.height / 2 - 1);
            //Populate
            foreach (ObstacleInfo obstacle in obstaclesToSpawn)
            {
                populate(obstacle);
            }
        }
    }

    void populate(ObstacleInfo obstacle)
    {
        for (int i = 0; i < obstacle.spawnCount; i++)
        {
            GameObject tree = objectSpawner.spawnObject(
                obstacle.spawnInfo,
                getRandomPosition(obstacle, mapPathGenerator.mapPath),
                Vector2.up
                );
            tree.transform.parent = folder;
            tree.GetComponentsInChildren<SpriteRenderer>().ToList()
                .ForEach(sr => sr.updateSortingOrder());
        }
    }

    Vector2 getRandomPosition(ObstacleInfo obstacle)
    {
        Vector2 pos = Vector2.zero;
        do
        {
            pos.x = Random.Range(min.x, max.x);
            pos.y = Random.Range(min.y, max.y);
        }
        while (avoidPosList.Any(
            t => Vector2.Distance(t.position, pos) <= obstacle.avoidRadius
            ));
        return pos;
    }

    Vector2 getRandomPosition(ObstacleInfo obstacle, MapPath pathToAvoid)
    {
        Vector2 pos;
        do
        {
            pos = getRandomPosition(obstacle);
        } while (pathToAvoid.distanceFromPath(pos, obstacle.pathAvoidRadius) <= obstacle.pathAvoidRadius);
        return pos;
    }
}
