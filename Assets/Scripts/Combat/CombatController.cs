using FishNet;
using FishNet.Object;
using UnityEngine;

/// <summary>
/// Handles the local player's attack input and routes damage to the server.
/// Extends NetworkBehaviour so it can send ServerRpcs.
/// IsOwner guard ensures only the owning client processes input.
/// </summary>
public class CombatController : NetworkBehaviour
{
    private TargetingController targetingController;
    private CombatStats         stats;
    private float               lastAttackTime;

    // -------------------------------------------------------------------------

    private void Awake()
    {
        targetingController = GetComponent<TargetingController>();
        stats               = GetComponent<CombatStats>();
    }

    private void Update()
    {
        // In multiplayer only the owning client processes input.
        // In offline mode (no FishNet) IsOwner is false, so we use the
        // fallback: if FishNet is not started, treat as owner.
        bool offline = !InstanceFinder.IsClientStarted && !InstanceFinder.IsServerStarted;
        if (!offline && !IsOwner) return;

        HandleAttackInput();
    }

    // -------------------------------------------------------------------------

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TryAttackTarget();
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

        float distance = Vector3.Distance(
            transform.position,
            targetingController.CurrentTarget.transform.position
        );

        if (distance > stats.AttackRange)
        {
            Debug.Log("Target out of range.");
            return;
        }

        lastAttackTime = Time.time;

        // --- Networked path ---------------------------------------------------
        NetworkObject targetNetObj =
            targetingController.CurrentTarget.GetComponent<NetworkObject>();

        bool offline = !InstanceFinder.IsClientStarted && !InstanceFinder.IsServerStarted;

        if (!offline && targetNetObj != null)
        {
            // Ask the server to apply the damage (server validates range again).
            ServerDealDamage(targetNetObj, stats.TotalDamage);
            return;
        }

        // --- Offline / non-networked fallback ---------------------------------
        Damageable damageable =
            targetingController.CurrentTarget.GetComponent<Damageable>();
        damageable?.ReceiveDamage(stats.TotalDamage);
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// Server validates the attack and applies damage.
    /// RequireOwnership = true (default) means only the owning client can call this.
    /// </summary>
    [ServerRpc]
    private void ServerDealDamage(NetworkObject targetNetObj, int amount)
    {
        if (targetNetObj == null) return;

        // Server-side range check — adds ~50 % tolerance to absorb latency.
        float distance = Vector3.Distance(
            transform.position,
            targetNetObj.transform.position
        );

        if (distance > stats.AttackRange * 1.5f)
        {
            Debug.Log($"Server rejected attack from {OwnerId}: out of range ({distance:F1}).");
            return;
        }

        Health health = targetNetObj.GetComponent<Health>();
        health?.TakeDamage(amount);

        Debug.Log($"Server applied {amount} damage to {targetNetObj.name}.");
    }
}
