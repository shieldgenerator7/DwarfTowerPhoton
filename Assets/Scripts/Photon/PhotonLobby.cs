using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{//2019-03-14: made by following steps from: https://www.youtube.com/watch?v=fbk_SIhbjDc

    public static PhotonLobby lobby;
    public GameObject connectingScreen;
    public GameObject menu;
    public GameObject btnBattle;
    public GameObject btnCancel;

    private void Awake()
    {
        lobby = this;
        UpdateMenuFromConnectedState(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to the Photon master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        UpdateMenuFromConnectedState(true);
    }

    public void OnBattleButtonClicked()
    {
        btnBattle.SetActive(false);
        btnCancel.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a random game but failed. There must be no open games available");
        CreateRoom();
    }
    public void CreateRoom()
    {
        Debug.Log("Trying to create a new Room");
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSetting.instance.maxPlayers };
        PhotonNetwork.CreateRoom($"Room_{PlayerInfo.instance.mapName}", roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed, there must already be a room with the same name");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        btnCancel.SetActive(false);
        btnBattle.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    private void UpdateMenuFromConnectedState(bool connected)
    {
        connectingScreen.SetActive(!connected);
        menu.SetActive(connected);
    }
}
