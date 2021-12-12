using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkerManager : MonoBehaviour
{
    public string markerPrefabName;

    public MapMarker createMapMarker(Vector2 pos, MapMarkerInfo markerInfo, TeamToken placer)
    {
        GameObject go = PhotonNetwork.Instantiate(
            markerPrefabName,
            pos,
            Quaternion.identity
            );
        MapMarker marker = go.FindComponent<MapMarker>();
        marker.init(markerInfo, placer); //placer.teamCaptain.teamColor, Color.white);
        return marker;
    }
}
