using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField] private int gold;

    public int Gold => gold;

    // Fired whenever gold changes so UI can update
    public event System.Action OnGoldChanged;

    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke();
        Debug.Log("Gold: " + gold);
    }

    // Returns false if the player can't afford it
    public bool SpendGold(int amount)
    {
        if (gold < amount)
        {
            Debug.Log("Not enough gold.");
            return false;
        }

        gold -= amount;
        OnGoldChanged?.Invoke();
        Debug.Log("Gold spent: " + amount + " | Remaining: " + gold);
        return true;
    }
}
