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
        questLog = FindAnyObjectByType<QuestLog>();
    }

    // Called by NetworkPlayerSetup when the local player spawns
    public void SetPlayer(GameObject localPlayer)
    {
        player              = localPlayer;
        experience          = player.GetComponent<Experience>();
        health              = player.GetComponent<Health>();
        combatStats         = player.GetComponent<CombatStats>();
        playerGold          = player.GetComponent<PlayerGold>();
        inventory           = player.GetComponent<Inventory>();
        equipmentManager    = player.GetComponent<EquipmentManager>();
        characterController = player.GetComponent<CharacterController>();

        // QuestLog may be on the player or elsewhere — find it now
        if (questLog == null)
            questLog = player.GetComponent<QuestLog>()
                ?? FindAnyObjectByType<QuestLog>();

        if (SaveSystem.SaveExists())
            LoadGame();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            SaveGame();
    }

    private void OnApplicationQuit()
    {
        // Player may already be destroyed when stopping Play mode in editor
        if (player != null)
            SaveGame();
    }

    // ─────────────────────────────────────────
    //  SAVE
    // ─────────────────────────────────────────
    public void SaveGame()
    {
        if (player == null)
        {
            Debug.LogWarning("SaveGame: player is null, skipping.");
            return;
        }

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
        // Never save at 0 HP — player should load alive next session.
        data.currentHealth = health.IsDead
            ? combatStats.TotalHealth
            : Mathf.Max(1, health.CurrentHealth);

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
