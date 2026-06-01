using UnityEngine;

// Attach to the Player. Pressing E near any NPC with an NPCDialogue
// component opens their dialogue. Pressing E again closes it.
public class NPCInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;

    private DialogueUI dialogueUI;

    private void Start()
    {
        dialogueUI = FindAnyObjectByType<DialogueUI>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        // If dialogue is already open, E closes it
        if (dialogueUI != null && dialogueUI.IsOpen)
        {
            dialogueUI.Close();
            return;
        }

        TryInteract();
    }

    private void TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactRange
        );

        foreach (Collider hit in hits)
        {
            // Skip self
            if (hit.gameObject == gameObject) continue;

            NPCDialogue dialogue = hit.GetComponent<NPCDialogue>();
            if (dialogue != null)
            {
                dialogue.OpenDialogue();
                return;
            }
        }
    }
}
