using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this script at the top level controller object so it knows what team it's on
/// EX: Next to PlayerController or ShotController
/// </summary>
public class TeamToken : MonoBehaviour
{
    public TeamTokenCaptain teamCaptain;
    public TeamToken owner;

    private PhotonView photonView;
    private PhotonView PV
    {
        get
        {
            if (!photonView)
            {
                photonView = GetComponent<PhotonView>();
                if (!photonView)
                {
                    photonView = GetComponentInParent<PhotonView>();
                }
            }
            return photonView;
        }
    }

    protected virtual void Start()
    {
        if (PV.IsMine)
        {
            //Make sure all guns register their launched objects
            foreach (GunController gc in GetComponentsInChildren<GunController>())
            {
                gc.onShotFired += recruitShot;
            }
            //Make sure all charged guns register their launched objects
            foreach (ChargedGunController cgc in GetComponentsInChildren<ChargedGunController>())
            {
                cgc.onShotFired += recruitShot;
            }
        }
        //If there's no owner,
        if (!owner)
        {
            //it owns itself
            owner = this;
        }
    }

    private TeamToken recruit(GameObject go)
    {
        TeamToken tt = go.GetComponent<TeamToken>();
        if (!tt)
        {
            tt = go.AddComponent<TeamToken>();
        }
        recruit(tt);
        return tt;
    }

    private void recruit(TeamToken tt)
    {
        tt.teamCaptain = this.teamCaptain;
    }

    private void recruitOwnedObject(GameObject go)
    {
        recruitOwnedObject(getTeamToken(go));
    }
    private void recruitOwnedObject(TeamToken tt)
    {
        recruit(tt);
        tt.owner = this;
    }

    public static void seeRecruiter(GameObject go, TeamToken recruiter, bool ownedObject = false)
    {
        TeamToken tt = getTeamToken(go, true);
        tt.seeRecruiter(recruiter, ownedObject);
    }

    public void seeRecruiter(TeamToken recruiter, bool ownedObject = false)
    {
        int recruiterID = recruiter.PV.ViewID;
        PV.RPC("RPC_SeeRecruiter", RpcTarget.AllBuffered, recruiterID, ownedObject);
    }

    [PunRPC]
    void RPC_SeeRecruiter(int recruiterID, bool ownedObject)
    {
        foreach (TeamToken tt in FindObjectsOfType<TeamToken>())
        {
            if (tt.PV.ViewID == recruiterID)
            {
                if (ownedObject)
                {
                    tt.recruitOwnedObject(this);
                }
                else
                {
                    tt.recruit(this);
                }
                break;
            }
        }
    }

    public bool onSameTeam(TeamToken other)
    {
        return this.teamCaptain == other.teamCaptain;
    }

    public static bool onSameTeam(GameObject go1, GameObject go2)
    {
        TeamToken tt1 = getTeamToken(go1);
        TeamToken tt2 = getTeamToken(go2);
        //If both have a team token
        if (tt1 && tt2)
        {
            //easy, just compare their team captains
            return tt1.onSameTeam(tt2);
        }
        //If neither has a team token
        else if (!tt1 && !tt2)
        {
            //basically on the same team
            //(neutral won't destroy neutral)
            return true;
        }
        //If one or either has a team token, but not both
        else
        {
            //they're basically on opposite teams
            //(players can destroy neutral objects, and vice versa)
            return false;
        }
    }

    private void recruitShot(GameObject shot, Vector2 targetPos, Vector2 targetDir)
    {
        seeRecruiter(shot, this, true);
    }

    public bool ownedBySamePlayer(TeamToken other)
    {
        return this.owner == other.owner;
    }

    public static bool ownedBySamePlayer(GameObject go1, GameObject go2)
    {//2019-03-20: copied from onSameTeam()

        //Get go1's team token
        TeamToken tt1 = go1.GetComponent<TeamToken>();
        if (!tt1)
        {
            tt1 = go1.GetComponentInParent<TeamToken>();
        }
        //Get go2's team token
        TeamToken tt2 = go2.GetComponent<TeamToken>();
        if (!tt2)
        {
            tt2 = go2.GetComponentInParent<TeamToken>();
        }
        //If both have a team token
        if (tt1 && tt2)
        {
            //easy, just compare their owners
            return tt1.ownedBySamePlayer(tt2);
        }
        //If neither has a team token
        else if (!tt1 && !tt2)
        {
            //not owned at all
            return false;
        }
        //If one or either has a team token, but not both
        else
        {
            //one's not owned at all,
            //therefore not owned by same player
            return false;
        }
    }

    public static TeamToken getTeamToken(GameObject go, bool addIfNone = false)
    {
        TeamToken tt = go.GetComponent<TeamToken>();
        if (!tt)
        {
            tt = go.GetComponentInParent<TeamToken>();
        }
        if (!tt && addIfNone)
        {
            tt = go.AddComponent<TeamToken>();
        }
        return tt;
    }

    public void assignTeam()
    {
        PV.RPC("RPC_Server_AssignTeam", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_Server_AssignTeam()
    {
        TeamTokenCaptain teamCaptain = getTeamWithFewestPlayers();
        PV.RPC("RPC_AssignTeam", RpcTarget.AllBuffered, teamCaptain.PV.ViewID);
    }

    [PunRPC]
    void RPC_AssignTeam(int captainID)
    {
        foreach (TeamTokenCaptain ttc in FindObjectsOfType<TeamTokenCaptain>())
        {
            if (ttc.PV.ViewID == captainID)
            {
                ttc.recruit(this);
                break;
            }
        }
    }

    public static TeamTokenCaptain getTeamWithFewestPlayers()
    {
        //Get list of all team captains
        Dictionary<TeamTokenCaptain, int> teamCaptains = new Dictionary<TeamTokenCaptain, int>();
        foreach (TeamTokenCaptain ttc in FindObjectsOfType<TeamTokenCaptain>())
        {
            if (!teamCaptains.ContainsKey(ttc))
            {
                teamCaptains.Add(ttc, 0);
            }
        }
        foreach (TeamToken tt in FindObjectsOfType<TeamToken>())
        {
            if (tt.teamCaptain != null)
            {
                int increment = (tt.isPlayer()) ? 1 : 0;
                teamCaptains[tt.teamCaptain] += increment;
            }
        }
        //Find the team with the lowest number of team members
        int minTeamMembers = int.MaxValue;
        TeamTokenCaptain minTeamCaptain = null;
        foreach (TeamTokenCaptain ttc in teamCaptains.Keys)
        {
            if ((int)teamCaptains[ttc] < minTeamMembers)
            {
                minTeamMembers = (int)teamCaptains[ttc];
                minTeamCaptain = ttc;
            }
        }
        return minTeamCaptain;
    }

    public bool isPlayer()
    {
        return CompareTag("Player");
    }
}
