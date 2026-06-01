using UnityEngine;
using UnityEngine.SceneManagement;

// Place this on a trigger collider at the edge of a zone.
// When the player enters it, the game saves and the target scene loads.
public class ZoneTransition : MonoBehaviour
{
    [Header("Destination")]
    [Tooltip("Exact name of the scene to load (must be in Build Settings).")]
    [SerializeField] private string targetScene;

    [Tooltip("The entranceId on the ZoneEntrance in the target scene " +
             "where the player should spawn.")]
    [SerializeField] private string targetEntranceId;

    // Static so the value survives the scene load and ZoneEntrance can read it
    public static string RequestedEntranceId;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Save everything before leaving so inventory/quests persist
        GameSaveManager saveManager =
            FindAnyObjectByType<GameSaveManager>();

        saveManager?.SaveGame();

        // Tell the destination scene which entrance to use
        RequestedEntranceId = targetEntranceId;

        SceneManager.LoadScene(targetScene);
    }

    // Draw a label in the Scene view so you can see where transitions are
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            transform.position + Vector3.up,
            "→ " + targetScene
        );
#endif
    }
}
