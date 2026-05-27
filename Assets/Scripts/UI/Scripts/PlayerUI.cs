using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;

    private Health playerHealth;

    private void Start()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();

            CombatStats stats =
                player.GetComponent<CombatStats>();

            healthBar.maxValue = stats.MaxHealth;
        }
    }

    private void Update()
    {
        if (playerHealth == null)
            return;

        healthBar.value = playerHealth.CurrentHealth;
    }
}