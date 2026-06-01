using System;
using System.Collections.Generic;

// Plain serializable containers — no Unity types, no MonoBehaviours.
// These are what actually gets written to the JSON file.

[Serializable]
public class SaveData
{
    // Scene
    public string sceneName;

    // Player transform
    public float posX;
    public float posY;
    public float posZ;

    // Progression
    public int level;
    public int currentXP;
    public int requiredXP;

    // Economy
    public int gold;

    // Combat stats (base values, before equipment bonuses)
    public int maxHealth;
    public int damage;
    public int currentHealth;

    // Inventory
    public List<SavedItem> inventory = new List<SavedItem>();

    // Equipment (one entry per occupied slot)
    public List<SavedEquipment> equipment = new List<SavedEquipment>();

    // Quests
    public List<SavedQuest> quests = new List<SavedQuest>();
}

[Serializable]
public class SavedItem
{
    public string itemName;
    public int quantity;
}

[Serializable]
public class SavedEquipment
{
    public string slotName;   // matches EquipmentSlot enum name
    public string itemName;
}

[Serializable]
public class SavedQuest
{
    public string questName;
    public string status;     // matches QuestStatus enum name
    public int currentKills;
}
