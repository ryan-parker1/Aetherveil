using UnityEngine;

public class Health : MonoBehaviour
{
    private CombatStats stats;

    private int currentHealth;

    public int CurrentHealth => currentHealth;

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        stats = GetComponent<CombatStats>();

        currentHealth = stats.MaxHealth;
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

    private void Die()
    {
        currentHealth = 0;

        Debug.Log(gameObject.name + " died.");

        Destroy(gameObject);
    }
}