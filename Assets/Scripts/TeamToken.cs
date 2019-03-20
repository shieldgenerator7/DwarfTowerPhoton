using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this script at the top level controller object so it knows what team it's on
/// EX: Next to PlayerController or ShotController
/// </summary>
public class TeamToken : MonoBehaviour
{
    public TeamToken teamCaptain;

    private void Start()
    {
        //Make sure all guns register their launched objects
        foreach (GunController gc in GetComponentsInChildren<GunController>())
        {
            gc.onShotFired += recruitShot;
        }
        //Make sure all guns register their launched objects
        foreach (ChargedGunController cgc in GetComponentsInChildren<ChargedGunController>())
        {
            cgc.onShotFired += recruitShot;
        }
    }

    public void recruit(GameObject go)
    {
        TeamToken tt = go.GetComponent<TeamToken>();
        if (!tt)
        {
            tt = go.AddComponent<TeamToken>();
        }
        tt.teamCaptain = this.teamCaptain;
    }

    public bool onSameTeam(TeamToken other)
    {
        return this.teamCaptain == other.teamCaptain;
    }

    public static bool onSameTeam(GameObject go1, GameObject go2)
    {
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
        recruit(shot);
    }
}
