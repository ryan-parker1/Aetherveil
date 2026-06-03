using System;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

/// <summary>
/// Tracks and synchronises hit points for any actor (player or enemy).
///
/// Network mode  : SyncVar<int> replicates currentHealth from server to all clients.
/// Offline mode  : Plain int is used directly; SyncVar is never touched.
///
/// When an enemy dies the server broadcasts RpcRegisterKill to all clients so
/// every player's QuestLog receives the kill — not just the host's.
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
        _health = stats.TotalHealth;
        _syncedHealth.Value = _health;
    }

    // -------------------------------------------------------------------------

    public void TakeDamage(int amount)
    {
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
            // Broadcast to all clients so every player's QuestLog gets the kill.
            // Each client filters to its own (IsOwner) player's QuestLog.
            string enemyName = gameObject.name.Replace("(Clone)", "").Trim();
            RpcRegisterKill(enemyName);
        }

        OnDeath?.Invoke();
    }

    /// <summary>
    /// Runs on all clients (including host) when an enemy dies.
    /// Each client registers the kill only in the OWNER player's QuestLog,
    /// preventing non-owner player prefabs from also claiming the kill.
    /// </summary>
    [ObserversRpc]
    private void RpcRegisterKill(string enemyName)
    {
        // Find all QuestLogs in scene — in multiplayer there will be one per player.
        QuestLog[] logs = FindObjectsByType<QuestLog>(FindObjectsInactive.Include);

        foreach (QuestLog log in logs)
        {
            // Each client only updates the QuestLog belonging to its owned player.
            NetworkObject netObj = log.GetComponent<NetworkObject>();
            bool isLocalPlayer   = netObj == null || netObj.IsOwner;
            if (!isLocalPlayer) continue;

            log.RegisterKill(enemyName);
            return; // one owner player per client
        }
    }
}
