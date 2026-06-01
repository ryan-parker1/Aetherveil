using UnityEngine;

public class Experience : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] private int currentLevel = 1;

    [SerializeField] private int currentXP = 0;

    [SerializeField] private int requiredXP = 100;

    [Header("Scaling")]
    [SerializeField] private float xpMultiplier = 1.5f;

    public int CurrentLevel => currentLevel;

    public int CurrentXP => currentXP;

    public int RequiredXP => requiredXP;

    // Called by GameSaveManager on load
    public void LoadSaveData(int level, int xp, int required)
    {
        currentLevel = level;
        currentXP    = xp;
        requiredXP   = required;
    }

    public void GainExperience(int amount)
    {
        currentXP += amount;

        Debug.Log("Gained XP: " + amount);

        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (currentXP >= requiredXP)
        {
            currentXP -= requiredXP;

            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;

        requiredXP =
            Mathf.RoundToInt(
                requiredXP * xpMultiplier
            );

        Debug.Log(
            "LEVEL UP! Reached Level " +
            currentLevel
        );

        IncreaseStats();
    }

    private void IncreaseStats()
    {
        Health health =
            GetComponent<Health>();

        CombatStats combatStats =
            GetComponent<CombatStats>();

        if (health != null)
        {
            health.IncreaseMaxHealth(10);
        }

        if (combatStats != null)
        {
            combatStats.IncreaseDamage(2);
        }
    }
}