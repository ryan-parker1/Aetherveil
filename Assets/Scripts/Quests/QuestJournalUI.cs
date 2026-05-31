using TMPro;
using UnityEngine;
using System.Text;

public class QuestJournalUI : MonoBehaviour
{
    [SerializeField]
    private GameObject questJournal;

    [SerializeField]
    private GameObject content;

    [SerializeField]
    private QuestLog questLog;

    [SerializeField]
    private TextMeshProUGUI questText;

    private bool isOpen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("J pressed");

            Toggle();
        }
    }

    private void Toggle()
    {
        questJournal.SetActive(
            !questJournal.activeSelf
        );

        if (questJournal.activeSelf)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        Debug.Log("Active Quests Count: " + questLog.ActiveQuests.Count);

        Debug.Log("Refresh called");

        Debug.Log("QuestLog: " + questLog);

        Debug.Log("QuestText: " + questText);

        StringBuilder builder =
            new StringBuilder();

        foreach (Quest quest in questLog.ActiveQuests)
        {
            builder.AppendLine(
                quest.Data.questName
            );

            builder.AppendLine(
                quest.CurrentKills +
                "/" +
                quest.Data.requiredKills
            );

            builder.AppendLine("");
        }

        questText.text =
            builder.ToString();
    }

    private void OnEnable()
    {
        if (questLog != null)
        {
            questLog.OnQuestUpdated += Refresh;
        }
    }

    private void OnDisable()
    {
        if (questLog != null)
        {
            questLog.OnQuestUpdated -= Refresh;
        }
    }
}