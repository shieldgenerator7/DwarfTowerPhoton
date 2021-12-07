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
    public Bounds bounds { get; set; }
    [Tooltip("The path must start at this position")]
    public Vector2 startPos;
    [Tooltip("The path must end at this position")]
    public Vector2 endPos;
    [Tooltip("The path's midpoint must be at this location")]
    public Vector2 middle;

    public bool validMapPath(MapPath mapPath)
    {
        //TODO: check more conditions for validity
        float length = mapPath.Length;
        return length >= minLength
            && length <= maxLength;
    }
}
