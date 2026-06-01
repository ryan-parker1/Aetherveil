using FishNet;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CombatStats))]
public class EnemyAI : NetworkBehaviour
{
    private Transform spawnPoint;

    private bool returningToSpawn;

    private CharacterController controller;

    private CombatStats stats;

    private Damageable playerDamageable;

    private Transform player;

    private Vector3 velocity;

    [SerializeField] private float gravity = -9.81f;

    [SerializeField] private float leashDistance = 12f;

    private float lastAttackTime;

    // -------------------------------------------------------------------------

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        stats      = GetComponent<CombatStats>();
    }

    private void Start()
    {
        TryFindPlayer();
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns true when this instance should run AI logic.
    /// In multiplayer only the server/host drives enemies.
    /// In offline mode (no FishNet) every instance drives itself.
    /// </summary>
    private bool ShouldRunAI =>
        !InstanceFinder.IsClientStarted && !InstanceFinder.IsServerStarted   // offline
        || IsServerInitialized;                                                // server/host

    // -------------------------------------------------------------------------

    private void TryFindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player           = playerObject.transform;
            playerDamageable = playerObject.GetComponent<Damageable>();
        }
    }

    private void Update()
    {
        // Non-server clients skip all AI — the server position syncs via
        // NetworkTransform on the enemy prefab.
        if (!ShouldRunAI) return;

        if (player == null)
        {
            TryFindPlayer();
            ApplyGravity();
            return;
        }

        HandleCombat();
        ApplyGravity();
    }

    // -------------------------------------------------------------------------

    private void HandleCombat()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is NULL.");
            return;
        }

        float distanceFromSpawn =
            Vector3.Distance(transform.position, spawnPoint.position);

        if (distanceFromSpawn > leashDistance)
        {
            Debug.Log("Enemy exceeded leash distance.");
            returningToSpawn = true;
        }

        if (returningToSpawn)
        {
            ReturnToSpawn();

            if (Vector3.Distance(transform.position, spawnPoint.position) < 0.5f)
            {
                returningToSpawn = false;
                GetComponent<Health>()?.RestoreFullHealth();
            }

            return;
        }

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > stats.DetectionRange) return;

        if (distanceToPlayer > stats.AttackRange)
            MoveTowardPlayer();
        else
            AttackPlayer();
    }

    private void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        controller.Move(direction * stats.MoveSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            10f * Time.deltaTime
        );
    }

    private void ReturnToSpawn()
    {
        Vector3 direction = (spawnPoint.position - transform.position).normalized;
        direction.y = 0f;

        controller.Move(direction * stats.MoveSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            10f * Time.deltaTime
        );
    }

    private void AttackPlayer()
    {
        if (Time.time < lastAttackTime + stats.AttackCooldown) return;

        // Server applies damage directly — it has authority over all health.
        playerDamageable?.ReceiveDamage(stats.Damage);

        Debug.Log($"{gameObject.name} attacked player for {stats.Damage} damage.");

        lastAttackTime = Time.time;
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // -------------------------------------------------------------------------

    public void SetSpawnPoint(Transform point)
    {
        spawnPoint = point;
        Debug.Log($"{gameObject.name} spawn point assigned.");
    }
}
