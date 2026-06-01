using UnityEngine;

// QuestGiver no longer drives the dialogue flow itself.
// NPCDialogue picks the right dialogue state and calls
// AcceptQuest() / TurnInQuest() based on player input.
public class QuestGiver : MonoBehaviour
{
    [SerializeField] private QuestData quest;

    private bool rewardGiven;

    // Read by NPCDialogue to know which quest this NPC manages
    public QuestData QuestData => quest;

    public void AcceptQuest()
    {
        QuestLog questLog = FindAnyObjectByType<QuestLog>();
        if (questLog == null) return;

        if (questLog.GetQuest(quest) != null) return; // already accepted

        questLog.AcceptQuest(quest);
        Debug.Log("Quest accepted: " + quest.questName);
    }

    public void TurnInQuest()
    {
        if (rewardGiven) return;

        QuestLog questLog = FindAnyObjectByType<QuestLog>();
        if (questLog == null) return;

        Quest existingQuest = questLog.GetQuest(quest);
        if (existingQuest == null) return;

        if (existingQuest.Status != QuestStatus.Complete) return;

        GiveRewards();
        rewardGiven = true;
        existingQuest.Status = QuestStatus.TurnedIn;
    }

    private void GiveRewards()
    {
        Debug.Log("Quest Turned In: " + quest.questName);

        Experience experience = FindAnyObjectByType<Experience>();
        if (experience != null)
            experience.GainExperience(quest.rewardXP);

        PlayerGold gold = FindAnyObjectByType<PlayerGold>();
        if (gold != null)
            gold.AddGold(quest.rewardGold);

        Inventory inventory = FindAnyObjectByType<Inventory>();
        if (inventory != null && quest.rewardItem != null)
        {
            inventory.AddItem(quest.rewardItem);
            Debug.Log("Item rewarded: " + quest.rewardItem.itemName);
        }

        Debug.Log("Rewards granted.");
    }
}
