using System;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

/// <summary>
/// Tracks and synchronises hit points for any actor (player or enemy).
///
/// Network mode  : SyncVar<int> replicates currentHealth from server to all clients.
///                 Mutation methods are safe to call from server code (EnemyAI,
///                 CombatController ServerRpc) without additional guards — those
///                 callers are already gated to run server-only.
/// Offline mode  : Plain int is used directly; SyncVar is never touched.
/// </summary>
public class Health : NetworkBehaviour
{
    private CombatStats stats;

    // ----------- local authoritative value (always valid) --------------------
    private int _health;

    // ----------- network sync: server writes, clients read ------------------
    private readonly SyncVar<int> _syncedHealth = new SyncVar<int>();

    // -------------------------------------------------------------------------
    public int  CurrentHealth => IsSpawned ? _syncedHealth.Value : _health;
    public bool IsDead        => CurrentHealth <= 0;

    public event Action OnDeath;

    // -------------------------------------------------------------------------

    private void Awake()
    {
        stats   = GetComponent<CombatStats>();
        _health = stats.TotalHealth;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Authoritative server initialises both values.
        _health = stats.TotalHealth;
        _syncedHealth.Value = _health;
    }

    // -------------------------------------------------------------------------

    public void TakeDamage(int amount)
    {
        // Guard: in multiplayer only server code should reach here; in offline
        // the plain int path is always valid. No NetworkBehaviour guard needed
        // because EnemyAI and CombatController already run server-only.
        if (IsDead) return;

        _health -= amount;

        if (IsSpawned)
            _syncedHealth.Value = _health;

        Debug.Log(
            $"{gameObject.name} took {amount} damage. Remaining HP: {CurrentHealth}"
        );

        if (_health <= 0)
        {
            _health = 0;
            if (IsSpawned) _syncedHealth.Value = 0;
            Die();
        }
    }

    public void RestoreFullHealth()
    {
        _health = stats.TotalHealth;
        if (IsSpawned) _syncedHealth.Value = _health;
    }

    /// <summary>Called by GameSaveManager on load.</summary>
    public void SetCurrentHealth(int amount)
    {
        // Clamp to minimum 1 so a player saved at 0 HP doesn't load as dead.
        _health = Mathf.Clamp(amount, 1, stats.TotalHealth);
        if (IsSpawned) _syncedHealth.Value = _health;
    }

    public void IncreaseMaxHealth(int amount)
    {
        stats.MaxHealth += amount;
        _health          = stats.TotalHealth;
        if (IsSpawned) _syncedHealth.Value = _health;
    }

    // -------------------------------------------------------------------------

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");

        if (CompareTag("Enemy"))
        {
            // In multiplayer this runs on the server; only the host player's
            // QuestLog is reachable here. Phase 7E will broadcast to the
            // correct client via TargetRpc.
            QuestLog questLog = FindAnyObjectByType<QuestLog>();
            questLog?.RegisterKill(
                gameObject.name.Replace("(Clone)", "").Trim()
            );
        }

        OnDeath?.Invoke();
    }
}
