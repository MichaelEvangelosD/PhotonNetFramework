using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager S;
    
    [Header("Set in inspector")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform spawnPoint;

    private void Awake()
    {
        if (S != null)
        {
            Destroy(gameObject);
        }
        else
        {
            S = this;
        }             
    }

    private void Start()
    {
        if(PhotonNetwork.IsConnected 
            && playerPrefab != null)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " Joined " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(PhotonNetwork.NickName + " Entered " + PhotonNetwork.CurrentRoom.Name + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    //Should be in a menu, here for convinience
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " Left " + PhotonNetwork.CurrentRoom.Name);

        SceneManager.LoadScene("LobbyScene");
    }
}
