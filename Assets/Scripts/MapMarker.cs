using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarker : MonoBehaviour
{
    private Vector2 targetPosition;
    private Transform followObj;
    private MapMarkerInfo info;
    private TeamToken placer;
    //[Tooltip("Should this marker show up on the map even when its position is in the camera view?")]
    //public bool showWhenOnScreen = false;

    [Header("Components")]
    public SpriteRenderer iconSR;
    public SpriteRenderer markerSR;

    public void Init(MapMarkerInfo info, TeamToken placer)
    {
        init(info, placer, placer.teamCaptain.teamColor, placer.teamCaptain.teamColor);
    }

    public void init(MapMarkerInfo info, Color iconColor, Color markerColor)
    {
        init(info, null, iconColor, markerColor);
    }

    public void init(MapMarkerInfo info, TeamToken placer, Color iconColor, Color markerColor)
    {
        this.info = info;
        this.placer = placer;
        iconSR.sprite = info.icon;
        iconSR.color = iconColor;
        markerSR.color = markerColor;
    }

    public void Mark(Vector2 pos)
    {
        if (info.canMarkPosition)
        {
            targetPosition = pos;
        }
        else
        {
            Debug.LogError($"This map marker {info.name} should not be able to mark position!");
        }
    }

    public void mark(Transform followObject)
    {
        if (info.canFollowObject)
        {
            targetPosition = followObj.transform.position;
        }
        else
        {
            Debug.LogError($"This map marker {info.name} should not be able to follow an object!");
        }
    }

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (followObj)
        {
            targetPosition = followObj.position;
        }
        updatePosition();
    }

    void updatePosition()
    {
        //Position
        transform.position = targetPosition;
        //Rotation
        transform.up = ((Vector2)cam.transform.position - targetPosition);
        //Make sure icon is always the right way up
        iconSR.transform.up = Vector2.up;
    }


}
