using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status")]
    public Text connectionStatusText;

    [Header("Connecting UI Panel")]
    public GameObject Connect_UI_Panel;

    [Header("Login UI Panel")]
    public InputField playerNameInput;
    public GameObject Login_UI_Panel;

    [Header("Game Options UI Panel")]
    public GameObject GameOptions_UI_Panel;

    [Header("Create Room UI Panel")]
    public GameObject CreateRoom_UI_Panel;
    public InputField roomNameInputField;
    public Text maxPlayerText;
    public Slider maxPlayerSlider;

    [Header("Inside Room UI Panel")]
    public GameObject InsideRoom_UI_Panel;
    public Text roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListContent;
    public GameObject startGameButton;

    [Header("Room List UI Panel")]
    public GameObject RoomList_UI_Panel;
    public GameObject roomListEntryPrefab;
    public GameObject roomListParentGameobject;

    [Header("Join Random Room UI Panel")]
    public GameObject JoinRandomRoom_UI_Panel;

    Dictionary<string, RoomInfo> cachedRoomPairs = new Dictionary<string, RoomInfo>();
    Dictionary<int, GameObject> roomListGameObjects = new Dictionary<int, GameObject>();
    Dictionary<string, GameObject> playerListGameObjects = new Dictionary<string, GameObject>();

    #region Unity Methods
    private void Start()
    {
        ActivatePanel(Login_UI_Panel.name);

        PhotonNetwork.AutomaticallySyncScene = true;

        maxPlayerSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void Update()
    {
        connectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
    }

    #endregion

    #region UI Callbacks
    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        ActivatePanel(Connect_UI_Panel.name);
        if (!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Playername is invalid!");
        }
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        ActivatePanel(JoinRandomRoom_UI_Panel.name);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnRoomCreateButtonClicked()
    {
        string roomName = roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(maxPlayerSlider.value.ToString());

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivatePanel(GameOptions_UI_Panel.name);
    }

    public void OnShowRoomsListButtomClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(RoomList_UI_Panel.name);
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon");
        ActivatePanel(GameOptions_UI_Panel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name+ " is created.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " joined to "+ PhotonNetwork.CurrentRoom.Name );
        ActivatePanel(InsideRoom_UI_Panel.name);

        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        UpdateRoomInfoText();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameObject = Instantiate(playerListPrefab);
            playerListGameObject.transform.SetParent(playerListContent.transform);
            playerListGameObject.transform.localScale = Vector3.one;

            playerListGameObject.GetComponentInChildren<Text>().text = player.NickName;
            if(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListGameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                playerListGameObject.transform.GetChild(1).gameObject.SetActive(false);
            }

            roomListGameObjects.Add(player.ActorNumber, playerListGameObject);
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        string roomName = "Room " + Random.Range(1000,10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomName,roomOptions);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomInfoText();

        GameObject playerListGameObject = Instantiate(playerListPrefab);
        playerListGameObject.transform.SetParent(playerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;

        playerListGameObject.GetComponentInChildren<Text>().text = newPlayer.NickName;
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListGameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            playerListGameObject.transform.GetChild(1).gameObject.SetActive(false);
        }

        roomListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptions_UI_Panel.name);
        foreach (GameObject playerListing in roomListGameObjects.Values)
        {
            Destroy(playerListing);
        }

        roomListGameObjects = null;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInfoText();

        Destroy(roomListGameObjects[otherPlayer.ActorNumber].gameObject);
        roomListGameObjects.Remove(otherPlayer.ActorNumber);

        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        foreach (RoomInfo roomInfo in roomList)
        {
            if(!roomInfo.IsOpen 
                ||!roomInfo.IsVisible
                || roomInfo.RemovedFromList)
            {
                if (cachedRoomPairs.ContainsKey(roomInfo.Name))
                {
                    cachedRoomPairs.Remove(roomInfo.Name);
                }
            }
            else
            {
                if (cachedRoomPairs.ContainsKey(roomInfo.Name))
                {
                    cachedRoomPairs[roomInfo.Name] = roomInfo;
                }
                else
                {
                    cachedRoomPairs.Add(roomInfo.Name, roomInfo);
                }
            }
        }

        foreach (RoomInfo roomInfo in cachedRoomPairs.Values)
        {
            GameObject roomEntry = Instantiate(roomListEntryPrefab);
            roomEntry.transform.SetParent(roomListParentGameobject.transform);
            roomEntry.transform.localScale = Vector3.one;

            roomEntry.transform.Find("RoomNameText").GetComponent<Text>().text = roomInfo.Name;
            roomEntry.transform.Find("RoomPlayersText").GetComponent<Text>().text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;

            roomEntry.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(roomInfo.Name));

            playerListGameObjects.Add(roomInfo.Name, roomEntry);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomListView();
    }
    #endregion

    #region Private Methods
    void OnSliderValueChanged(float value)
    {
        maxPlayerText.text = "Max Players: " + value.ToString();
    }

    void UpdateRoomInfoText()
    {
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name +
                            " Player/Max Players: " + PhotonNetwork.CurrentRoom.PlayerCount +
                            " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    void OnJoinRoomButtonClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    void ClearRoomListView()
    {
        foreach (GameObject roomList in playerListGameObjects.Values)
        {
            Destroy(roomList.gameObject);
        }

        playerListGameObjects = new Dictionary<string, GameObject>();
        cachedRoomPairs = new Dictionary<string, RoomInfo>();
    }
    #endregion

    #region Public Methods
    public void ActivatePanel(string panelToBeActivated)
    {
        Login_UI_Panel.SetActive(panelToBeActivated.Equals(Login_UI_Panel.name));
        Connect_UI_Panel.SetActive(panelToBeActivated.Equals(Connect_UI_Panel.name));
        GameOptions_UI_Panel.SetActive(panelToBeActivated.Equals(GameOptions_UI_Panel.name));
        CreateRoom_UI_Panel.SetActive(panelToBeActivated.Equals(CreateRoom_UI_Panel.name));
        InsideRoom_UI_Panel.SetActive(panelToBeActivated.Equals(InsideRoom_UI_Panel.name));
        RoomList_UI_Panel.SetActive(panelToBeActivated.Equals(RoomList_UI_Panel.name));
        JoinRandomRoom_UI_Panel.SetActive(panelToBeActivated.Equals(JoinRandomRoom_UI_Panel.name));
    }
    #endregion
}
