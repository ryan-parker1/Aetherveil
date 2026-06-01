using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to a persistent GameObject in the scene.
// F5 = manual save. Auto-saves on quit. Auto-loads on start if a save exists.
public class GameSaveManager : MonoBehaviour
{
    [Header("Registry")]
    [SerializeField] private GameRegistry registry;

    [Header("Player")]
    [SerializeField] private GameObject player;

    // Cached component references
    private Experience    experience;
    private Health        health;
    private CombatStats   combatStats;
    private PlayerGold    playerGold;
    private Inventory     inventory;
    private EquipmentManager equipmentManager;
    private QuestLog      questLog;
    private CharacterController characterController;

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        experience         = player.GetComponent<Experience>();
        health             = player.GetComponent<Health>();
        combatStats        = player.GetComponent<CombatStats>();
        playerGold         = player.GetComponent<PlayerGold>();
        inventory          = player.GetComponent<Inventory>();
        equipmentManager   = player.GetComponent<EquipmentManager>();
        questLog           = FindAnyObjectByType<QuestLog>();
        characterController = player.GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (SaveSystem.SaveExists())
            LoadGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    // ─────────────────────────────────────────
    //  SAVE
    // ─────────────────────────────────────────
    public void SaveGame()
    {
        SaveData data = new SaveData();

        // Scene
        data.sceneName = SceneManager.GetActiveScene().name;

        // Position
        data.posX = player.transform.position.x;
        data.posY = player.transform.position.y;
        data.posZ = player.transform.position.z;

        // Progression
        data.level      = experience.CurrentLevel;
        data.currentXP  = experience.CurrentXP;
        data.requiredXP = experience.RequiredXP;

        // Economy
        data.gold = playerGold.Gold;

        // Combat stats — save base values (before equipment bonuses)
        data.maxHealth     = combatStats.MaxHealth;
        data.damage        = combatStats.Damage;
        data.currentHealth = health.CurrentHealth;

        // Inventory
        foreach (InventorySlot slot in inventory.Slots)
        {
            data.inventory.Add(new SavedItem
            {
                itemName = slot.Item.itemName,
                quantity = slot.Quantity
            });
        }

        // Equipment
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            ItemData item = equipmentManager.GetEquippedItem(slot);
            if (item != null)
            {
                data.equipment.Add(new SavedEquipment
                {
                    slotName = slot.ToString(),
                    itemName = item.itemName
                });
            }
        }

        // Quests
        foreach (Quest quest in questLog.ActiveQuests)
        {
            data.quests.Add(new SavedQuest
            {
                questName    = quest.Data.questName,
                status       = quest.Status.ToString(),
                currentKills = quest.CurrentKills
            });
        }

        SaveSystem.Save(data);
        Debug.Log("Game saved.");
    }

    // ─────────────────────────────────────────
    //  LOAD
    // ─────────────────────────────────────────
    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();
        if (data == null) return;

        // Position — disable CharacterController to move transform directly
        characterController.enabled = false;
        player.transform.position = new Vector3(data.posX, data.posY, data.posZ);
        characterController.enabled = true;

        // Progression
        experience.LoadSaveData(data.level, data.currentXP, data.requiredXP);

        // Economy
        playerGold.SetGold(data.gold);

        // Combat stats — restore base values first, then re-equip adds bonuses
        combatStats.MaxHealth = data.maxHealth;
        combatStats.Damage    = data.damage;

        // Inventory — clear then restore
        inventory.ClearAll();
        foreach (SavedItem saved in data.inventory)
        {
            ItemData item = registry.GetItem(saved.itemName);
            if (item != null)
                inventory.AddItem(item, saved.quantity);
        }

        // Equipment — unequip all then re-equip (re-applies stat bonuses)
        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            equipmentManager.Unequip(slot);

        foreach (SavedEquipment saved in data.equipment)
        {
            ItemData item = registry.GetItem(saved.itemName);
            if (item != null)
                equipmentManager.Equip(item);
        }

        // Restore current health after stats and equipment are set
        health.SetCurrentHealth(data.currentHealth);

        // Quests — clear then restore
        questLog.ClearAll();
        foreach (SavedQuest saved in data.quests)
        {
            QuestData questData = registry.GetQuest(saved.questName);
            if (questData == null) continue;

            QuestStatus status = (QuestStatus)Enum.Parse(
                typeof(QuestStatus), saved.status
            );

            questLog.LoadSavedQuest(questData, status, saved.currentKills);
        }

        Debug.Log("Game loaded.");
    }
}
