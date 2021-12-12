using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkerManager : MonoBehaviour
{
    public GameObject markerPrefab;

    private Dictionary<int, MapMarker> mapMarkerMap = new Dictionary<int, MapMarker>();

    public MapMarker CreateMapMarker(int id, Vector2 pos, MapMarkerInfo markerInfo, TeamToken placer)
    {
        MapMarker marker = GetOrCreateMapMarker(id);
        marker.Init(markerInfo, placer); //placer.teamCaptain.teamColor, Color.white);
        marker.Mark(pos);
        return marker;
    }
    public MapMarker CreateMapMarker(int id, Transform follow, MapMarkerInfo markerInfo, TeamToken placer)
    {
        MapMarker marker = GetOrCreateMapMarker(id);
        marker.Init(markerInfo, placer); //placer.teamCaptain.teamColor, Color.white);
        marker.Mark(follow);
        return marker;
    }

    private MapMarker GetOrCreateMapMarker(int id)
    {
        if (mapMarkerMap.ContainsKey(id))
        {
            return mapMarkerMap[id];
        }
        else
        {
            GameObject go = Instantiate(
                markerPrefab,
                Vector2.zero,
                Quaternion.identity
                );
            MapMarker marker = go.FindComponent<MapMarker>();
            mapMarkerMap.Add(id, marker);
            return marker;
        }
    }
}
