using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlagController : MonoBehaviour
{
    private HealthPool healthPool;
    private TeamTokenCaptain teamTokenCaptain;

    private List<TeamTokenCaptain> teamCaptains;

    // Start is called before the first frame update
    void Start()
    {
        healthPool = GetComponent<HealthPool>();
        teamTokenCaptain = GetComponent<TeamTokenCaptain>();
        healthPool.onDied += checkGameOver;
        //Team Token Captains
        teamCaptains = FindObjectsOfType<TeamTokenCaptain>().ToList();
    }

    void checkGameOver()
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
