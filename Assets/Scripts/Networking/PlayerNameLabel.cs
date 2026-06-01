using FishNet.Object;
using TMPro;
using UnityEngine;

/// <summary>
/// Attach to the Player prefab.
/// Displays a floating name label above the character that always faces the camera.
/// NetworkPlayerSetup calls SetPlayerName() after the client spawns.
/// </summary>
public class PlayerNameLabel : NetworkBehaviour
{
    [Header("References")]
    [Tooltip("Drag the TextMeshPro component inside the NameLabel child here.")]
    [SerializeField] private TextMeshPro nameText;

    [Header("Settings")]
    [SerializeField] private float verticalOffset = 2.4f;
    [SerializeField] private float labelScale    = 0.4f;

    private Transform _cam;

    // -------------------------------------------------------------------------

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Hide our OWN label — we don't need to see our own name above our head
        if (IsOwner && nameText != null)
        {
            nameText.gameObject.SetActive(false);
            return;
        }

        // Default label text until the server/owner syncs the real name
        if (nameText != null)
            nameText.text = $"Player {OwnerId}";
    }

    private void Start()
    {
        _cam = Camera.main != null ? Camera.main.transform : null;

        // Position the text above the character
        if (nameText != null)
        {
            nameText.transform.localPosition = new Vector3(0f, verticalOffset, 0f);
            nameText.transform.localScale    = Vector3.one * labelScale;
        }
    }

    private void LateUpdate()
    {
        if (nameText == null || !nameText.gameObject.activeSelf) return;

        // Billboard — rotate label to always face the camera
        if (_cam == null)
            _cam = Camera.main != null ? Camera.main.transform : null;

        if (_cam != null)
            nameText.transform.rotation = Quaternion.LookRotation(
                nameText.transform.position - _cam.position
            );
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// Called by NetworkPlayerSetup on all clients to set the visible name.
    /// </summary>
    public void SetPlayerName(string playerName)
    {
        if (nameText == null) return;

        nameText.text = playerName;

        // Still hide our own label even after name is set
        if (IsOwner)
            nameText.gameObject.SetActive(false);
    }
}
