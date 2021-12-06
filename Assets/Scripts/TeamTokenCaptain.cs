using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamTokenCaptain : TeamToken
{
    public Color teamColor = Color.white;

    public List<Transform> spawnPoints;

    private int nextSpawn = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        teamCaptain = this;
    }

    public Transform getNextSpawnPoint()
    {
        Transform spawn = spawnPoints[nextSpawn];
        nextSpawn++;
        nextSpawn = nextSpawn % spawnPoints.Count;
        return spawn;
    }
}
