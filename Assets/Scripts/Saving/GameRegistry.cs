using System.Collections.Generic;
using UnityEngine;

// Assign all ItemData and QuestData assets here once.
// The save system uses this to look up assets by name at load time.
[CreateAssetMenu(
    fileName = "GameRegistry",
    menuName = "MMO/Game Registry"
)]
public class GameRegistry : ScriptableObject
{
    [Header("All Items")]
    public List<ItemData> items;

    [Header("All Quests")]
    public List<QuestData> quests;

    public ItemData GetItem(string itemName)
    {
        foreach (ItemData item in items)
        {
            if (item.itemName == itemName)
                return item;
        }

        Debug.LogWarning("GameRegistry: Item not found: " + itemName);
        return null;
    }

    public QuestData GetQuest(string questName)
    {
        foreach (QuestData quest in quests)
        {
            if (quest.questName == questName)
                return quest;
        }

        Debug.LogWarning("GameRegistry: Quest not found: " + questName);
        return null;
    }
}
