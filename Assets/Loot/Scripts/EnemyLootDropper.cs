using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(LootTable))]
public class EnemyLootDropper : MonoBehaviour
{
    private Health health;

    private LootTable lootTable;

    private void Awake()
    {
        health = GetComponent<Health>();

        lootTable = GetComponent<LootTable>();
    }

    private void OnEnable()
    {
        health.OnDeath += DropLoot;
    }

    private void OnDisable()
    {
        health.OnDeath -= DropLoot;
    }

    private void DropLoot()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        Inventory inventory =
            player.GetComponent<Inventory>();

        if (inventory == null)
            return;

        lootTable.RollLoot(inventory);
    }
}