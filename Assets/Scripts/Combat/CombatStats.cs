using UnityEngine;

public class CombatStats : MonoBehaviour
{
    [Header("Combat Stats")]
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private int damage = 10;

    [SerializeField] private float attackRange = 2f;

    [SerializeField] private float detectionRange = 8f;

    [SerializeField] private float moveSpeed = 3.5f;

    [SerializeField] private float attackCooldown = 1.5f;

    public int MaxHealth => maxHealth;
    public int Damage => damage;
    public float AttackRange => attackRange;
    public float DetectionRange => detectionRange;
    public float MoveSpeed => moveSpeed;
    public float AttackCooldown => attackCooldown;
}