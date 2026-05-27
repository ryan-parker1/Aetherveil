using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CombatStats))]
public class EnemyAI : MonoBehaviour
{
    private CharacterController controller;
    private CombatStats stats;

    private Transform player;

    private Vector3 velocity;

    [SerializeField] private float gravity = -9.81f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<CombatStats>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        HandleMovement();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        float distance =
            Vector3.Distance(transform.position, player.position);

        if (distance > stats.DetectionRange)
            return;

        if (distance <= stats.AttackRange)
        {
            return;
        }

        Vector3 direction =
            (player.position - transform.position).normalized;

        direction.y = 0f;

        controller.Move(
            direction * stats.MoveSpeed * Time.deltaTime
        );

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            10f * Time.deltaTime
        );
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