using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField]
    private DialogueData dialogue;

    public void OpenDialogue()
    {
        Debug.Log("OpenDialogue called");

        DialogueUI ui =
            FindAnyObjectByType<DialogueUI>();

        Debug.Log("UI Found? " + (ui != null));

        if (ui != null)
        {
            ui.ShowDialogue(
                dialogue.dialogueText
            );
        }
    }
}