using System.Collections.Generic;
using UnityEngine;

public class QuestLog : MonoBehaviour
{
    public event System.Action OnQuestUpdated;
    private List<Quest> activeQuests =
        new List<Quest>();

    public void AcceptQuest(QuestData data)
    {
        activeQuests.Add(
            new Quest(data)
        );

        OnQuestUpdated?.Invoke();

        Debug.Log(
            "Accepted Quest: " +
            data.questName
        );

        Debug.Log(
            "Quest Count: " +
            activeQuests.Count
        );
    }

    public List<Quest> ActiveQuests
    {
        get => activeQuests;
    }

    public void RegisterKill(
        string enemyName
    )
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.Status != QuestStatus.Active)
                continue;

            if (quest.Data.targetEnemyName != enemyName)
                continue;

            quest.AddKill();

            OnQuestUpdated?.Invoke();

            Debug.Log(
                quest.Data.questName +
                ": " +
                quest.CurrentKills +
                "/" +
                quest.Data.requiredKills
            );

            if (quest.Status ==
                QuestStatus.Complete)
            {
                Debug.Log(
                    "Quest Complete: " +
                    quest.Data.questName
                );
            }
        }
    }

    public Quest GetQuest(
        QuestData questData
    )
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.Data == questData)
            {
                return quest;
            }
        }

        return null;
    }
}