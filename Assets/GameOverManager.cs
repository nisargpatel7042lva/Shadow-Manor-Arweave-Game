using UnityEngine;
using TMPro;  // If using TextMeshPro

public class GameOverManager : MonoBehaviour
{
    public TMP_Text timeText;  // Make sure this is assigned
    private float timeSurvived;

    void Start()
    {
        timeSurvived = 0f;
    }

    void Update()
    {
        // Increase time survived while the game is running
        timeSurvived += Time.deltaTime;
    }

    public void GameOver()
    {
        // Show time survived on Game Over panel
        timeText.text = "Time Survived: " + timeSurvived.ToString("F2") + "s";
        
        // Enable Game Over UI
        gameObject.SetActive(true);
    }
}
