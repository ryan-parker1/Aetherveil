using UnityEngine;

// Attach to any NPC that should open a shop when interacted with.
// Can coexist with NPCDialogue — dialogue will open first, shop on next press.
public class NPCVendor : MonoBehaviour
{
    [SerializeField] private VendorData vendorData;

    public void OpenShop()
    {
        VendorUI ui = FindAnyObjectByType<VendorUI>(FindObjectsInactive.Include);

        if (ui == null)
        {
            Debug.LogWarning("NPCVendor: No VendorUI found in scene.");
            return;
        }

        if (vendorData == null)
        {
            Debug.LogWarning("NPCVendor: No VendorData assigned on " + gameObject.name);
            return;
        }

        ui.OpenShop(vendorData);
    }
}
