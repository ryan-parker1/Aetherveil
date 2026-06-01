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

        // Owner never sees their own name label — leave it inactive
        if (IsOwner) return;

        // Remote player: activate the label and set a default name.
        // SetPlayerName() called by NetworkPlayerSetup will update this.
        if (nameText != null)
        {
            nameText.gameObject.SetActive(true);
            nameText.text = $"Player {OwnerId}";
        }
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

        // Owner never shows their own label
        if (IsOwner) return;

        nameText.text = playerName;
        nameText.gameObject.SetActive(true);
    }
}
