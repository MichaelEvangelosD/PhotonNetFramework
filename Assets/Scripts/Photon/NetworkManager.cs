using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;

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
   

    public InputField maxPlayerInputField;


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



    #region Unity Methods

    // Start is called before the first frame update
    private void Start()
    {
        ActivatePanel(Login_UI_Panel.name);


        PhotonNetwork.AutomaticallySyncScene = true;

    }

    // Update is called once per frame
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
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);


        string roomName = "Room " + Random.Range(1000,10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomName,roomOptions);
    }
    #endregion

    #region Private Methods

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
