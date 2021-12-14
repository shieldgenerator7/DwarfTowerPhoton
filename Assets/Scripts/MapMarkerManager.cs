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

    private static MapMarkerManager instance;
    private void Awake()
    {
        instance = this;
    }

    public static MapMarker CreateMapMarker(PhotonView placer, Vector2 pos, MapMarkerInfo markerInfo)
    {
        MapMarker marker = null;
            marker = instance.CreateMapMarkerPos(placer, pos, markerInfo);
        instance.PV.RPC(
            "RPC_CreateMapMarkerPos",
            RpcTarget.Others,
            placer.ViewID,
            pos,
            instance.knownMarkerInfos.IndexOf(markerInfo)
            );
        return marker;
    }

    private MapMarker CreateMapMarkerPos(PhotonView placer, Vector2 pos, MapMarkerInfo markerInfo)
    {
        MapMarker marker = GetOrCreateMapMarker(placer.ViewID);
        marker.Init(markerInfo, TeamToken.getTeamToken(placer.gameObject)); //placer.teamCaptain.teamColor, Color.white);
        marker.Mark(pos);
        return marker;
    }

    public static MapMarker CreateMapMarker(PhotonView placer, Transform follow, MapMarkerInfo markerInfo)
    {
        MapMarker marker = null;
        {
            marker = instance.CreateMapMarkerFollow(placer, follow, markerInfo);
        }
        //instance.PV.RPC(
        //    "RPC_CreateMapMarkerPos",
        //    RpcTarget.Others,
        //    placer.ViewID,
        //    pos,
        //    instance.knownMarkerInfos.IndexOf(markerInfo)
        //    );
        return marker;
    }

    private MapMarker CreateMapMarkerFollow(PhotonView placer, Transform follow, MapMarkerInfo markerInfo)
    {
        MapMarker marker = GetOrCreateMapMarker(placer.ViewID);
        marker.Init(markerInfo, TeamToken.getTeamToken(placer.gameObject)); //placer.teamCaptain.teamColor, Color.white);
        marker.Mark(follow);
        return marker;
    }

    public static void DestroyMapMarker(PhotonView placer)
    {
        instance.DestroyMapMarkerNow(placer);
        //RPC
        instance.PV.RPC(
            "RPC_DestroyMapMarker",
            RpcTarget.Others,
            placer.ViewID
            );
    }

    private void DestroyMapMarkerNow(PhotonView placer)
    {
        if (mapMarkerMap.ContainsKey(placer.ViewID))
        {
            //Destroy marker
            Destroy(mapMarkerMap[placer.ViewID].gameObject);
            //Remove from list
            mapMarkerMap.Remove(placer.ViewID);
        }
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

    //TODO: Make a version like this for follow objects
    [PunRPC]
    void RPC_CreateMapMarkerPos(int placerId, Vector2 pos, int markerInfoIndex)
    {
        PhotonView placer = PhotonView.Find(placerId);
        MapMarkerInfo markerInfo = knownMarkerInfos[markerInfoIndex];
            CreateMapMarkerPos(
                placer,
                pos,
                markerInfo
                );
    }

    [PunRPC]
    void RPC_DestroyMapMarker(int placerId)
    {
        DestroyMapMarkerNow(
            PhotonView.Find(placerId)
            );
    }
}
