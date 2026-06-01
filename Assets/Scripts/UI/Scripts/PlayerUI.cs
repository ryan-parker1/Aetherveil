using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;

    private Health playerHealth;
    private CombatStats playerStats;

    private void Update()
    {
        // Find player lazily after FishNet spawns it
        if (playerHealth == null)
        {
            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
                playerStats  = player.GetComponent<CombatStats>();
            }

            return;
        }

        // Keep maxValue in sync in case stats change (level up, equipment)
        if (playerStats != null)
            healthBar.maxValue = playerStats.TotalHealth;

        healthBar.value = playerHealth.CurrentHealth;
    }
}
