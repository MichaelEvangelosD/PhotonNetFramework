using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

namespace NetManager
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        #region INSPECTOR_SETUP
        [Header("Connection Status")]
        [SerializeField] TextMeshProUGUI connectionStatusText;

        [Header("Login UI Panel")]
        [SerializeField] GameObject loginUIPanel;
        [SerializeField] TMP_InputField playerNameInput;

        [Header("Game Options UI Panel")]
        [SerializeField] GameObject gameOptionsUIPanel;

        [Header("Create Room UI Panel")]
        [SerializeField] GameObject createRoomUIPanel;
        [SerializeField] TMP_InputField roomNameField;
        [SerializeField] TMP_InputField maxPlayerField;

        [Header("Inside Room UI Panel")]
        [SerializeField] GameObject insideRoomUIPanel;
        [SerializeField] TextMeshProUGUI roomInfoText;
        [SerializeField] GameObject playerListContent;
        [SerializeField] GameObject playerListPrefab;
        [SerializeField] GameObject startGameButton;

        [Header("Room List UI Panel")]
        [SerializeField] GameObject roomListUIPanel;
        [SerializeField] GameObject roomListEntryPrefab;
        [SerializeField] GameObject roomListParentGameobject;

        [Header("Join Randrom room UI Panel")]
        [SerializeField] GameObject joinRandomUIPanel;
        #endregion

        #region Unity_Methods
        private void Awake()
        {
            
        }

        private void Start()
        {
            ActivatePanel(loginUIPanel.name);

            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Update()
        {
            connectionStatusText.SetText("Connection Status: " + PhotonNetwork.NetworkClientState.ToString());
        }
        #endregion

        #region UI_Callbacks
        public void OnLoginButtonPressed()
        {
            string playerName = playerNameInput.text;
            if (ValidateName(playerName))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.LogError("Invalid name");
            }
        }
         
        public void OnJoinRandomRoomButtonPressed()
        {
            ActivatePanel(joinRandomUIPanel.name);

            PhotonNetwork.JoinRandomRoom();
        }
        #endregion

        #region Photon_Callbacks
        public override void OnConnected()
        {
            Debug.Log("Connected to internet");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log(PhotonNetwork.LocalPlayer.NickName + " connected");
            ActivatePanel(gameOptionsUIPanel.name);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Joined room " + PhotonNetwork.CurrentRoom.Name);

            ActivatePanel(insideRoomUIPanel.name);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(message);
            Debug.Log("Return code: " + returnCode);

            CreateNewRoomOnRandomJoinFailed();
        }
        #endregion

        #region Internal_Methods
        bool ValidateName(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        void CreateNewRoomOnRandomJoinFailed()
        {
            string roomName = "Room " + Random.Range(1000, 10000);

            RoomOptions roomOptions = new RoomOptions();

            roomOptions.MaxPlayers = 10;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
        #endregion

        #region Public_Methods
        public void ActivatePanel(string panelToDeactivate)
        {
            loginUIPanel.SetActive(panelToDeactivate.Equals(loginUIPanel.name));
            gameOptionsUIPanel.SetActive(panelToDeactivate.Equals(gameOptionsUIPanel));
            createRoomUIPanel.SetActive(panelToDeactivate.Equals(createRoomUIPanel));
            insideRoomUIPanel.SetActive(panelToDeactivate.Equals(insideRoomUIPanel));
            roomListUIPanel.SetActive(panelToDeactivate.Equals(roomListUIPanel));
            joinRandomUIPanel.SetActive(panelToDeactivate.Equals(joinRandomUIPanel));
        }
        #endregion
    }
}