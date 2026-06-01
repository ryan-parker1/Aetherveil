using System.Collections;
using FishNet;
using FishNet.Managing.Server;
using FishNet.Transporting;
using UnityEngine;

/// <summary>
/// Spawns and respawns an enemy at this transform.
///
/// Multiplayer:  waits for the FishNet server to start, then spawns via
///               ServerManager.Spawn() so all clients see the enemy.
/// Offline:      FishNet NetworkManager is present but no server ever starts,
///               so we also handle the case where no server event fires.
///               A one-frame yield in Start lets us detect whether a server
///               is already running (e.g., scene loaded mid-session).
/// </summary>
public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float respawnDelay = 5f;

    private GameObject currentEnemy;
    private bool spawned;

    // -------------------------------------------------------------------------

    private void OnEnable()
    {
        // Subscribe before Start so we don't miss a fast server-start.
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnServerConnectionState +=
                OnServerConnectionState;
    }

    private void OnDisable()
    {
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnServerConnectionState -=
                OnServerConnectionState;
    }

    private void Start()
    {
        // If the server is already running when this object enables
        // (e.g., scene was loaded after Host Game was clicked), spawn now.
        if (InstanceFinder.IsServerStarted && !spawned)
            SpawnEnemy();
    }

    // -------------------------------------------------------------------------

    private void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started && !spawned)
            SpawnEnemy();
    }

    // -------------------------------------------------------------------------

    private void SpawnEnemy()
    {
        spawned = true;

        currentEnemy = Instantiate(
            enemyPrefab,
            transform.position,
            Quaternion.identity
        );

        // Register with FishNet so all clients see the enemy.
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
        if (InstanceFinder.IsServerStarted)
            InstanceFinder.ServerManager.Despawn(currentEnemy);
        else
            Destroy(currentEnemy);

        spawned = false;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnEnemy();
    }
}
