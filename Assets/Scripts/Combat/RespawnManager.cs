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

        // Try to find a respawn point by tag. FindGameObjectWithTag throws
        // a UnityException (not null) if the tag doesn't exist in the project,
        // so we catch it and fall back to the player's current position.
        Vector3 respawnPos = transform.position; // default: respawn in place
        try
        {
            GameObject respawnObj = GameObject.FindGameObjectWithTag("RespawnPoint");
            if (respawnObj != null)
                respawnPos = respawnObj.transform.position;
        }
        catch
        {
            Debug.LogWarning("RespawnManager: 'RespawnPoint' tag not defined. Respawning in place.");
        }

        controller.enabled    = false;
        transform.position    = respawnPos;
        controller.enabled    = true;

        health.RestoreFullHealth();

        if (movement != null) movement.enabled = true;
        if (combat   != null) combat.enabled   = true;

        Debug.Log(gameObject.name + " respawned.");
    }
}
