using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Handles both the equipment slots and the character stats panel.
// Toggle the window open/closed with K.
public class EquipmentUI : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private GameObject equipmentWindow;

    [Header("Equipment Manager")]
    [SerializeField] private EquipmentManager equipmentManager;

    [Header("Equipment Slots")]
    [SerializeField] private Image headSlot;
    [SerializeField] private Image chestSlot;
    [SerializeField] private Image gloveSlot;
    [SerializeField] private Image weaponSlot;
    [SerializeField] private Image legSlot;
    [SerializeField] private Image feetSlot;

    [Header("Character Stats")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI goldText;

    private Experience experience;
    private Health health;
    private CombatStats combatStats;
    private PlayerGold playerGold;

    private void Start()
    {
        equipmentWindow.SetActive(false);

        equipmentManager.OnEquipmentChanged += Refresh;

        // Cache player component references
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            experience  = player.GetComponent<Experience>();
            health      = player.GetComponent<Health>();
            combatStats = player.GetComponent<CombatStats>();
            playerGold  = player.GetComponent<PlayerGold>();
        }

        // Subscribe to gold changes so the display updates in real time
        if (playerGold != null)
            playerGold.OnGoldChanged += RefreshStats;
    }

    private void OnDestroy()
    {
        equipmentManager.OnEquipmentChanged -= Refresh;

        if (playerGold != null)
            playerGold.OnGoldChanged -= RefreshStats;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            Toggle();
    }

    private void Toggle()
    {
        equipmentWindow.SetActive(!equipmentWindow.activeSelf);

        if (equipmentWindow.activeSelf)
            Refresh();
    }

    // Refreshes both equipment slots and stats
    private void Refresh()
    {
        RefreshEquipmentSlots();
        RefreshStats();
    }

    private void RefreshEquipmentSlots()
    {
        UpdateSlot(EquipmentSlot.Head,   headSlot);
        UpdateSlot(EquipmentSlot.Chest,  chestSlot);
        UpdateSlot(EquipmentSlot.Glove,  gloveSlot);
        UpdateSlot(EquipmentSlot.Weapon, weaponSlot);
        UpdateSlot(EquipmentSlot.Legs,   legSlot);
        UpdateSlot(EquipmentSlot.Feet,   feetSlot);
    }

    private void RefreshStats()
    {
        if (levelText != null && experience != null)
            levelText.text = "Level: " + experience.CurrentLevel;

        if (healthText != null && health != null && combatStats != null)
            healthText.text = "HP: " + health.CurrentHealth + " / " + combatStats.TotalHealth;

        if (damageText != null && combatStats != null)
            damageText.text = "Damage: " + combatStats.TotalDamage;

        if (xpText != null && experience != null)
            xpText.text = "XP: " + experience.CurrentXP + " / " + experience.RequiredXP;

        if (goldText != null && playerGold != null)
            goldText.text = "Gold: " + playerGold.Gold + "g";
    }

    private void UpdateSlot(EquipmentSlot slot, Image image)
    {
        if (image == null)
        {
            Debug.LogError(slot + " image is not assigned!");
            return;
        }

        ItemData item = equipmentManager.GetEquippedItem(slot);

        if (item != null)
        {
            image.sprite = item.icon;
            image.color  = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color  = new Color(1, 1, 1, 0.2f);
        }
    }
}
