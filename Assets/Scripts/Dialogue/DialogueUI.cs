using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private GameObject dialogueWindow;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    // Fired when the player clicks Accept on a quest offer
    public event System.Action OnQuestAccepted;

    // Fired when the player clicks Decline on a quest offer
    public event System.Action OnQuestDeclined;

    // Fired whenever the dialogue window closes for any reason
    public event System.Action OnDialogueClosed;

    public bool IsOpen => dialogueWindow.activeSelf;

    private string[] pages;
    private int currentPage;
    private QuestData pendingQuest;

    private void Start()
    {
        dialogueWindow.SetActive(false);

        nextButton.onClick.AddListener(NextPage);
        closeButton.onClick.AddListener(Close);
        acceptButton.onClick.AddListener(HandleAccept);
        declineButton.onClick.AddListener(HandleDecline);
    }

    // Called by NPCDialogue to open a conversation
    public void ShowDialogue(
        string npcName,
        string[] dialoguePages,
        QuestData questOffer = null
    )
    {
        pages        = dialoguePages;
        currentPage  = 0;
        pendingQuest = questOffer;

        npcNameText.text = npcName;

        dialogueWindow.SetActive(true);
        DisplayCurrentPage();
    }

    private void DisplayCurrentPage()
    {
        dialogueText.text = pages[currentPage];

        bool isLastPage = currentPage >= pages.Length - 1;

        // Next: visible on every page except the last
        nextButton.gameObject.SetActive(!isLastPage);

        if (isLastPage && pendingQuest != null)
        {
            // Quest offer — show Accept / Decline, hide Close
            closeButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(true);
            declineButton.gameObject.SetActive(true);
        }
        else if (isLastPage)
        {
            // Normal last page — show Close only
            closeButton.gameObject.SetActive(true);
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
        }
        else
        {
            // Mid-conversation — hide all decision buttons
            closeButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
        }
    }

    private void NextPage()
    {
        currentPage++;
        DisplayCurrentPage();
    }

    private void HandleAccept()
    {
        OnQuestAccepted?.Invoke();
        CloseInternal();
    }

    private void HandleDecline()
    {
        OnQuestDeclined?.Invoke();
        CloseInternal();
    }

    // Public so NPCInteractor can close via E key
    public void Close()
    {
        CloseInternal();
    }

    private void CloseInternal()
    {
        dialogueWindow.SetActive(false);
        pendingQuest = null;
        OnDialogueClosed?.Invoke();
    }
}
