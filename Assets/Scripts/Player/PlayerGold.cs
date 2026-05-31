using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField]
    private int gold;

    public int Gold => gold;

    public void AddGold(int amount)
    {
        gold += amount;

        Debug.Log(
            "Gold: " + gold
        );
    }
}