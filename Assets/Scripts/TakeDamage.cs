using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TakeDamage : MonoBehaviourPunCallbacks
{
    [Header("Set in inspector")]
    [SerializeField] Image healthBar;
    [SerializeField] float startHealth;

    [Header("Set dynamically")]
    [SerializeField] float currentHealth;

    private void Start()
    {
        currentHealth = startHealth;
        healthBar.fillAmount = currentHealth / startHealth;
    }

    [PunRPC]
    public void DamageInteraction(float _dmg)
    {
        currentHealth -= _dmg;

        healthBar.fillAmount = currentHealth / startHealth;
        if(currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (photonView.IsMine)
        {
            GameManager.S.LeaveRoom();
        }
    }
}
