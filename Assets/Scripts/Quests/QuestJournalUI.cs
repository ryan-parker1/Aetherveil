using System.Text;
using TMPro;
using UnityEngine;

public class QuestJournalUI : MonoBehaviour
{
    [SerializeField] private GameObject        questJournal;
    [SerializeField] private TextMeshProUGUI   questText;

    // QuestLog lives on the Player prefab spawned by FishNet — find lazily.
    private QuestLog questLog;

    // -------------------------------------------------------------------------

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
        questJournal.SetActive(!questJournal.activeSelf);

        if (questJournal.activeSelf)
            Refresh();
    }

    // -------------------------------------------------------------------------

    private void TryCacheQuestLog()
    {
        if (questLog != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            questLog = player.GetComponent<QuestLog>();
            if (questLog != null)
                questLog.OnQuestUpdated += Refresh;
        }
    }

    private void OnDisable()
    {
        if (questLog != null)
            questLog.OnQuestUpdated -= Refresh;
    }

    // -------------------------------------------------------------------------

    private void Refresh()
    {
        TryCacheQuestLog();

        if (questLog == null)
        {
            Debug.LogWarning("QuestJournalUI: player not spawned yet — quest log unavailable.");
            return;
        }

        var builder = new StringBuilder();

        foreach (Quest quest in questLog.ActiveQuests)
        {
            builder.AppendLine(quest.Data.questName);
            builder.AppendLine(
                $"{quest.CurrentKills}/{quest.Data.requiredKills}"
            );
            builder.AppendLine();
        }

        questText.text = builder.ToString();
    }
}
