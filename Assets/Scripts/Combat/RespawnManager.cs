using System.Collections;
using UnityEngine;

/// <summary>
/// Handles player death and respawn.
/// Respawn point is found at runtime by the "RespawnPoint" tag so
/// this prefab component never needs a scene-object Inspector reference.
/// </summary>
public class RespawnManager : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 3f;

    private Health           health;
    private CharacterController controller;

    private void Awake()
    {
        health     = GetComponent<Health>();
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()  { health.OnDeath += HandleDeath; }
    private void OnDisable() { health.OnDeath -= HandleDeath; }

    private void HandleDeath() => StartCoroutine(RespawnRoutine());

    private IEnumerator RespawnRoutine()
    {
        Debug.Log(gameObject.name + " respawning...");

        PlayerController movement = GetComponent<PlayerController>();
        CombatController combat   = GetComponent<CombatController>();

        if (movement != null) movement.enabled = false;
        if (combat   != null) combat.enabled   = false;

        yield return new WaitForSeconds(respawnDelay);

        // Find the respawn point by tag at runtime — avoids the
        // prefab-cannot-reference-scene-object limitation.
        GameObject respawnObj = GameObject.FindGameObjectWithTag("RespawnPoint");
        Vector3 respawnPos = respawnObj != null
            ? respawnObj.transform.position
            : Vector3.zero;

        if (respawnObj == null)
            Debug.LogWarning("RespawnManager: no GameObject tagged 'RespawnPoint' found. Respawning at world origin.");

        controller.enabled    = false;
        transform.position    = respawnPos;
        controller.enabled    = true;

        health.RestoreFullHealth();

        if (movement != null) movement.enabled = true;
        if (combat   != null) combat.enabled   = true;

        Debug.Log(gameObject.name + " respawned.");
    }
}
