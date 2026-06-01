using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CombatStats))]
public class EnemyAI : MonoBehaviour
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

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        stats = GetComponent<CombatStats>();
    }

    private void Start()
    {
        TryFindPlayer();
    }

    private void TryFindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerDamageable =
                playerObject.GetComponent<Damageable>();
        }
    }

    private void Update()
    {
        // Keep trying to find the player until FishNet spawns it
        if (player == null)
        {
            TryFindPlayer();
            ApplyGravity();
            return;
        }

        HandleCombat();
        ApplyGravity();
    }

    private void HandleCombat()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is NULL.");
            return;
        }

        float distanceFromSpawn =
            Vector3.Distance(
                transform.position,
                spawnPoint.position
            );

        // Debug.Log("Distance From Spawn: " + distanceFromSpawn);

        if (distanceFromSpawn > leashDistance)
        {
            Debug.Log("Enemy exceeded leash distance.");

            returningToSpawn = true;
        }

        if (returningToSpawn)
        {
            ReturnToSpawn();

            float distanceToSpawn =
                Vector3.Distance(
                    transform.position,
                    spawnPoint.position
                );

            if (distanceToSpawn < 0.5f)
            {
                returningToSpawn = false;

                Health health =
                    GetComponent<Health>();

                if (health != null)
                {
                    health.RestoreFullHealth();
                }
            }

            return;
        }

        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distanceToPlayer > stats.DetectionRange)
        {
            return;
        }

        if (distanceToPlayer > stats.AttackRange)
        {
            MoveTowardPlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    private void MoveTowardPlayer()
    {
        Vector3 direction =
            (player.position - transform.position).normalized;

        direction.y = 0f;

        controller.Move(
            direction *
            stats.MoveSpeed *
            Time.deltaTime
        );

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            10f * Time.deltaTime
        );
    }

    private void ReturnToSpawn()
    {

        Vector3 direction =
            (spawnPoint.position - transform.position).normalized;

        direction.y = 0f;

        controller.Move(
            direction *
            stats.MoveSpeed *
            Time.deltaTime
        );

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            10f * Time.deltaTime
        );
    }

    private void AttackPlayer()
    {
        if (Time.time < lastAttackTime + stats.AttackCooldown)
        {
            return;
        }

        if (playerDamageable != null)
        {
            playerDamageable.ReceiveDamage(stats.Damage);

            Debug.Log(gameObject.name + " attacked player.");
        }

        lastAttackTime = Time.time;
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
    public void SetSpawnPoint(Transform point)
    {
        spawnPoint = point;

        Debug.Log(gameObject.name + " spawn point assigned.");
    }
}