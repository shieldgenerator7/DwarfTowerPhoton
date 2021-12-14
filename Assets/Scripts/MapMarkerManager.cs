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

    private TeamToken localPlayerTeamToken
    {
        get
        {
            //NO, we CANNOT set this in Start() or Awake()!
            //It takes time for PhotonPlayer.localPlayer to be set
            //(and no, PhotonView.isMine doesn't get set any earlier)
            //2021-12-13: PP.lP and PV.iM are not set before Start()
            if (!_lptt)
            {
                _lptt = PhotonPlayer.localPlayer.myAvatar.FindComponent<TeamToken>();
            }
            return _lptt;
        }
    }
    private TeamToken _lptt;

    public static MapMarker CreateMapMarker(PhotonView placer, Vector2 pos, MapMarkerInfo markerInfo)
    {
        MapMarker marker = null;
        if (instance.CanCreateMapMarker(placer, markerInfo.permission))
        {
            marker = instance.CreateMapMarkerPos(placer, pos, markerInfo);
        }
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
        if (instance.CanCreateMapMarker(placer, markerInfo.permission))
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

    private bool CanCreateMapMarker(PhotonView placerPV, MapMarkerPermission permission)
    {
        TeamToken placer = TeamToken.getTeamToken(placerPV.gameObject);
        switch (permission)
        {
            case MapMarkerPermission.EVERYONE:
                return true;
            case MapMarkerPermission.ALLIES_ONLY:
                return placer.onSameTeam(localPlayerTeamToken);
            case MapMarkerPermission.SELF_ONLY:
                return placer.ownedBySamePlayer(localPlayerTeamToken);
        }
        throw new System.InvalidOperationException($"No such permission handled: {permission}");
    }

    //TODO: Make a version like this for follow objects
    [PunRPC]
    void RPC_CreateMapMarkerPos(int placerId, Vector2 pos, int markerInfoIndex)
    {
        PhotonView placer = PhotonView.Find(placerId);
        MapMarkerInfo markerInfo = knownMarkerInfos[markerInfoIndex];
        if (CanCreateMapMarker(placer, markerInfo.permission))
        {
            CreateMapMarkerPos(
                placer,
                pos,
                markerInfo
                );
        }
    }

    [PunRPC]
    void RPC_DestroyMapMarker(int placerId)
    {
        DestroyMapMarkerNow(
            PhotonView.Find(placerId)
            );
    }
}
