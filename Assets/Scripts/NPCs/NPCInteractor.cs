using UnityEngine;

// Attach to the Player. Pressing E near any NPC opens dialogue or a shop.
// Pressing E while a window is open closes it.
public class NPCInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;

    private DialogueUI dialogueUI;
    private VendorUI vendorUI;

    private void Start()
    {
        dialogueUI = FindAnyObjectByType<DialogueUI>(FindObjectsInactive.Include);
        vendorUI   = FindAnyObjectByType<VendorUI>(FindObjectsInactive.Include);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        // E closes whichever window is already open
        if (dialogueUI != null && dialogueUI.IsOpen)
        {
            dialogueUI.Close();
            return;
        }

        if (vendorUI != null && vendorUI.IsOpen)
        {
            vendorUI.Close();
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

            // Dialogue takes priority if the NPC has both components
            NPCDialogue dialogue = hit.GetComponent<NPCDialogue>();
            if (dialogue != null)
            {
                dialogue.OpenDialogue();
                return;
            }

            NPCVendor vendor = hit.GetComponent<NPCVendor>();
            if (vendor != null)
            {
                vendor.OpenShop();
                return;
            }
        }
    }
}
