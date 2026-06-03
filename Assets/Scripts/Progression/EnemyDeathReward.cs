using FishNet.Object;
using UnityEngine;

/// <summary>
/// Grants XP to all players when this enemy dies.
/// Uses ObserversRpc so every connected client receives the reward —
/// not just the host. Each client filters to its own (IsOwner) player.
/// </summary>
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(ExperienceReward))]
public class EnemyDeathReward : NetworkBehaviour
{
    private Health         health;
    private ExperienceReward reward;

    private void Awake()
    {
        health = GetComponent<Health>();
        reward = GetComponent<ExperienceReward>();
    }

    private void OnEnable()  { health.OnDeath += GiveExperience; }
    private void OnDisable() { health.OnDeath -= GiveExperience; }

    // -------------------------------------------------------------------------

    private void GiveExperience()
    {
        // Only the server broadcasts the XP reward.
        if (!IsServerInitialized && IsSpawned) return;

        RpcGiveExperience(reward.XPReward);
    }

    /// <summary>
    /// Runs on all clients when this enemy dies.
    /// Each client gives XP to its own (IsOwner) player only.
    /// </summary>
    [ObserversRpc]
    private void RpcGiveExperience(int xpAmount)
    {
        Experience[] experiences = FindObjectsByType<Experience>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Experience exp in experiences)
        {
            NetworkObject netObj = exp.GetComponent<NetworkObject>();
            bool isLocalPlayer   = netObj == null || netObj.IsOwner;
            if (!isLocalPlayer) continue;

            exp.GainExperience(xpAmount);
            return; // one owner player per client
        }
    }
}
