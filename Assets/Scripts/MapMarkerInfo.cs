using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapMarkerInfo", menuName = "Map Marker Info", order = 1)]
public class MapMarkerInfo : ScriptableObject
{
    [Tooltip("The icon to display for this marker")]
    public Sprite icon;
    [Tooltip("Can this marker mark a set position on the map?")]
    public bool canMarkPosition = true;
    [Tooltip("Can this marker mark a movable object?")]
    public bool canFollowObject = true;
    [Tooltip("Should this marker show up even when it's in the camera's view?")]
    public bool showWhileInView = false;
    [Tooltip("Who should be able to see this map marker")]
    public MapMarkerPermission permission = MapMarkerPermission.EVERYONE;
}
