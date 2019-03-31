using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamTokenCaptain : TeamToken
{
    public GameObject[] spawnPoints;

    private int nextSpawn = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        teamCaptain = this;
    }

    public GameObject getNextSpawnPoint()
    {
        GameObject spawn = spawnPoints[nextSpawn];
        nextSpawn++;
        nextSpawn = nextSpawn % spawnPoints.Length;
        return spawn;
    }
}
