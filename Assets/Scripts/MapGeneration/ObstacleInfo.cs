using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleInfo", menuName = "Map Generation/Obstacle", order = 1)]
public class ObstacleInfo : ScriptableObject
{
    [Range(0, 1000)]
    public int spawnCount = 100;
    public ObjectSpawnInfo spawnInfo;
    [Range(1, 10)]
    public float avoidRadius = 5;
    [Range(1, 10)]
    public float pathAvoidRadius = 5;
}
