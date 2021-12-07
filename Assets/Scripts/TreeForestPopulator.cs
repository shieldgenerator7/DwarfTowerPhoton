using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeForestPopulator : MonoBehaviour
{
    [Range(0, 1000)]
    public int treeCount = 100;
    public int treeIndex = 0;
    public Transform folder;

    public PlayArea playArea;
    public MapPathGenerator mapPathGenerator;

    public List<Transform> avoidPosList;
    [Range(1, 10)]
    public float avoidRadius = 5;
    [Range(1, 10)]
    public float pathAvoidRadius = 5;

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
            populate();
        }
    }

    void populate()
    {
        for (int i = 0; i < treeCount; i++)
        {
            GameObject tree = objectSpawner.spawnObject(
                treeIndex,
                getRandomPosition(mapPathGenerator.mapPath),
                Vector2.up
                );
            tree.transform.parent = folder;
            tree.GetComponentsInChildren<SpriteRenderer>().ToList()
                .ForEach(sr => sr.updateSortingOrder());
        }
    }

    Vector2 getRandomPosition()
    {
        Vector2 pos = Vector2.zero;
        do
        {
            pos.x = Random.Range(min.x, max.x);
            pos.y = Random.Range(min.y, max.y);
        }
        while (avoidPosList.Any(
            t => Vector2.Distance(t.position, pos) <= avoidRadius
            ));
        return pos;
    }

    Vector2 getRandomPosition(MapPath pathToAvoid)
    {
        Vector2 pos = Vector2.zero;
        do
        {
            pos = getRandomPosition();
        } while (pathToAvoid.distanceFromPath(pos, pathAvoidRadius) <= pathAvoidRadius);
        return pos;
    }
}
