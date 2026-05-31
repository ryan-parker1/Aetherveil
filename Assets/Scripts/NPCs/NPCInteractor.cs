using UnityEngine;

public class NPCInteractor : MonoBehaviour
{
    [SerializeField]
    private float interactRange = 3f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                interactRange
            );

        foreach (Collider hit in hits)
        {
            QuestGiver questGiver =
                hit.GetComponent<QuestGiver>();

            if (questGiver != null)
            {
                NPCDialogue dialogue =
                    hit.GetComponent<NPCDialogue>();

                if (dialogue != null)
                {
                    dialogue.OpenDialogue();
                }

                questGiver.Interact();

                return;
            }
        }
    }
}