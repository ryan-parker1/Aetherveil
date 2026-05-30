using UnityEngine;

public class CombatStats : MonoBehaviour
{
    [Header("Combat Stats")]
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private int damage = 10;
    private int bonusHealth;
    private int bonusDamage;

    [SerializeField] private float attackRange = 2f;

    [SerializeField] private float detectionRange = 8f;

    [SerializeField] private float moveSpeed = 3.5f;

    [SerializeField] private float attackCooldown = 1.5f;

    public int TotalHealth => maxHealth + bonusHealth;

    public int TotalDamage => damage + bonusDamage;

    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    public int Damage
    {
        get => damage;
        set => damage = value;
    }
    public float AttackRange => attackRange;
    public float DetectionRange => detectionRange;
    public float MoveSpeed => moveSpeed;
    public float AttackCooldown => attackCooldown;
    public void IncreaseDamage(int amount)
    {
        damage += amount;
    }
    public void AddHealthBonus(int amount)
    {
        bonusHealth += amount;
    }

    public void AddDamageBonus(int amount)
    {
        bonusDamage += amount;
    }
    public void RemoveHealthBonus(int amount)
    {
        bonusHealth -= amount;
    }

    public void RemoveDamageBonus(int amount)
    {
        bonusDamage -= amount;
    }
}