using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleInfo
{
    [Range(0, 1000)]
    public int spawnCount = 100;
    public ObjectSpawnInfo spawnInfo;
    [Range(1, 10)]
    public float avoidRadius = 5;
    [Range(1, 10)]
    public float pathAvoidRadius = 5;
}
