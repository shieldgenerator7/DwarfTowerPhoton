using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Unicorn/CarriedShotControllerData", order = 1)]
public class CarriedShotControllerData : ScriptableObject
{
    [Tooltip("The angle between the facing direction and the shot's hold direction")]
    public float positionAngle = 0;//the angle between the facing direction and the shot's hold direction
    [Tooltip("The angle between the hold direction and the weapon's rotation")]
    public float rotationAngle = 0;//the angle between the hold direction and the shot's rotation
    [Tooltip("The distance between the shot and the holder")]
    public float holdBuffer = 1;
    [Tooltip("The percent size of the shot: 1=normal, 0.5=half, 2=double")]
    public float size = 1;
}
