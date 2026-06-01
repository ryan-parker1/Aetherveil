using UnityEngine;

// Attach this to any NPC that should be able to speak.
// QuestGivers also need a QuestGiver component — NPCDialogue
// will detect it and drive the Accept / TurnIn flow automatically.
public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue States")]
    [Tooltip("Shown for NPCs with no quest, or as a fallback.")]
    [SerializeField] private DialogueData defaultDialogue;

    [Tooltip("Shown when the player hasn't accepted the quest yet. " +
             "Put questOffer in the DialogueData to show Accept/Decline.")]
    [SerializeField] private DialogueData questOfferDialogue;

    [Tooltip("Shown while the quest is active and not yet complete.")]
    [SerializeField] private DialogueData questActiveDialogue;

    [Tooltip("Shown when the quest is complete and ready to turn in.")]
    [SerializeField] private DialogueData questCompleteDialogue;

    private QuestGiver questGiver;

    private void Awake()
    {
        questGiver = GetComponent<QuestGiver>();
    }

    public void OpenDialogue()
    {
        DialogueUI ui = FindAnyObjectByType<DialogueUI>(FindObjectsInactive.Include);
        if (ui == null)
        {
            Debug.LogWarning("NPCDialogue: No DialogueUI found in scene.");
            return;
        }

        DialogueData data = ChooseDialogue();
        if (data == null || data.pages == null || data.pages.Length == 0)
        {
            Debug.LogWarning("NPCDialogue: Selected dialogue has no pages.");
            return;
        }

        // Wire quest events only when relevant
        if (data == questOfferDialogue && questGiver != null)
        {
            ui.OnQuestAccepted -= OnQuestAccepted;
            ui.OnQuestDeclined -= OnQuestDeclined;
            ui.OnQuestAccepted += OnQuestAccepted;
            ui.OnQuestDeclined += OnQuestDeclined;
        }
        else if (data == questCompleteDialogue && questGiver != null)
        {
            ui.OnDialogueClosed -= OnTurnInClosed;
            ui.OnDialogueClosed += OnTurnInClosed;
        }

        // Use the NPC name from DialogueData if set, else the GameObject name
        string npcName = !string.IsNullOrEmpty(data.npcName)
            ? data.npcName
            : gameObject.name;

        // questOffer is only passed for the offer state so the UI shows
        // Accept/Decline; all other states pass null
        QuestData questOffer = (data == questOfferDialogue)
            ? data.questOffer
            : null;

        ui.ShowDialogue(npcName, data.pages, questOffer);
    }

    // Pick the right dialogue based on quest state
    private DialogueData ChooseDialogue()
    {
        if (questGiver == null || questGiver.QuestData == null)
            return defaultDialogue;

        QuestLog questLog = FindAnyObjectByType<QuestLog>();
        if (questLog == null)
            return defaultDialogue;

        Quest quest = questLog.GetQuest(questGiver.QuestData);

        if (quest == null)
            return questOfferDialogue != null ? questOfferDialogue : defaultDialogue;

        switch (quest.Status)
        {
            case QuestStatus.Active:
                return questActiveDialogue != null ? questActiveDialogue : defaultDialogue;

            case QuestStatus.Complete:
                return questCompleteDialogue != null ? questCompleteDialogue : defaultDialogue;

            default: // TurnedIn or any future state
                return defaultDialogue;
        }
    }

    private void OnQuestAccepted()
    {
        UnsubscribeQuestEvents();
        questGiver?.AcceptQuest();
    }

    private void OnQuestDeclined()
    {
        UnsubscribeQuestEvents();
    }

    private void OnTurnInClosed()
    {
        DialogueUI ui = FindAnyObjectByType<DialogueUI>(FindObjectsInactive.Include);
        if (ui != null) ui.OnDialogueClosed -= OnTurnInClosed;

        questGiver?.TurnInQuest();
    }

    private void UnsubscribeQuestEvents()
    {
        DialogueUI ui = FindAnyObjectByType<DialogueUI>(FindObjectsInactive.Include);
        if (ui == null) return;
        ui.OnQuestAccepted -= OnQuestAccepted;
        ui.OnQuestDeclined -= OnQuestDeclined;
    }
}
