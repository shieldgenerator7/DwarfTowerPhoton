using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeForestPopulator : MonoBehaviour
{
    [Range(0, 1000)]
    public int treeCount = 100;
    public int treeIndex = 0;
    public Transform folder;

    public PlayArea playArea;

    private ObjectSpawner objectSpawner;
    // Start is called before the first frame update
    void Start()
    {
        objectSpawner = GetComponent<ObjectSpawner>();
        populate();
    }

    void populate()
    {
        Vector2 min = new Vector2(-playArea.width / 2, -playArea.height / 2);
        Vector2 max = new Vector2(playArea.width / 2, playArea.height / 2);
        for (int i = 0; i < treeCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y)
                );
            GameObject tree = objectSpawner.spawnObject(treeIndex, pos, Vector2.up);
            tree.transform.parent = folder;
        }
    }
}
