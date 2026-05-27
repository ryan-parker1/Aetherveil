using UnityEngine;

[RequireComponent(typeof(Health))]
public class Damageable : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void ReceiveDamage(int amount)
    {
        health.TakeDamage(amount);
    }
}