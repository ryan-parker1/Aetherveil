using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{

    public Dictionary<AbilityData, float> CooldownTimers
        => cooldownTimers;

    public List<AbilityData> Abilities
        => abilities;
    
    [SerializeField]
    private List<AbilityData> abilities =
        new List<AbilityData>();

    private TargetingController targetingController;

    private Dictionary<AbilityData, float> cooldownTimers =
        new Dictionary<AbilityData, float>();

    private void Awake()
    {
        targetingController =
            GetComponent<TargetingController>();
    }

    private void Update()
    {
        HandleAbilityInput();
    }

    private void HandleAbilityInput()
    {
        foreach (AbilityData ability in abilities)
        {
            if (Input.GetKeyDown(ability.keybind))
            {
                TryUseAbility(ability);
            }
        }
    }

    private void TryUseAbility(AbilityData ability)
    {
        if (targetingController.CurrentTarget == null)
        {
            Debug.Log("No target selected.");
            return;
        }

        if (cooldownTimers.ContainsKey(ability))
        {
            if (Time.time < cooldownTimers[ability])
            {
                Debug.Log(
                    ability.abilityName +
                    " is on cooldown."
                );

                return;
            }
        }

        float distance =
            Vector3.Distance(
                transform.position,
                targetingController.CurrentTarget.transform.position
            );

        if (distance > ability.range)
        {
            Debug.Log("Target out of range.");
            return;
        }

        Damageable damageable =
            targetingController.CurrentTarget
                .GetComponent<Damageable>();

        if (damageable == null)
            return;

        damageable.ReceiveDamage(ability.damage);

        cooldownTimers[ability] =
            Time.time + ability.cooldown;

        Debug.Log(
            "Used ability: " +
            ability.abilityName
        );
    }
}