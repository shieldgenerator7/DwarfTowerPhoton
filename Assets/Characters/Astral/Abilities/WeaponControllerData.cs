﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Astral/WeaponControllerData", order = 1)]
public class WeaponControllerData : ScriptableObject
{
    public float positionAngle = 0;//the angle between the facing direction and the weapon's hold direction
    public float rotationAngle = 0;//the angle between the hold direction and the weapon's rotation
    public float holdBuffer = 1;
}
