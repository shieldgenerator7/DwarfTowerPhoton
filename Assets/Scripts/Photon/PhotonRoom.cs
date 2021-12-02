using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoom : MonoBehaviourPunCallbacks
{//2019-03-14: made by following this tutorial: https://www.youtube.com/watch?v=IsiWRD1Xh5g

    //Room Info
    public static PhotonRoom instance;
    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;

    //Player Info
    Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    [SerializeField]
    private int _playersInGame;
    public int playersInGame
    {
        get => _playersInGame;
        set
        {
            _playersInGame = value;
            PhotonNetwork.CurrentRoom.IsOpen = _playersInGame < MultiplayerSetting.instance.maxPlayers;
        }
    }

    public bool showTimeContinually = false;

    //Delayed Start
    private bool readyToCount;
    private bool readyToStart;
    private float startingTime = 10;
    private float lessThanMaxPlayers;
    private float atMaxPlayers;
    private float timeToStart = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(instance.gameObject);
                instance = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);

        PV = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayers = 6;
        timeToStart = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (showTimeContinually)
        {
            if (Mathf.Approximately(Time.time, Mathf.Floor(Time.time)))
            {
                Debug.Log($"Update: time: {Time.time}");
            }
        }
        if (MultiplayerSetting.instance.delayStart)
        {
            if (playersInRoom == 1)
            {
                RestartTimer();
            }
            if (!isGameLoaded)
            {
                if (readyToStart)
                {
                    atMaxPlayers -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayers;
                    timeToStart = atMaxPlayers;
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                Debug.Log($"Display time to start to the players {timeToStart}");
                if (readyToStart || timeToStart <= 0)
                {
                    Debug.Log($"Update, calling StartGame: ready?: {readyToStart}, timeToStart: {timeToStart}");
                    StartGame();
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are now in a room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = $"{myNumberInRoom}";
        if (MultiplayerSetting.instance.delayStart)
        {
            Debug.Log($"(Joined) Players in room out of max players possible ({playersInRoom}:{MultiplayerSetting.instance.maxPlayers})");
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSetting.instance.maxPlayers)
            {
                Debug.Log("(JoinedRoom) Ready to start!");
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                {
                    return;
                }
            }
        }
        else
        {
            Debug.Log($"No delay, so starting. delay?: {MultiplayerSetting.instance.delayStart}");
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has joined the room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        if (MultiplayerSetting.instance.delayStart)
        {
            Debug.Log($"(PlayerEntered) Players in room out of max players possible ({playersInRoom}:{MultiplayerSetting.instance.maxPlayers})");
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom >= MultiplayerSetting.instance.maxPlayers)
            {
                Debug.Log("(PlayerEntered) Ready to start!");
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                {
                    return;
                }
            }
        }
    }

    void StartGame()
    {
        Debug.Log("Start Game");
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PhotonNetwork.LoadLevel(MultiplayerSetting.instance.multiplayerScene);
    }

    void RestartTimer()
    {
        Debug.Log("Restart Timer");
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayers = 6;
        readyToCount = false;
        readyToStart = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"OnSceneFinishedLoading: scene: {scene.name}, mode: {mode}");
        currentScene = scene.buildIndex;
        if (currentScene == MultiplayerSetting.instance.menuScene)
        {
            if (MultiplayerSetting.instance.delayStart)
            {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else
            {
                RPC_CreatePlayer();
            }
        }
        else
        {
            RPC_CreatePlayer();
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene()//"RPC_" not necessary in front of method name, it's only for style
    {
        Debug.Log("RPC_LoadedGameScene()");
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        Debug.Log("Create Player");
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " has left the game");
        playersInGame--;
    }
}
