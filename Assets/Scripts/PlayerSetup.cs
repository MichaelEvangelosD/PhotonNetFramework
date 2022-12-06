using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [Header("Set in inspector")]
    [SerializeField] GameObject playerCam;
    [SerializeField] TextMeshProUGUI playerNameText;

    private void Start()
    {
        if (photonView.IsMine)
        {
            transform.GetComponent<PlayerMovement>().enabled = true;
            playerCam.GetComponent<Camera>().enabled = true;
            playerCam.GetComponent<AudioListener>().enabled = true;
        }
        else
        {
            transform.GetComponent<PlayerMovement>().enabled = false;
            playerCam.GetComponent<Camera>().enabled = false;
            playerCam.GetComponent<AudioListener>().enabled = false;
        }

        SetPlayerUI();
    }

    void SetPlayerUI()
    {
        if(playerNameText != null)
        {
            playerNameText.SetText(photonView.Owner.NickName);
        }
    }
}
