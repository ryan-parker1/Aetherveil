using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueWindow;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    private void Start()
    {
        dialogueWindow.SetActive(false);
    }

    public void ShowDialogue(string text)
    {
        dialogueWindow.SetActive(true);
        dialogueText.text = text;
    }

    public void CloseDialogue()
    {
        dialogueWindow.SetActive(false);
    }
}