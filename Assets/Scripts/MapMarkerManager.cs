using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkerManager : MonoBehaviour
{
    public string markerPrefabName;

    public MapMarker CreateMapMarker(Vector2 pos, MapMarkerInfo markerInfo, TeamToken placer)
    {
        GameObject go = PhotonNetwork.Instantiate(
            markerPrefabName,
            pos,
            Quaternion.identity
            );
        MapMarker marker = go.FindComponent<MapMarker>();
        marker.Init(markerInfo, placer); //placer.teamCaptain.teamColor, Color.white);
        marker.Mark(pos);
        return marker;
    }
    public MapMarker CreateMapMarker(Transform follow, MapMarkerInfo markerInfo, TeamToken placer)
    {
        GameObject go = PhotonNetwork.Instantiate(
            markerPrefabName,
            Vector2.zero,
            Quaternion.identity
            );
        MapMarker marker = go.FindComponent<MapMarker>();
        marker.Init(markerInfo, placer); //placer.teamCaptain.teamColor, Color.white);
        marker.Mark(follow);
        return marker;
    }
}
