using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkerManager : MonoBehaviour
{
    public GameObject markerPrefab;
    public List<MapMarkerInfo> knownMarkerInfos;//TODO: serialize MapMarkerInfo with Photon so that we don't have to store references to all known marker infos
    public PhotonView PV;

    private Dictionary<int, MapMarker> mapMarkerMap = new Dictionary<int, MapMarker>();

    public MapMarker CreateMapMarker(PhotonView placer, Vector2 pos, MapMarkerInfo markerInfo, bool callRPC = true)
    {
        MapMarker marker = GetOrCreateMapMarker(placer.ViewID);
        marker.Init(markerInfo, TeamToken.getTeamToken(placer.gameObject)); //placer.teamCaptain.teamColor, Color.white);
        marker.Mark(pos);
        if (callRPC)
        {
            PV.RPC(
                "RPC_CreateMapMarkerPos",
                RpcTarget.Others,
                placer,
                pos,
                knownMarkerInfos.IndexOf(markerInfo)
                );
        }
        return marker;
    }
    public MapMarker CreateMapMarker(PhotonView placer, Transform follow, MapMarkerInfo markerInfo)
    {
        MapMarker marker = GetOrCreateMapMarker(placer.ViewID);
        marker.Init(markerInfo, TeamToken.getTeamToken(placer.gameObject)); //placer.teamCaptain.teamColor, Color.white);
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

    [PunRPC]
    void RPC_CreateMapMarkerPos(int placerId, Vector2 pos, int markerInfoIndex)
    {
        CreateMapMarker(
            PhotonView.Find(placerId),
            pos,
            knownMarkerInfos[markerInfoIndex],
            false
            );
    }
}
