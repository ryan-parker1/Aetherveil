using System.Collections;
using FishNet;
using UnityEngine;

/// <summary>
/// Spawns and respawns an enemy at this transform.
/// In multiplayer: only the server/host runs this logic and registers
/// the enemy with FishNet via ServerManager.Spawn().
/// In offline mode: runs normally with regular Instantiate().
/// </summary>
public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private float respawnDelay = 5f;

    private GameObject currentEnemy;

    // -------------------------------------------------------------------------

    private void Start()
    {
        // In multiplayer non-server clients don't spawn enemies —
        // the server's spawns are replicated to them by FishNet.
        if (InstanceFinder.IsClientStarted && !InstanceFinder.IsServerStarted)
            return;

        SpawnEnemy();
    }

    // -------------------------------------------------------------------------

    private void SpawnEnemy()
    {
        currentEnemy = Instantiate(
            enemyPrefab,
            transform.position,
            Quaternion.identity
        );

        // Register with FishNet when a server is running so all clients
        // see the enemy. In offline mode Instantiate() is sufficient.
        if (InstanceFinder.IsServerStarted)
            InstanceFinder.ServerManager.Spawn(currentEnemy);

        EnemyAI enemyAI = currentEnemy.GetComponent<EnemyAI>();
        enemyAI?.SetSpawnPoint(transform);

        Health health = currentEnemy.GetComponent<Health>();
        if (health != null)
            health.OnDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        // Despawn through FishNet so all clients remove the enemy.
        if (InstanceFinder.IsServerStarted)
            InstanceFinder.ServerManager.Despawn(currentEnemy);
        else
            Destroy(currentEnemy);

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnEnemy();
    }
}
