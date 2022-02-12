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
    [Tooltip("This determines which team it is on")]
    public TeamTokenCaptain teamCaptain;
    [Tooltip("This is the player that owns it (created it)")]
    public TeamToken owner;
    [Tooltip("This is the player that currently controls it (decides what it does)")]
    public TeamToken controller;

    public bool HasController => controller && controller != this;

    private PhotonView photonView;
    public PhotonView PV
    {
        get
        {
            if (!photonView)
            {
                photonView = gameObject.FindComponent<PhotonView>();
            }
            return photonView;
        }
    }

    protected virtual void Start()
    {
        //If there's no owner,
        if (!owner)
        {
            //it owns itself
            owner = this;
        }
        //If there's no controller,
        if (!controller)
        {
            //it controls itself
            controller = this;
            onControllerGainedControl?.Invoke(this);
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
        tt.controller = this;
        tt.onControllerGainedControl?.Invoke(this);
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
        TeamToken tt = TeamToken.FindTeamToken(recruiterID);
        if (ownedObject)
        {
            tt?.recruitOwnedObject(this);
        }
        else
        {
            tt?.recruit(this);
        }
    }

    public static void switchController(GameObject go, TeamToken controller)
    {
        TeamToken tt = getTeamToken(go, true);
        tt.switchController(controller);
    }
    public void switchController(TeamToken controller)
    {
        int controllerID = controller?.PV.ViewID ?? this.owner.PV.ViewID;
        if (controller)
        {
            PV.TransferOwnership(controller.PV.Owner);
        }
        else
        {
            PV.TransferOwnership(this.owner.PV.Owner);
        }
        PV.RPC("RPC_SwitchController", RpcTarget.AllBuffered, controllerID);
    }
    [PunRPC]
    void RPC_SwitchController(int controllerID)
    {
        //Old TT is the current controller
        TeamToken oldTT = this.controller;
        //New TT is the given controller,or if it's null,
        //New TT is this teamToken (it controls itself)
        TeamToken newTT = TeamToken.FindTeamToken(controllerID)
            ?? this;
        //If the controller has changed
        if (oldTT != newTT)
        {
            //Switch the controller
            onControllerLostControl?.Invoke(oldTT);
            this.controller = newTT;
            onControllerGainedControl?.Invoke(newTT);
        }
    }
    public delegate void OnControllerChanged(TeamToken controller);
    public event OnControllerChanged onControllerGainedControl;
    public event OnControllerChanged onControllerLostControl;

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
        TeamToken tt1 = go1.FindComponent<TeamToken>();
        //Get go2's team token
        TeamToken tt2 = go2.FindComponent<TeamToken>();
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

    public bool controlledBySamePlayer(TeamToken other)
    {
        return this.controller == other.controller;
    }

    public static bool controlledBySamePlayer(GameObject go1, GameObject go2)
    {//2022-02-10: copied from ownedBySamePlayer()

        //Get go1's team token
        TeamToken tt1 = go1.FindComponent<TeamToken>();
        //Get go2's team token
        TeamToken tt2 = go2.FindComponent<TeamToken>();
        //If both have a team token
        if (tt1 && tt2)
        {
            //easy, just compare their controllers
            return tt1.controlledBySamePlayer(tt2);
        }
        //If neither has a team token
        else if (!tt1 && !tt2)
        {
            //not controlled at all
            return false;
        }
        //If one or either has a team token, but not both
        else
        {
            //one's not controlled at all,
            //therefore not controlled by same player
            return false;
        }
    }

    public static TeamToken getTeamToken(GameObject go, bool addIfNone = false)
    {
        TeamToken tt = go.FindComponent<TeamToken>();
        if (!tt && addIfNone)
        {
            tt = go.AddComponent<TeamToken>();
        }
        return tt;
    }

    public static TeamToken FindTeamToken(int viewID)
        => TeamToken.getTeamToken(PhotonView.Find(viewID).gameObject);

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
        TeamToken ttc = TeamToken.FindTeamToken(captainID);
        ttc?.recruit(this);
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
            if (teamCaptains[ttc] < minTeamMembers)
            {
                minTeamMembers = teamCaptains[ttc];
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
