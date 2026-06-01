using System;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

/// <summary>
/// Tracks and synchronises hit points for any actor (player or enemy).
/// TakeDamage / RestoreFullHealth / SetCurrentHealth must only be called
/// on the server (or in offline / single-player mode where FishNet is not running).
/// The SyncVar ensures every client always sees the correct health value.
/// </summary>
public class Health : NetworkBehaviour
{
    private CombatStats stats;

    // Server sets this; FishNet replicates it to all clients automatically.
    [SyncVar]
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public bool IsDead      => currentHealth <= 0;

    public event Action OnDeath;

    // -------------------------------------------------------------------------

    private void Awake()
    {
        stats = GetComponent<CombatStats>();
        // Initialise locally so the value is valid before OnStartServer fires.
        // For offline play (no FishNet) this is the only initialisation path.
        currentHealth = stats.TotalHealth;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Re-initialise on the server so the SyncVar broadcast starts correct.
        currentHealth = stats.TotalHealth;
    }

    // -------------------------------------------------------------------------
    // All mutating methods guard against non-server calls in multiplayer.
    // In offline mode (InstanceFinder not started) they run unconditionally.

    private bool IsAuthoritative =>
        !InstanceFinder.IsClientStarted && !InstanceFinder.IsServerStarted   // offline
        || IsServerInitialized;                                                // server/host

    public void TakeDamage(int amount)
    {
        if (!IsAuthoritative) return;
        if (IsDead) return;

        currentHealth -= amount;

        Debug.Log(
            $"{gameObject.name} took {amount} damage. Remaining HP: {currentHealth}"
        );

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void RestoreFullHealth()
    {
        if (!IsAuthoritative) return;
        currentHealth = stats.TotalHealth;
    }

    /// <summary>Called by GameSaveManager on load.</summary>
    public void SetCurrentHealth(int amount)
    {
        if (!IsAuthoritative) return;
        currentHealth = Mathf.Clamp(amount, 0, stats.TotalHealth);
    }

    public void IncreaseMaxHealth(int amount)
    {
        if (!IsAuthoritative) return;
        stats.MaxHealth += amount;
        currentHealth    = stats.TotalHealth;
    }

    // -------------------------------------------------------------------------

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");

        if (CompareTag("Enemy"))
        {
            // In multiplayer this runs on the server; FindAnyObjectByType finds
            // the host player's QuestLog only. Phase 7E will broadcast the kill
            // to the correct client via TargetRpc.
            QuestLog questLog = FindAnyObjectByType<QuestLog>();
            questLog?.RegisterKill(
                gameObject.name.Replace("(Clone)", "").Trim()
            );
        }

        OnDeath?.Invoke();
    }
}
