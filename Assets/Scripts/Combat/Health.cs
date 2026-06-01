using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    private CombatStats stats;

    private int currentHealth;

    public int CurrentHealth => currentHealth;

    public bool IsDead => currentHealth <= 0;

    public event Action OnDeath;

    private void Awake()
    {
        stats = GetComponent<CombatStats>();

        currentHealth = stats.TotalHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead)
            return;

        currentHealth -= amount;

        Debug.Log(
            gameObject.name +
            " took " +
            amount +
            " damage. Remaining HP: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void RestoreFullHealth()
    {
        currentHealth = stats.TotalHealth;
    }

    // Called by GameSaveManager on load
    public void SetCurrentHealth(int amount)
    {
        currentHealth = Mathf.Clamp(amount, 0, stats.TotalHealth);
    }

    public void IncreaseMaxHealth(int amount)
    {
        stats.MaxHealth += amount;

        currentHealth = stats.TotalHealth;
    }

    private void Die()
    {
        currentHealth = 0;

        Debug.Log(gameObject.name + " died.");

        Debug.Log("Die() reached for: " + gameObject.name);

        if (CompareTag("Enemy"))
        {
            QuestLog questLog =
                FindAnyObjectByType<QuestLog>();

            if (questLog != null)
            {
                questLog.RegisterKill(
                    gameObject.name.Replace(
                        "(Clone)",
                        ""
                    )
                );
            }
        }
        
        OnDeath?.Invoke();
    }
}