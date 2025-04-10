using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    public Transform player; // Assign the Player in Inspector
    private NavMeshAgent agent;

    // void Start()
    // {
    //     agent = GetComponent<NavMeshAgent>();

    //     // Ensure enemy is on NavMesh, otherwise reposition
    //     NavMeshHit hit;
    //     // if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
    //     // {
    //     //     transform.position = hit.position;
    //     //     Debug.Log("Enemy repositioned onto NavMesh.");
    //     // }
    //     // else
    //     // {
    //     //     Debug.LogError("Enemy is outside the NavMesh! Please reposition it manually.");
    //     // }
    // }

    // void Update()
    // {
    //     if (player != null)
    //     {
    //         // Check if player is on NavMesh
    //         if (!IsOnNavMesh(player.position))
    //         {
    //             Debug.LogError("Player is outside the NavMesh! Fix their position.");
    //             return;
    //         }

    //         // Set destination to player
    //         agent.SetDestination(player.position);

    //         // Check if path is valid
    //         if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
    //         {
    //             Debug.LogError("No valid path found to Player!");
    //         }
    //         else if (agent.pathStatus == NavMeshPathStatus.PathPartial)
    //         {
    //             Debug.LogWarning("Path to Player is incomplete, check obstacles.");
    //         }
    //     }
    // }

    // ✅ Function to check if a point is on the NavMesh
    bool IsOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas);
    }
}
 