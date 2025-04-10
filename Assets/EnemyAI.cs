using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;  // For restarting scene
using UnityEngine.UI; // For UI elements

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    public Transform player;
    public float chaseRange = 10f; 
    private bool isChasing = false;

    public float health = 100f;  // Player Health
    public float damagePerSecond = 20f; // Damage per second when enemy is close
    public GameObject gameOverScreen; // Assign in Inspector

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Enemy is not on a valid NavMesh!");
            return;
        }

        GoToNextPoint();
    }

    void Update()
    {
        if (!agent.enabled) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < 1f)
        {
            // Enemy is too close, drain health
            TakeDamage();
        }

        if (distanceToPlayer < chaseRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > chaseRange * 1.2f)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void GoToNextPoint()
    {
        if (waypoints.Length == 0 || !agent.enabled) return;

        agent.isStopped = false;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    void ChasePlayer()
    {
        if (!agent.enabled || !agent.isOnNavMesh) return;
        
        if (agent.hasPath && Vector3.Distance(agent.destination, player.position) < 0.5f) return;

        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void TakeDamage()
    {
        health -= damagePerSecond * Time.deltaTime;
        Debug.Log("Player Health: " + health);

        if (health <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        gameOverScreen.SetActive(true);
        Time.timeScale = 0; // Freeze game
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Unfreeze game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart level
    }
}
    