using System.Collections;
using UnityEngine;

// Place one of these at each entry point in a scene.
// Give it an entranceId that matches the targetEntranceId
// on the ZoneTransition that leads here.
public class ZoneEntrance : MonoBehaviour
{
    [Tooltip("Must match the targetEntranceId on the ZoneTransition " +
             "that leads to this scene.")]
    [SerializeField] private string entranceId;

    private void Start()
    {
        if (ZoneTransition.RequestedEntranceId == entranceId)
        {
            // Wait one frame so GameSaveManager finishes loading first
            StartCoroutine(SpawnPlayer());
        }
    }

    private IEnumerator SpawnPlayer()
    {
        yield return null; // wait one frame

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) yield break;

        CharacterController cc =
            player.GetComponent<CharacterController>();

        // Must disable CharacterController to move transform directly
        if (cc != null) cc.enabled = false;
        player.transform.position = transform.position;
        if (cc != null) cc.enabled = true;

        // Clear so other entrances in this scene don't also trigger
        ZoneTransition.RequestedEntranceId = null;

        Debug.Log("Player spawned at entrance: " + entranceId);
    }

    // Draw a green sphere in the Scene view so you can see spawn points
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            transform.position + Vector3.up,
            "Entrance: " + entranceId
        );
#endif
    }
}
