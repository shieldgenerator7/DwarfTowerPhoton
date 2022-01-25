using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HoldShotData
{
    [Tooltip("The distance between the shot and the holder")]
    public float holdBuffer;
    [Tooltip("The angle between the facing direction and the shot's hold direction")]
    public float holdAngle;//the angle between the facing direction and the shot's hold direction
    [Tooltip("The angle between the hold direction and the weapon's rotation")]
    public float rotationAngle;//the angle between the hold direction and the shot's rotation
    [Tooltip("The percent size of the shot: 1=normal, 0.5=half, 2=double")]
    public float size;

    public HoldShotData(float holdBuffer = 1, float holdAngle = 0, float rotationAngle = 0, float size = 1)
    {
        this.holdBuffer = holdBuffer;
        this.holdAngle = holdAngle;
        this.rotationAngle = rotationAngle;
        this.size = size;
    }
}
