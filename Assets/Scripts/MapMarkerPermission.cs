using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapMarkerPermission 
{
    /// <summary>
    /// Show this map marker to everyone, regardless of team
    /// </summary>
    EVERYONE,
    /// <summary>
    /// Only show this map marker to the placer and their allies
    /// </summary>
    ALLIES_ONLY,
    /// <summary>
    /// Only show this map marker to the placer
    /// </summary>
    SELF_ONLY,
}
