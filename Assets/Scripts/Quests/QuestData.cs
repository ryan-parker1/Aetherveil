using UnityEngine;

[CreateAssetMenu(
    fileName = "New Quest",
    menuName = "MMO/Quest"
)]
public class QuestData : ScriptableObject
{
    [Header("Basic")]
    public string questName;

    [TextArea]
    public string description;

    [Header("Objective")]
    public string targetEnemyName;

    public int requiredKills = 5;

    [Header("Rewards")]
    public int rewardXP = 100;

    public int rewardGold = 25;

    public ItemData rewardItem;
}