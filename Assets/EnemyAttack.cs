using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 20f;
    private PlayerHealth playerHealth;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
