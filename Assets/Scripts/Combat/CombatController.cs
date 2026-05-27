using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int damageAmount = 10;

    private TargetingController targetingController;

    private void Awake()
    {
        targetingController = GetComponent<TargetingController>();
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
        if (targetingController.CurrentTarget == null)
        {
            Debug.Log("No target selected.");
            return;
        }

        Damageable damageable =
            targetingController.CurrentTarget.GetComponent<Damageable>();

        if (damageable == null)
        {
            Debug.Log("Target is not damageable.");
            return;
        }

        damageable.ReceiveDamage(damageAmount);
    }
}