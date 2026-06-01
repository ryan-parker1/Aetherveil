using System;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

/// <summary>
/// Tracks and synchronises hit points for any actor (player or enemy).
/// Uses FishNet SyncVar<int> so all clients see the correct health value.
/// Mutating methods (TakeDamage, RestoreFullHealth, etc.) must only be
/// called on the server in multiplayer, or freely in offline mode.
/// </summary>
public class Health : NetworkBehaviour
{
    private CombatStats stats;

    // SyncVar<T> — server sets Value, FishNet replicates to all clients.
    private readonly SyncVar<int> _currentHealth = new SyncVar<int>();

    public int  CurrentHealth => _currentHealth.Value;
    public bool IsDead        => _currentHealth.Value <= 0;

    public event Action OnDeath;

    // -------------------------------------------------------------------------

    private void Awake()
    {
        stats = GetComponent<CombatStats>();
        // Initialise locally so the value is valid before OnStartServer fires.
        // In offline play (no FishNet) this is the only initialisation path.
        _currentHealth.Value = stats.TotalHealth;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Re-initialise on the server so the SyncVar broadcast starts correct.
        _currentHealth.Value = stats.TotalHealth;
    }

    // -------------------------------------------------------------------------
    // True when this instance is allowed to mutate health:
    //   • Offline (FishNet not running at all), or
    //   • This object is initialised on the server/host.
    private bool IsAuthoritative =>
        (!InstanceFinder.IsClientStarted && !InstanceFinder.IsServerStarted)
        || IsServerInitialized;

    // -------------------------------------------------------------------------

    public void TakeDamage(int amount)
    {
        if (!IsAuthoritative) return;
        if (IsDead) return;

        _currentHealth.Value -= amount;

        Debug.Log(
            $"{gameObject.name} took {amount} damage. Remaining HP: {_currentHealth.Value}"
        );

        if (_currentHealth.Value <= 0)
        {
            _currentHealth.Value = 0;
            Die();
        }
    }

    public void RestoreFullHealth()
    {
        if (!IsAuthoritative) return;
        _currentHealth.Value = stats.TotalHealth;
    }

    /// <summary>Called by GameSaveManager on load.</summary>
    public void SetCurrentHealth(int amount)
    {
        if (!IsAuthoritative) return;
        _currentHealth.Value = Mathf.Clamp(amount, 0, stats.TotalHealth);
    }

    public void IncreaseMaxHealth(int amount)
    {
        if (!IsAuthoritative) return;
        stats.MaxHealth      += amount;
        _currentHealth.Value  = stats.TotalHealth;
    }

    // -------------------------------------------------------------------------

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");

        if (CompareTag("Enemy"))
        {
            // In multiplayer this runs on the server; only the host player's
            // QuestLog is found here. Phase 7E will broadcast the kill to the
            // correct client via TargetRpc.
            QuestLog questLog = FindAnyObjectByType<QuestLog>();
            questLog?.RegisterKill(
                gameObject.name.Replace("(Clone)", "").Trim()
            );
        }

        OnDeath?.Invoke();
    }
}
