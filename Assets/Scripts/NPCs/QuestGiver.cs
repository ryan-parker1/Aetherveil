using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    private QuestData quest;

    private bool rewardGiven;

    public void Interact()
    {
        QuestLog questLog =
            FindAnyObjectByType<QuestLog>();

        if (questLog == null)
            return;

        Quest existingQuest =
            questLog.GetQuest(quest);

        if (existingQuest == null)
        {
            questLog.AcceptQuest(quest);

            Debug.Log(
                "Quest given: " +
                quest.questName
            );

            return;
        }

        if (
            existingQuest.Status ==
            QuestStatus.Complete
            &&
            !rewardGiven
        )
        {
            GiveRewards();

            rewardGiven = true;

            existingQuest.Status =
                QuestStatus.TurnedIn;
        }
    }

    private void GiveRewards()
    {
        Debug.Log(
            "Quest Turned In!"
        );

        Experience experience =
            FindAnyObjectByType<Experience>();

        if (experience != null)
        {
            experience.GainExperience(
                quest.rewardXP
            );
        }

        PlayerGold gold =
            FindAnyObjectByType<PlayerGold>();

        if (gold != null)
        {
            gold.AddGold(
                quest.rewardGold
            );
        }

        Inventory inventory =
            FindAnyObjectByType<Inventory>();

        if (
            inventory != null &&
            quest.rewardItem != null
        )
        {
            inventory.AddItem(
                quest.rewardItem
            );

            Debug.Log(
                "Received: " +
                quest.rewardItem.itemName
            );
        }

        Debug.Log(
            "Rewards Granted"
        );
    }
}