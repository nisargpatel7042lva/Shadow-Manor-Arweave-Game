using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Player Health: " + health);

        if (health <= 0)
        {
            Debug.Log("Game Over!");
            SceneManager.LoadScene("GameOverScene");  // Change this to your game over scene name
        }
    }
}
