using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] float fireRate = 0.1f;

    float fireTimer;

    private void Update()
    {
        if(fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }

        if (Input.GetButton("Fire1") 
            && fireTimer > fireRate)
        {
            fireTimer = 0;
            RaycastHit hitInfo;
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if(Physics.Raycast(ray,out hitInfo, 100f))
            {
                if(hitInfo.collider.gameObject.CompareTag("Player") 
                    && !hitInfo.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    hitInfo.collider.gameObject.GetComponent<PhotonView>().RPC("DamageInteraction", RpcTarget.AllBuffered, 10f);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 100f);
        }
#endif
    }
}
