using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 3f;

    [SerializeField] private Transform respawnPoint;

    private Health health;

    private CharacterController controller;

    private void Awake()
    {
        health = GetComponent<Health>();

        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
{
    Debug.Log(gameObject.name + " respawning...");

    PlayerController movement =
        GetComponent<PlayerController>();

    CombatController combat =
        GetComponent<CombatController>();

    if (movement != null)
    {
        movement.enabled = false;
    }

    if (combat != null)
    {
        combat.enabled = false;
    }

    yield return new WaitForSeconds(respawnDelay);

    controller.enabled = false;

    transform.position = respawnPoint.position;

    controller.enabled = true;

    health.RestoreFullHealth();

    if (movement != null)
    {
        movement.enabled = true;
    }

    if (combat != null)
    {
        combat.enabled = true;
    }

    Debug.Log(gameObject.name + " respawned.");
}
}