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

    private Vector2 min;
    private Vector2 max;

    public void populateObstacles(MapProfile mapProfile)
    {
        min = mapProfile.GeneratableBounds.min;
        max = mapProfile.GeneratableBounds.max;
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
            obst.transform.parent = folder;
            obst.GetComponentsInChildren<SpriteRenderer>().ToList()
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
}
