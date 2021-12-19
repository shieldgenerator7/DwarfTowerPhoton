using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FlagController : MonoBehaviour
{
    public MapMarkerInfo flagMarkerInfo;
    public GameObject flagMarkerBalloonPrefab;

    public List<Pedestal> pedestalList;

    private HealthPool healthPool;
    private TeamTokenCaptain teamTokenCaptain;

    private List<TeamTokenCaptain> teamCaptains;

    private TMP_Text txtMapName;

    // Start is called before the first frame update
    void Start()
    {
        healthPool = GetComponent<HealthPool>();
        teamTokenCaptain = GetComponent<TeamTokenCaptain>();
        healthPool.onDied += checkGameOver;
        //Team Token Captains
        teamCaptains = FindObjectsOfType<TeamTokenCaptain>().ToList();
        //Text Map Name
        txtMapName = gameObject.FindComponent<TMP_Text>();
        MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
        txtMapName.text = mapGenerator.mapName;
        mapGenerator.onMapNameChanged += (mapName) => txtMapName.text = mapName;
        //Marker
        MapMarker mapMarker = MapMarkerManager.CreateMapMarker(
            PhotonView.Get(gameObject),
            transform.position,
            flagMarkerInfo
            );
        GameObject balloons = Instantiate(flagMarkerBalloonPrefab);
        balloons.transform.parent = mapMarker.iconSR.transform;
        balloons.transform.localPosition = Vector2.zero;
        balloons.GetComponent<FlagMapMarkerDisplay>().Init(this);
    }

    void checkGameOver(float hp)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //TODO: store teamCaptains somewhere more universal,
            //so that this works even when there's more than 2 teams
            //(even tho it's unlikely there will ever be more than 2 teams)
            if (teamCaptains.Contains(teamTokenCaptain))
            {
                teamCaptains.Remove(teamTokenCaptain);
            }
            if (teamCaptains.Count == 1)
            {
                foreach (TeamTokenCaptain ttcWin in teamCaptains)
                {
                    GameSetup.showResultsScreen(ttcWin);
                    break;//only let one team win
                }
            }
        }
    }
}
