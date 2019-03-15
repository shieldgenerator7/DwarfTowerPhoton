using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class photonConnect : MonoBehaviour
{//2019-03-14: made by following steps from https://www.youtube.com/watch?v=evrth262vfs
    public GameObject pnlConnecting, pnlConnected, pnlDisconnected;

    private void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void connectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings();

        Debug.Log("Connecting to photon...");
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);

        Debug.Log("We are connected to Master");
    }

    private void OnJoinedLobby()
    {
        pnlConnecting.SetActive(false);
        pnlConnected.SetActive(true);
        Debug.Log("On Joined Lobby");
    }

    private void OnDisconnectedFromPhoton()
    {
        pnlConnected.SetActive(false);
        pnlConnecting.SetActive(false);
        pnlDisconnected.SetActive(true);
        Debug.Log("Disconnected from photon services");
    }
}
