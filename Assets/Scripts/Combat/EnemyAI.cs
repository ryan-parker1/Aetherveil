using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CombatStats))]
public class EnemyAI : MonoBehaviour
{
    private CharacterController controller;

    private CombatStats stats;

    private Damageable playerDamageable;

    private Transform player;

    private Vector3 velocity;

    [SerializeField] private float gravity = -9.81f;

    private float lastAttackTime;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        stats = GetComponent<CombatStats>();
    }

    private void Start()
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
        if (player == null)
            return;

        HandleCombat();

        ApplyGravity();
    }

    private void HandleCombat()
    {
        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distance > stats.DetectionRange)
        {
            return;
        }

        if (distance > stats.AttackRange)
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
}