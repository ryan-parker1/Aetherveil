using UnityEngine;

public class CombatController : MonoBehaviour
{
    private TargetingController targetingController;

    private CombatStats stats;

    private float lastAttackTime;

    private void Awake()
    {
        targetingController =
            GetComponent<TargetingController>();

        stats =
            GetComponent<CombatStats>();
    }

    private void Update()
    {
        HandleAttackInput();
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryAttackTarget();
        }
    }

    private void TryAttackTarget()
    {
        if (Time.time < lastAttackTime + stats.AttackCooldown)
        {
            Debug.Log("Attack on cooldown.");
            return;
        }

        if (targetingController.CurrentTarget == null)
        {
            Debug.Log("No target selected.");
            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                targetingController.CurrentTarget.transform.position
            );

        if (distance > stats.AttackRange)
        {
            Debug.Log("Target out of range.");
            return;
        }

        Damageable damageable =
            targetingController.CurrentTarget
                .GetComponent<Damageable>();

        if (damageable == null)
        {
            return;
        }

        damageable.ReceiveDamage(stats.Damage);

        lastAttackTime = Time.time;
    }
}