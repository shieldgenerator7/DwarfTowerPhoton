using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_spawn_info", menuName = "ObjectSpawnInfo", order = 0)]
public class ObjectSpawnInfo : ScriptableObject
{
    [Tooltip("The name of the prefab to spawn from the folder")]
    public string objectName;
    [Tooltip("How far away from the given position it should spawn")]
    public float spawnBuffer = 1;
    [Tooltip("Add this vector to the given spawn location. Useful when the object's pivot is not its center")]
    public Vector2 spawnOffset = Vector2.zero;
    [Tooltip("Should the shot be rotated to face its launch direction?")]
    public bool rotateShot = true;
    [Tooltip("Should this object be put on the same team as its spawner? If not, it will be neutral")]
    public bool inheritTeamToken = true;
    [Tooltip("Shoud this object be recolored to matched the spawner's color?")]
    public bool inheritColor = true;
}
