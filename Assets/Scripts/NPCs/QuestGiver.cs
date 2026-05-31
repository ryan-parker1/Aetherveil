using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    private QuestData quest;

    private bool rewardGiven;

    public void Interact()
    {
        QuestLog questLog =
            FindFirstObjectByType<QuestLog>();

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

        LevelSystem levelSystem =
            FindFirstObjectByType<LevelSystem>();

        if (levelSystem != null)
        {
            levelSystem.AddExperience(
                quest.rewardXP
            );
        }

        PlayerGold gold =
            FindFirstObjectByType<PlayerGold>();

        if (gold != null)
        {
            gold.AddGold(
                quest.rewardGold
            );
        }

        Debug.Log(
            "Rewards Granted"
        );
    }
}