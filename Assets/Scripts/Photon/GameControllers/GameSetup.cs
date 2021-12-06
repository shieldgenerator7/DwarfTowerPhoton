using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=JjfPaY57dDM


    public Transform spawnPoint;
    public Canvas endScreen;
    public Image winScreen;
    public Image loseScreen;

    private PhotonView PV;
    public static GameSetup instance;

    private void OnEnable()
    {
        if (GameSetup.instance == null)
        {
            GameSetup.instance = this;
        }
        PV = GetComponent<PhotonView>();
    }

    public void DisconnectPlayer()
    {
        Destroy(FindObjectOfType<PhotonRoom>().gameObject);
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        SceneManager.LoadScene(MultiplayerSetting.instance.menuScene);
    }

    public static void showResultsScreen(TeamTokenCaptain winner)
    {
        instance.PV.RPC("RPC_ShowResultScreen", RpcTarget.AllBuffered, winner.PV.ViewID);
    }

    [PunRPC]
    void RPC_ShowResultScreen(int winnerID)
    {
        foreach (TeamTokenCaptain ttc in FindObjectsOfType<TeamTokenCaptain>())
        {
            if (ttc.PV.ViewID == winnerID)
            {
                if (ttc.onSameTeam(TeamToken.getTeamToken(PhotonPlayer.localPlayer.myAvatar)))
                {
                    showResultsScreen(true);
                }
                else
                {
                    showResultsScreen(false);
                }
                break;
            }
        }
    }

    private void showResultsScreen(bool win)
    {
        endScreen.gameObject.SetActive(true);
        winScreen.enabled = win;
        loseScreen.enabled = !win;
    }
}
