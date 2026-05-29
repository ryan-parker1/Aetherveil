using System.Collections;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private float respawnDelay = 5f;

    private GameObject currentEnemy;

    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        currentEnemy =
            Instantiate(
                enemyPrefab,
                transform.position,
                Quaternion.identity
            );
        
        EnemyAI enemyAI =
            currentEnemy.GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.SetSpawnPoint(transform);
        }

        Health health =
            currentEnemy.GetComponent<Health>();

        if (health != null)
        {
            health.OnDeath += HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath()
    {
        Destroy(currentEnemy);

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        SpawnEnemy();
    }
}