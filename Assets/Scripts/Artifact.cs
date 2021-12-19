using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    public StatLayer statLayer;

    private PhotonView PV;
    private TeamTokenCaptain teamCaptain;

    // Start is called before the first frame update
    void Start()
    {
        PV = gameObject.FindComponent<PhotonView>();
    }

    public void Activate(TeamTokenCaptain teamCaptain, bool activate = true)
    {
        if (!teamCaptain)
        {
            teamCaptain = this.teamCaptain;
        }
        List<StatKeeper> statKeepers = FindObjectsOfType<StatKeeper>().ToList()
            .FindAll(sk => TeamToken.getTeamToken(sk.gameObject).onSameTeam(teamCaptain));
        if (activate)
        {
            this.teamCaptain = teamCaptain;
            statKeepers.ForEach(sk => sk.selfStats.addLayer(PV.ViewID, statLayer));
        }
        else
        {
            statKeepers.ForEach(sk => sk.selfStats.removeLayer(PV.ViewID));
            this.teamCaptain = null;
        }
    }
}
