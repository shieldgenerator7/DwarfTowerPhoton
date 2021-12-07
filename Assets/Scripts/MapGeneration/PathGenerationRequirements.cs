using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PathGenerationRequirements 
{
    [Tooltip("Should all path segments be strictly horizontal or vertical?")]
    public bool forceRectangularPaths;
    public float minLength;
    public float maxLength;
    [Tooltip("Every point must be within these bounds")]
    public Bounds bounds;
    [Tooltip("The path must start at this position")]
    public Vector2 startPos;
    [Tooltip("The path must end at this position")]
    public Vector2 endPos;
    public Vector2 middle;
}
